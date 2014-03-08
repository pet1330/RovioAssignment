﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Rovio
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private System.Threading.Thread robot_thread;

        private void MainForm_Load(object sender, EventArgs e)
        {
            //instantiate the robot object
            MyRobot ron = new MyRobot("http://10.82.0.33/", "user", "password");
            //create and start the robot thread: your own implementation in MyRobot class
            robot_thread = new System.Threading.Thread(new System.Threading.ThreadStart(ron.ProcessImages));
            robot_thread.Start();


            VideoViewer.Location = new Point(0, 0);
            VideoViewer.Size = this.ClientSize;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //close the robot thread
            robot_thread.Abort();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            VideoViewer.Size = this.ClientSize;
        }


        //===================================================================================================================================
        
        private delegate void UpdateImageValue(Image image);

        //update the picture box content
        public void UpdateImage(Image image)
        {
            VideoViewer.Image = image;
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(delegate { UpdateImage(image); }));
            else
                this.ClientSize = VideoViewer.Image.Size;
        }

        //===================================================================================================================================
    }
}
