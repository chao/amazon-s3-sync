using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Fr.Zhou.S3
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
            Application.Run(new S3Notify());
        }
    }
}
