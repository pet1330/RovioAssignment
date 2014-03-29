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
        public static BlockingCollection<Bitmap> queue = new BlockingCollection<Bitmap>(10);
        public static AutoResetEvent Notifier = new AutoResetEvent(false);
        //constructor
        public ProcessImage() { }

        //functions
        public void process(Bitmap image)
        {
            while (true)
            {
                ProcessImage.Notifier.WaitOne();
                foreach (Bitmap _image in queue.GetConsumingEnumerable())//it will block here automatically waiting from new items to be added and it will not take cpu down 
                {
                    //Red
                    HSLFiltering redFilter = new HSLFiltering(new IntRange(345, 15), new Range(0.400f, 1.0f), new Range(0.15f, 1.0f));

                    HSLFiltering yellowFilter = new HSLFiltering(new IntRange(345, 15), new Range(0.400f, 1.0f), new Range(0.15f, 1.0f));

                    HSLFiltering whiteFilter = new HSLFiltering(new IntRange(345, 15), new Range(0.400f, 1.0f), new Range(0.15f, 1.0f));

                    HSLFiltering greenFilter = new HSLFiltering(new IntRange(345, 15), new Range(0.400f, 1.0f), new Range(0.15f, 1.0f));

                    HSLFiltering blueFilter = new HSLFiltering(new IntRange(345, 15), new Range(0.400f, 1.0f), new Range(0.15f, 1.0f));


                    redFilter.FillOutsideRange = true;
                    greenFilter.FillOutsideRange = true;
                    blueFilter.FillOutsideRange = true;
                    whiteFilter.FillOutsideRange = true;
                    yellowFilter.FillOutsideRange = true;

                    redFilter.ApplyInPlace(_image);
                    blueFilter.ApplyInPlace(_image);
                    greenFilter.ApplyInPlace(_image);
                    yellowFilter.ApplyInPlace(_image);
                    whiteFilter.ApplyInPlace(_image);

                    FeatureExtracting.queue.Add(_image);
                    FeatureExtracting.Notifier.Set();
                }
            }
        }
    }
}
   


