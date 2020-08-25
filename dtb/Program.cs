using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dtb
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool nextIsAffinity = false;
            bool disableThreadBoost = true;
            long affinity = -1;
            foreach( var opt in Environment.GetCommandLineArgs())
            {
                if (nextIsAffinity)
                {
                    nextIsAffinity = false;
                    affinity = long.Parse(opt);

                } else if(opt == "-a")
                {
                    nextIsAffinity = true;
                } else if(opt == "-n")
                {
                    disableThreadBoost = false;
                } 
            }
            Application.Run(new MainForm(disableThreadBoost, affinity));

        }
    }
}
