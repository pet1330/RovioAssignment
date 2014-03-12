using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using AForge.Imaging.Filters;

namespace Rovio
{
    class FeatureExtracting
    {
                //Vars
        public static BlockingCollection<Bitmap> queue = new BlockingCollection<Bitmap>(10);
        public static AutoResetEvent Notifier = new AutoResetEvent(false);

        //constructor
        public FeatureExtracting(){}
        
        //functions
        public void process() 
        {
            while (true)
            {
                FeatureExtracting.Notifier.WaitOne();
                foreach (Bitmap image in queue.GetConsumingEnumerable())
                {
                    AForge.Imaging.BlobCounter bc = new AForge.Imaging.BlobCounter(image);
                    Rectangle[] rects = bc.GetObjectsRectangles();
                    Rectangle biggest = new Rectangle(0, 0, 0, 0);
                    Graphics g = Graphics.FromImage(image);
                    double ratio = 0;

                    foreach (Rectangle r in rects)
                    {
                        ratio = (r.Height + 1.0) / (r.Width + 1.0);
                        if (biggest.Width * biggest.Height < r.Width * r.Height)
                        {
                            //check ratio
                            if ((ratio < 1.1) && (ratio > 0.45))
                            {
                                if (r.Width * r.Height > 200)
                                {
                                    biggest = r;
                                }
                            }
                        }
                    }

                    int imageCenter = (biggest.Width / 2) + biggest.X;
                    int screenCenter = image.Width / 2;

                    g.DrawRectangle(new Pen(Color.FromArgb(255, 0, 0)), biggest);
                    string drawString = biggest.Height + " <-Height   Width-> " + biggest.Width + "\nratio = " + ratio + "\n Image Center = " + imageCenter + "\nScreen Center = " + screenCenter;
                    System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 11);
                    System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                    float x = 10.0F;
                    float y = 10.0F;
                    System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
                    g.DrawString(drawString, drawFont, drawBrush, x, y, drawFormat);
                    drawFont.Dispose();
                    drawBrush.Dispose();

                    Stats a = new Stats();
                    a.blobCount = 0;
                    ActionPlanning.queue.Add(a);
                    ActionPlanning.Notifier.Set();

                    #if DEBUG
                    Program.mainForm.VideoViewer.Image = image;
                    #endif
                }
            }
        }
    }
}