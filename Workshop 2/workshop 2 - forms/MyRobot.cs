using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AForge;
using System.Threading;
using System.Collections.Concurrent;
using AForge.Imaging.Filters;
using AForge.Imaging;

namespace Rovio
{
    class MyRobot : Robot
    {
        public static BlockingCollection<Stats> queue = new BlockingCollection<Stats>(10);
        public static AutoResetEvent Notifier = new AutoResetEvent(false);

        
        public MyRobot(string address, string user, string password)
            : base(address, user, password)
        {
          //  Thread ActionPlanningThread = new Thread(new ThreadStart(new ActionPlanning().process));
           // ActionPlanningThread.IsBackground = true;
           // ActionPlanningThread.Start();

           // Thread FeatureExtractionThread = new Thread(new ThreadStart(new FeatureExtracting().process));
           // FeatureExtractionThread.IsBackground = true;
           // FeatureExtractionThread.Start();

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
                //Crop crop = new Crop(new Rectangle(0,100,352,178));
                //Crop crop = new Crop(new Rectangle(0, 88, 352, 200));
                //Bitmap image = crop.Apply(this.Camera.Image);
                Bitmap image = this.Camera.Image;
                HSLFiltering filt = new HSLFiltering();
                filt.Hue = new IntRange(90, 150);
                filt.Saturation = new Range(0.3f, 0.9f);
                filt.Luminance = new Range(0.0f, 0.9f);
                filt.FillOutsideRange = true;
                filt.ApplyInPlace(image);
                BlobCounter bc = new BlobCounter();
                bc.MinWidth = 70;
                bc.MinHeight = 70;
                bc.FilterBlobs = true;
                bc.ProcessImage(image);
                Rectangle[] rects = bc.GetObjectsRectangles();
                Rectangle biggest = new Rectangle(0, 0, 0, 0);
                Graphics g = Graphics.FromImage(image);
                //double ratio = 0;

                foreach (Rectangle r in rects)
                {
                    g.DrawRectangle(new Pen(Color.Green,3), r);
                }
                int objectCeter = 0;
                if (biggest.Width > 15)
                {
                    objectCeter = (((biggest.Width / 2) + biggest.X) - image.Width / 2);
                }
                
                g.DrawRectangle(new Pen(Color.Blue), biggest);

                string drawString = biggest.Height + " <-- Height    Width --> " + biggest.Width + "\n Image Center = " + objectCeter;
                System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 8);
                System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                float x = 10.0F;
                float y = 10.0F;
                System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
                g.DrawString(drawString, drawFont, drawBrush, x, y, drawFormat);
                drawFont.Dispose();
                drawBrush.Dispose();
                //Merge a = new Merge(image);
               // Program.mainForm.VideoViewer.Image = a.Apply(crop.Apply(this.Camera.Image));
                Program.mainForm.VideoViewer.Image = image;
                Console.WriteLine(rects.Length);

                if (objectCeter < -40) 
                {
                    
                   // this.Request("rev.cgi?Cmd=nav&action=18&drive=5&speed=4&angle=1");
                 //   this.Request("rev.cgi?Cmd=nav&action=18&drive=0&speed=0");
                }
                else if (objectCeter > 40) 
                {
                //    this.Request("rev.cgi?Cmd=nav&action=18&drive=6&speed=4&angle=1");
                //    this.Request("rev.cgi?Cmd=nav&action=18&drive=0&speed=0");
                }
                /*
                 * Green H = 110  20
                 * Green S = 0.4 1
                 * Green V = 0.5 0.9
                 * 
                 * Red H =  0  20
                 * Red S = 0.5f  1
                 * Red V = 0.3f  1 
                 * 
                 * Blue H = 225  25
                 * Blue S = 0.4f 1
                 * Blue V = 0.5f 0.9
                 * 
                 * Yellow wall H = 65 20
                 * Yellow wall S = 0.3 0.8
                 * Yellow wall V = 0.25 0.875
                 * 
                 * White wall H = 180  180
                 * white wall S = 0 0.2
                 * White wall V = 0.55  1
                 * 
                 */

                //ProcessImage.queue.Add();
               // ProcessImage.Notifier.Set();

               }
        }
    }
}
