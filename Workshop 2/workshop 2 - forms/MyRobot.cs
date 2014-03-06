using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AForge;
using System.Threading;

namespace Rovio
{
    class MyRobot : Robot
    {

        private System.Threading.Thread imageProcessingThread { get; set; }
        private System.Threading.Thread FeatureExtractionThread { get; set; }
        private System.Threading.Thread ActionPlanningThread { get; set; }
        private ProcessImage pi  { get; set; }
        private FeatureExtracting fe { get; set; }
        private ActionPlanning ap { get; set; }

        

        public MyRobot(string address, string user, string password)
            : base(address, user, password)
        {

            ap = new ActionPlanning(this);
            ActionPlanningThread = new Thread(new ThreadStart(ap.process));
            ActionPlanningThread.IsBackground = true;
            ActionPlanningThread.Start();

            fe = new FeatureExtracting(ap);
            FeatureExtractionThread = new Thread(new ThreadStart(fe.process));
            FeatureExtractionThread.IsBackground = true;
            FeatureExtractionThread.Start();

            pi = new ProcessImage(fe);
            imageProcessingThread = new Thread(new ThreadStart(pi.process));
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
                //Bitmap image = this.Camera.Image;

                pi.add(this.Camera.Image);
                Bitmap t = this.Camera.Image;

                //emit events
                //SourceImage(t);
            }
        }
    }
}
