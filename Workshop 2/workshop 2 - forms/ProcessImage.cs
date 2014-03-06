using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Collections.Concurrent;

namespace Rovio
{
    class ProcessImage
    {
        //Vars
        public BlockingCollection<Bitmap> queue { get; set; }

        FeatureExtracting fe;
        //constructor
        public ProcessImage(FeatureExtracting _fe) 
        {
            fe = _fe;
            queue = new BlockingCollection<Bitmap>(10);
        }


        //functions
        public void process() 
        {
            while (true)
            {
                while (queue.Count>0)
                {
                    Bitmap image =  queue.Take();
                    AForge.Imaging.Filters.ColorFiltering test = new AForge.Imaging.Filters.ColorFiltering();
                    test.Red = new AForge.IntRange(40, 120);
                    test.Green = new AForge.IntRange(40, 120);
                    test.Blue = new AForge.IntRange(40, 120);
                    test.ApplyInPlace(image);
                    fe.add(image);
                    
                }
            }
        }

        public void add(Bitmap im) 
        {
            queue.Add(im);
        }

        private Bitmap consume() 
        {
            return queue.Take();
        }

    }
}
