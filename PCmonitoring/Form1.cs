using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management; // za WMI
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices; // za RAM info

namespace PCmonitoring
{
    public partial class Form1 : Form
    {
        PerformanceCounter cpuCounter;

        // alert flags
        bool cpuAlertShown = false;
        bool ramAlertShown = false;
        bool diskAlertShown = false;

        public Form1()
        {
            InitializeComponent();

            // ListView setup
            listViewProcesses.View = View.Details;
            listViewProcesses.FullRowSelect = true;
            listViewProcesses.GridLines = true;
            listViewProcesses.Columns.Clear();
            listViewProcesses.Columns.Add("Process Name", 200);
            listViewProcesses.Columns.Add("PID", 70);
            listViewProcesses.Columns.Add("Memory (MB)", 100);

            // CPU counter
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            // Progress bars
            progressBarCPU.Minimum = 0;
            progressBarCPU.Maximum = 100;

            progressBarRAM.Minimum = 0;
            progressBarRAM.Maximum = 16000; // prilagodi po RAM-u

            progressBarDisk.Minimum = 0;
            progressBarDisk.Maximum = 100;

            // inicijalno učitavanje
            btnRefresh_Click_1(this, EventArgs.Empty);
        }

        private void UpdateStats()
        {
            // CPU
            float cpuUsage = cpuCounter.NextValue();
            System.Threading.Thread.Sleep(500);
            cpuUsage = cpuCounter.NextValue();
            progressBarCPU.Value = (int)cpuUsage;

            ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2",
                "SELECT Name, CurrentClockSpeed FROM Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                string name = mo["Name"]?.ToString();
                uint clock = (uint)mo["CurrentClockSpeed"];
                lblCPU.Text = $"{name} | {cpuUsage:F0}% | {clock} MHz";
            }

            // RAM
            ComputerInfo ci = new ComputerInfo();
            ulong totalRam = ci.TotalPhysicalMemory / 1024 / 1024;
            ulong availableRam = ci.AvailablePhysicalMemory / 1024 / 1024;
            ulong usedRam = totalRam - availableRam;

            progressBarRAM.Maximum = (int)totalRam;
            progressBarRAM.Value = (int)usedRam;
            lblRAM.Text = $"RAM: {usedRam} MB used / {totalRam} MB total";

            // Disk
            DriveInfo drive = DriveInfo.GetDrives().FirstOrDefault(d => d.Name == "C:\\");
            if (drive != null && drive.IsReady)
            {
                long used = drive.TotalSize - drive.TotalFreeSpace;
                int percent = (int)((double)used / drive.TotalSize * 100);
                progressBarDisk.Value = percent;
                lblDisk.Text = $"Disk C: {percent}% used";

                // Basic SMART status
                ManagementObjectSearcher mosDisk = new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT Status, DeviceID FROM Win32_DiskDrive");
                foreach (ManagementObject moDisk in mosDisk.Get())
                {
                    string status = moDisk["Status"]?.ToString();
                    lblDisk.Text += $"\nDrive {moDisk["DeviceID"]}: {status}";
                }
            }
        }

        private void GetRAMSticksInfo()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2",
                "SELECT Capacity, Speed, Manufacturer FROM Win32_PhysicalMemory");

            string ramDetails = "";
            ulong totalRam = 0;

            foreach (ManagementObject mo in mos.Get())
            {
                ulong capacity = (ulong)mo["Capacity"] / 1024 / 1024; // MB
                uint speed = (uint)mo["Speed"]; // MHz
                string manufacturer = mo["Manufacturer"]?.ToString();

                totalRam += capacity;
                ramDetails += $"{manufacturer} {capacity}MB {speed}MHz\n";
            }

            lblRAM.Text = $"Total RAM: {totalRam} MB\n{ramDetails}";
        }

        private void LoadProcesses()
        {
            listViewProcesses.Items.Clear();
            var processes = Process.GetProcesses().OrderBy(p => p.ProcessName);
            foreach (var proc in processes)
            {
                ListViewItem item = new ListViewItem(proc.ProcessName);
                item.SubItems.Add(proc.Id.ToString());
                try
                {
                    item.SubItems.Add((proc.WorkingSet64 / 1024 / 1024).ToString("F0"));
                }
                catch
                {
                    item.SubItems.Add("N/A");
                }
                listViewProcesses.Items.Add(item);
            }
        }

        private void CheckAlerts()
        {
            float cpuUsage = cpuCounter.NextValue();

            ComputerInfo ci = new ComputerInfo();
            ulong availableRam = ci.AvailablePhysicalMemory / 1024 / 1024;

            DriveInfo drive = DriveInfo.GetDrives().FirstOrDefault(d => d.Name == "C:\\");

            // CPU ALERT
            if (cpuUsage > 90 && !cpuAlertShown)
            {
                cpuAlertShown = true;
                MessageBox.Show("ALERT: CPU usage over 90%","WARNING!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }

            // RAM ALERT
            if (availableRam < 10000 && !ramAlertShown)
            {
                ramAlertShown = true;
                MessageBox.Show("ALERT: Low RAM available!","WARNING!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // DISK ALERT
            if (drive != null && drive.IsReady)
            {
                long used = drive.TotalSize - drive.TotalFreeSpace;
                int percent = (int)((double)used / drive.TotalSize * 100);

                if (percent > 90 && !diskAlertShown)
                {
                    diskAlertShown = true;
                    MessageBox.Show("ALERT: Disk usage over 90%","WARNING!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // Refresh dugme
        private void btnRefresh_Click_1(object sender, EventArgs e)
        {
            UpdateStats();
            LoadProcesses();
            GetRAMSticksInfo();
            CheckAlerts();
        }

        // Export dugme
        private void btnExport_Click_1(object sender, EventArgs e)
        {
            string filePath = "process_report.csv";
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("ProcessName,PID,Memory(MB)");
                foreach (ListViewItem item in listViewProcesses.Items)
                {
                    sw.WriteLine($"{item.SubItems[0].Text},{item.SubItems[1].Text},{item.SubItems[2].Text}");
                }
            }
            MessageBox.Show($"Process report exported to {filePath}");
        }

        // TIMER1_Tick
        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateStats();
            LoadProcesses();
            CheckAlerts();
        }
    }
}