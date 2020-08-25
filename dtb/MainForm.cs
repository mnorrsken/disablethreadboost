using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Management;

namespace dtb
{
    public partial class MainForm : Form
    {
        private IntPtr affinity = IntPtr.Zero;
        private bool disableThreadBoost = false;
        private bool setPriority = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disableThreadBoost">Disables FlightSim threadboost if true, else do not change the value</param>
        /// <param name="affinity">Autocalc affinity if set to -1, or sets affinity to the value if > 0, if 0 do not change affinity</param>
        public MainForm(bool disableThreadBoost, long affinity, bool setPriority)
        {
            this.disableThreadBoost = disableThreadBoost;
            this.setPriority = setPriority;
            InitializeComponent();
            if (affinity == -1)
            {
                CalcAffinityMask();
            } else if (affinity > 0)
            {
                this.affinity = (IntPtr)affinity;
            }
        }

        private void CalcAffinityMask()
        {
            ManagementObjectSearcher searcher =
               new ManagementObjectSearcher("root\\CIMV2",
               "SELECT * FROM Win32_Processor");

            uint nrCores = 0;
            uint nrLogical = 0;

            foreach (ManagementObject queryObj in searcher.Get())
            {
                nrCores += (uint)queryObj["NumberOfCores"];
                nrLogical += (uint)queryObj["NumberOfLogicalProcessors"];
            }

            ulong mask = (1ul << (int)nrLogical) - 1ul;

            string htInfo;
            if(nrLogical > nrCores)
            {
                htInfo = "enabled";
                mask = mask & ~11ul;
            } else
            {
                htInfo = "disabled";
                mask = mask & ~1ul;
            }

            notifyIcon1.ShowBalloonTip(5000,"PerfFix 2020", $"Detected {nrCores} cores, HT {htInfo}\nSetting mask to {mask}", ToolTipIcon.Info);
            this.affinity = (IntPtr)mask;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var fsprocs = Process.GetProcessesByName("FlightSimulator");
            foreach (Process item in fsprocs) {
                List<string> msg = new List<string>();
                if (this.disableThreadBoost)
                {
                    if (item.PriorityBoostEnabled)
                    {
                        msg.Add("Disabling Thread Boost");
                        item.PriorityBoostEnabled = false;
                    }
                }
                if (this.setPriority)
                {
                    if (item.PriorityClass != ProcessPriorityClass.Idle)
                    {
                        msg.Add("Setting Priority Low");
                        item.PriorityClass = ProcessPriorityClass.Idle;
                    }
                }
                if (this.affinity != IntPtr.Zero)
                {
                    if(this.affinity != item.ProcessorAffinity)
                    {
                        msg.Add($"Setting Affinity {Convert.ToString((long)item.ProcessorAffinity, 2)} -> {Convert.ToString((long)this.affinity,2)}");
                        item.ProcessorAffinity = this.affinity;
                    }

                }

                if (msg.Count > 0) {
                    string sMessage = string.Join(", ", msg.ToArray());
                    notifyIcon1.ShowBalloonTip(3000, "PerfFix 2020", $"Detected {item.ProcessName} [{item.Id}]: {sMessage}", ToolTipIcon.Info);
                }
            }

            
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
