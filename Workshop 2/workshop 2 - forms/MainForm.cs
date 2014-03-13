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

namespace Rovio
{
    public partial class MainForm : Form
    {
        private BaseRobot ron;
        public Bitmap map;
        public Bitmap robotIcon;
        public MainForm()
        {
            InitializeComponent();
        }

        private System.Threading.Thread robot_thread;

        private void MainForm_Load(object sender, EventArgs e)
        {
            map = global::Rovio.Properties.Resources.Map;
            robotIcon = global::Rovio.Properties.Resources.SmallestRobot;

            //instantiate the robot object
            ron = new User("http://10.82.0.33/", "user", "password");
            //create and start the robot thread: your own implementation in MyRobot class
            robot_thread = new System.Threading.Thread(new System.Threading.ThreadStart(ron.runRovio));
            robot_thread.IsBackground = true;
            robot_thread.Start();
        }

        private void Predator_Button_Click(object sender, EventArgs e)
        {
            Predator_Button.Enabled = false;
            robot_thread.Abort();
            ron = new predator("http://10.82.0.33/", "user", "password");
            robot_thread = new System.Threading.Thread(new System.Threading.ThreadStart(ron.runRovio));
            robot_thread.IsBackground = true;
            robot_thread.Start();
            User_Button.Enabled = true;
            Prey_Button.Enabled = true;
        }

        private void Prey_Button_Click(object sender, EventArgs e)
        {
            Prey_Button.Enabled = false;
            robot_thread.Abort();
            ron = new Prey("http://10.82.0.33/", "user", "password");
            robot_thread = new System.Threading.Thread(new System.Threading.ThreadStart(ron.runRovio));
            robot_thread.IsBackground = true;
            robot_thread.Start();
            Predator_Button.Enabled = true;
            User_Button.Enabled = true;
        }

        private void User_Button_Click(object sender, EventArgs e)
        {
            User_Button.Enabled = false;
            robot_thread.Abort();
            ron = new User("http://10.82.0.33/", "user", "password");
            robot_thread = new System.Threading.Thread(new System.Threading.ThreadStart(ron.runRovio));
            robot_thread.IsBackground = true;
            robot_thread.Start();
            Prey_Button.Enabled = true;
            Predator_Button.Enabled = true;
        }

        private void Close_Button_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}