using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using Rovio.Properties;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Threading;

namespace Rovio
{
    public partial class MainForm : Form
    {
        private BaseRobot ron;
        private Thread robot_thread;
        private String[] login = { "http://10.82.0.33/", "user", "password" };

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ron = new User(login[0], login[1], login[2]);
            robot_thread = new System.Threading.Thread(new System.Threading.ThreadStart(ron.runRovio));
            robot_thread.IsBackground = true;
            robot_thread.Start();
        }

        private void Predator_Button_Click(object sender, EventArgs e)
        {
            ChangeMode(1);
        }

        private void Prey_Button_Click(object sender, EventArgs e)
        {
            ChangeMode(2);
        }

        private void User_Button_Click(object sender, EventArgs e)
        {
            ChangeMode(0);
        }

        private void Close_Button_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ChangeMode(int mode)
        {
            Predator_Button.Enabled = false;
            User_Button.Enabled = false;
            Prey_Button.Enabled = false;
            ron.terminateRovio();
            switch (mode)
            {
                case 1:
                    ron = new predator(login[0], login[1], login[2]);
                    Predator_Button.Enabled = false;
                    User_Button.Enabled = true;
                    Prey_Button.Enabled = true;
                    break;
                case 2:
                    ron = new Prey(login[0], login[1], login[2]);
                    Predator_Button.Enabled = true;
                    User_Button.Enabled = true;
                    Prey_Button.Enabled = false;
                    break;
                default:
                    ron = new User(login[0], login[1], login[2]);
                    Predator_Button.Enabled = true;
                    User_Button.Enabled = false;
                    Prey_Button.Enabled = true;
                    break;
            }
            robot_thread = new System.Threading.Thread(new System.Threading.ThreadStart(ron.runRovio));
            robot_thread.IsBackground = true;
            robot_thread.Start();
        }
    }
}