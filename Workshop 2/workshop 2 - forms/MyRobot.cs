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

            ActionPlanning ap = new ActionPlanning();
            Thread ActionPlanningThread = new Thread(new ThreadStart(ap.process));
             ActionPlanningThread.IsBackground = true;
            ActionPlanningThread.Start();

            FeatureExtracting fe = new FeatureExtracting();
            Thread FeatureExtractionThread = new Thread(new ThreadStart(fe.process));
            FeatureExtractionThread.IsBackground = true;
            FeatureExtractionThread.Start();

            ProcessImage pi = new ProcessImage();
            Thread imageProcessingThread = new Thread(new ThreadStart(pi.process));
            imageProcessingThread.IsBackground = true;
            imageProcessingThread.Start();

            


        }

        public delegate void ImageReady(Image image);
        public event ImageReady SourceImage;

        public void ProcessImages()
        {
            //check if we can receive responses from the robot
            try { API.Movement.GetLibNSVersion(); } // a dummy request
            catch (Exception)
            {
                //simple way of getting feedback in the form mode
                System.Windows.Forms.MessageBox.Show("Could not connect to the robot");
                return;
            }

            //endless loop
            while (true)
            {
              //  foreach (Stats instructions in MyRobot.queue.GetConsumingEnumerable())
                //{
                    
               // }

                //Bitmap image = this.Camera.Image;
                ProcessImage.queue.Add(this.Camera.Image);
                ProcessImage.Notifier.Set();
            }
        }
    }
}
