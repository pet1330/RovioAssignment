using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AForge;
using System.Threading;
using System.Collections.Concurrent;

namespace Rovio
{
    class MyRobot : Robot
    {

        public static BlockingCollection<Stats> queue = new BlockingCollection<Stats>(10);
        public static AutoResetEvent Notifier = new AutoResetEvent(false);

        
        public MyRobot(string address, string user, string password)
            : base(address, user, password)
        {
            Thread ActionPlanningThread = new Thread(new ThreadStart(new ActionPlanning().process));
            ActionPlanningThread.IsBackground = true;
            ActionPlanningThread.Start();

            Thread FeatureExtractionThread = new Thread(new ThreadStart(new FeatureExtracting().process));
            FeatureExtractionThread.IsBackground = true;
            FeatureExtractionThread.Start();

            Thread imageProcessingThread = new Thread(new ThreadStart(new ProcessImage().process));
            imageProcessingThread.IsBackground = true;
            imageProcessingThread.Start();
        }

        public void ProcessImages()
        {
            //check if we can receive responses from the robot
            try { API.Movement.GetLibNSVersion(); } // a dummy request
            catch (Exception)
            {
                //simple way of getting feedback in the form mode
                System.Windows.Forms.MessageBox.Show("Could not connect to the robot");
                Environment.Exit(1057);
            }

            //endless loop
            while (true)
            {
                ProcessImage.queue.Add(this.Camera.Image);
                ProcessImage.Notifier.Set();

            }
        }
    }
}
