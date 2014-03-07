using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Rovio
{
    static class Program
    {
#if DEBUG
        public static MainForm mainForm;
#endif
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#if DEBUG
            mainForm = new MainForm();
             Application.Run(mainForm);
#else
Application.Run(new MainForm());
#endif
        }
    }
}
