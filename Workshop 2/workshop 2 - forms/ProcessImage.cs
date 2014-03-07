using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Collections.Concurrent;
using AForge;
using AForge.Imaging.Filters;

namespace Rovio
{
    class ProcessImage
    {
        //Vars
        public static BlockingCollection<Bitmap> queue = new BlockingCollection<Bitmap>(10);
        public static AutoResetEvent Notifier = new AutoResetEvent(false);
        //constructor
        public ProcessImage(){}

        //functions
        public void process() 
        {
            while (true)
            {
                ProcessImage.Notifier.WaitOne();
                foreach (Bitmap image in queue.GetConsumingEnumerable())//it will block here automatically waiting from new items to be added and it will not take cpu down 
                {
                    ColorFiltering filter = new ColorFiltering();
                    filter.Red = new IntRange(100, 255);
                    filter.Green = new IntRange(0, 75);
                    filter.Blue = new IntRange(0, 75);
                    filter.ApplyInPlace(image);
                    FeatureExtracting.queue.Add(image);
                    FeatureExtracting.Notifier.Set();
                }
            }
        }
    }
}
