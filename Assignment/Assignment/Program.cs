using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Rovio
{
    static class Program
    {
        public static MainForm mainForm;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainForm = new MainForm();
            Application.Run(mainForm);
        }
    }
}