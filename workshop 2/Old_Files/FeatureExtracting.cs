using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge;

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

                    BlobCounter bc = new BlobCounter();
                    bc.MinWidth = 5;
                    bc.MinHeight = 5;
                    bc.FilterBlobs = true;
                    bc.ObjectsOrder = ObjectsOrder.Size;
                    bc.ProcessImage(image);
                    Rectangle[] rects = bc.GetObjectsRectangles();
                    Rectangle biggest = new Rectangle(0, 0, 0, 0);
                    Graphics g = Graphics.FromImage(image);
                    //double ratio = 0;
                        
foreach (Blob blob in bc.GetObjectsInformation())
{
     List<IntPoint> edgePoints = bc.GetBlobsEdgePoints(blob);
     List<IntPoint> top;
     List<IntPoint> bottom;
     bc.GetBlobsTopAndBottomEdges(blob, out top, out bottom);

}
                    
                    foreach (Rectangle r in rects)
                    {

                        biggest = rects[0];
                        g.DrawRectangle(new Pen(Color.Green, 3), r);
                    }
                    

                    int objectCeter = 0;
                    if (biggest.Width > 70)
                    {
                        objectCeter = (((biggest.Width / 2) + biggest.X) - image.Width / 2);
                    }

                    g.DrawRectangle(new Pen(Color.Blue), biggest);


                    if (biggest.Height < 20) 
                    { 
                    
                    }


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