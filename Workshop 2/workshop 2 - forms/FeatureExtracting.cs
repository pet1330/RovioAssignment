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