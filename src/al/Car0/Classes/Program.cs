using System;
using System.Collections.Generic;

using System.Windows.Forms;

namespace Car0
{
     class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
         void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new KUKAcar0());
        }
    }
}
