using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace PCmonitoring
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==============================================");
            Console.WriteLine("         PC MONITORING SYSTEM");
            Console.WriteLine("==============================================");
            Console.WriteLine();
            
            int refreshSeconds = args.Length > 0 && int.TryParse(args[0], out var s) ? s : 2;
            
            bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            
            while (true)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
                    Console.WriteLine(new string('-', 47));
                    
                    var cpu = isLinux ? GetCpuUsageLinux() : GetCpuUsageWindows();
                    Console.WriteLine($"  CPU Usage:    {cpu,6:F1}%  {GetBar(cpu)}");
                    
                    var mem = isLinux ? GetMemoryLinux() : GetMemoryWindows();
                    Console.WriteLine($"  Memory Used:  {mem.used,6:F0}MB / {mem.total,6:F0}MB  {GetBar(mem.percent)}");
                    
                    var disk = isLinux ? GetDiskUsageLinux() : GetDiskUsageWindows();
                    Console.WriteLine($"  Disk Used:    {disk.used,6:F0}GB / {disk.total,6:F0}GB  {GetBar(disk.percent)}");
                    
                    Console.WriteLine(new string('-', 47));
                    Console.WriteLine($"  Refresh: {refreshSeconds}s  |  Ctrl+C to exit");
                    Console.WriteLine();
                    
                    Thread.Sleep(refreshSeconds * 1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Thread.Sleep(1000);
                }
            }
        }
        
        static string GetBar(double percent)
        {
            int filled = (int)(percent / 5);
            return "[" + new string('#', Math.Min(filled, 20)) + new string('-', 20 - Math.Min(filled, 20)) + "]";
        }
        
        static double GetCpuUsageLinux()
        {
            var before = File.ReadAllLines("/proc/stat")[0];
            var parts = before.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            long idle1 = long.Parse(parts[4]);
            long total1 = 0;
            for (int i = 1; i < parts.Length; i++) total1 += long.Parse(parts[i]);
            Thread.Sleep(100);
            var after = File.ReadAllLines("/proc/stat")[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            long idle2 = long.Parse(after[4]);
            long total2 = 0;
            for (int i = 1; i < after.Length; i++) total2 += long.Parse(after[i]);
            return 100.0 * (1.0 - (double)(idle2 - idle1) / (total2 - total1));
        }
        
        static double GetCpuUsageWindows()
        {
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            Thread.Sleep(100);
            return cpuCounter.NextValue();
        }
        
        static (double total, double used, double percent) GetMemoryLinux()
        {
            var lines = File.ReadAllLines("/proc/meminfo");
            double total = 0, free = 0, buffers = 0, cached = 0;
            foreach (var line in lines)
            {
                if (line.StartsWith("MemTotal:")) total = double.Parse(line.Split(':')[1].Trim().Split(' ')[0]) / 1024;
                if (line.StartsWith("MemFree:")) free = double.Parse(line.Split(':')[1].Trim().Split(' ')[0]) / 1024;
                if (line.StartsWith("Buffers:")) buffers = double.Parse(line.Split(':')[1].Trim().Split(' ')[0]) / 1024;
                if (line.StartsWith("Cached:")) cached = double.Parse(line.Split(':')[1].Trim().Split(' ')[0]) / 1024;
            }
            double used = total - free - buffers - cached;
            return (total, used, used / total * 100);
        }
        
        static (double total, double used, double percent) GetMemoryWindows()
        {
            var gcMemInfo = GC.GetGCMemoryInfo();
            long totalMemory = gcMemInfo.TotalAvailableMemoryBytes / 1024 / 1024;
            var process = Process.GetCurrentProcess();
            long workingSet = process.WorkingSet64 / 1024 / 1024;
            double percent = (double)workingSet / totalMemory * 100;
            return (totalMemory, workingSet, percent);
        }
        
        static (double total, double used, double percent) GetDiskUsageLinux()
        {
            var lines = File.ReadAllLines("/proc/mounts");
            string? rootDev = null;
            foreach (var line in lines)
            {
                var parts = line.Split(' ');
                if (parts.Length > 1 && parts[1] == "/")
                {
                    rootDev = parts[0];
                    break;
                }
            }
            if (rootDev == null) return (0, 0, 0);
            var drivelines = new Process { StartInfo = new ProcessStartInfo("df", "-B1 --output=source,size,used") { RedirectStandardOutput = true, UseShellExecute = false } };
            drivelines.Start();
            var output = drivelines.StandardOutput.ReadToEnd();
            drivelines.WaitForExit();
            var diskLines = output.Split('\n');
            foreach (var dl in diskLines)
            {
                if (dl.Contains(rootDev))
                {
                    var dp = dl.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (dp.Length >= 3)
                    {
                        double total = double.Parse(dp[1]) / 1024 / 1024 / 1024;
                        double used = double.Parse(dp[2]) / 1024 / 1024 / 1024;
                        return (total, used, used / total * 100);
                    }
                }
            }
            return (0, 0, 0);
        }
        
        static (double total, double used, double percent) GetDiskUsageWindows()
        {
            var drive = new DriveInfo("C");
            double total = drive.TotalSize / 1024.0 / 1024.0 / 1024.0;
            double free = drive.AvailableFreeSpace / 1024.0 / 1024.0 / 1024.0;
            double used = total - free;
            return (total, used, used / total * 100);
        }
    }
}
