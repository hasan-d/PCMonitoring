namespace PCmonitoring
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            progressBarCPU = new ProgressBar();
            progressBarRAM = new ProgressBar();
            progressBarDisk = new ProgressBar();
            lblCPU = new Label();
            lblRAM = new Label();
            lblDisk = new Label();
            btnRefresh = new Button();
            btnExport = new Button();
            listViewProcesses = new ListView();
            timer1 = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // progressBarCPU
            // 
            progressBarCPU.Location = new Point(376, 65);
            progressBarCPU.Name = "progressBarCPU";
            progressBarCPU.Size = new Size(404, 29);
            progressBarCPU.TabIndex = 0;
            // 
            // progressBarRAM
            // 
            progressBarRAM.Location = new Point(376, 135);
            progressBarRAM.Name = "progressBarRAM";
            progressBarRAM.Size = new Size(404, 29);
            progressBarRAM.TabIndex = 0;
            // 
            // progressBarDisk
            // 
            progressBarDisk.Location = new Point(376, 212);
            progressBarDisk.Name = "progressBarDisk";
            progressBarDisk.Size = new Size(404, 29);
            progressBarDisk.TabIndex = 0;
            // 
            // lblCPU
            // 
            lblCPU.AutoSize = true;
            lblCPU.BackColor = Color.Silver;
            lblCPU.BorderStyle = BorderStyle.FixedSingle;
            lblCPU.Location = new Point(33, 65);
            lblCPU.Name = "lblCPU";
            lblCPU.Size = new Size(38, 22);
            lblCPU.TabIndex = 1;
            lblCPU.Text = "CPU";
            // 
            // lblRAM
            // 
            lblRAM.AutoSize = true;
            lblRAM.BackColor = Color.Silver;
            lblRAM.Location = new Point(33, 124);
            lblRAM.Name = "lblRAM";
            lblRAM.Size = new Size(41, 20);
            lblRAM.TabIndex = 1;
            lblRAM.Text = "RAM";
            // 
            // lblDisk
            // 
            lblDisk.AutoSize = true;
            lblDisk.BackColor = Color.Silver;
            lblDisk.Location = new Point(33, 212);
            lblDisk.Name = "lblDisk";
            lblDisk.Size = new Size(41, 20);
            lblDisk.TabIndex = 1;
            lblDisk.Text = "DISK";
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(619, 302);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(161, 29);
            btnRefresh.TabIndex = 3;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click_1;
            // 
            // btnExport
            // 
            btnExport.Location = new Point(619, 364);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(161, 29);
            btnExport.TabIndex = 3;
            btnExport.Text = "Export";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click_1;
            // 
            // listViewProcesses
            // 
            listViewProcesses.Location = new Point(33, 286);
            listViewProcesses.Name = "listViewProcesses";
            listViewProcesses.Size = new Size(514, 121);
            listViewProcesses.TabIndex = 4;
            listViewProcesses.UseCompatibleStateImageBehavior = false;
            listViewProcesses.View = View.Details;
            // 
            // timer1
            // 
            timer1.Interval = 1000;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 444);
            Controls.Add(listViewProcesses);
            Controls.Add(btnExport);
            Controls.Add(btnRefresh);
            Controls.Add(lblDisk);
            Controls.Add(lblRAM);
            Controls.Add(lblCPU);
            Controls.Add(progressBarDisk);
            Controls.Add(progressBarRAM);
            Controls.Add(progressBarCPU);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar progressBarCPU;
        private ProgressBar progressBarRAM;
        private ProgressBar progressBarDisk;
        private Label lblCPU;
        private Label lblRAM;
        private Label lblDisk;
        private Button btnRefresh;
        private Button btnExport;
        private ListView listViewProcesses;
        private System.Windows.Forms.Timer timer1;
    }
}
