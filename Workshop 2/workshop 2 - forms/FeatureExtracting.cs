using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Collections.Concurrent;

namespace Rovio
{
    class FeatureExtracting
    {
                //Vars
        public BlockingCollection<Bitmap> queue { get; set; }
        ActionPlanning ap;

        //constructor
        public FeatureExtracting(ActionPlanning _ap) 
        {
            ap = _ap;
            queue = new BlockingCollection<Bitmap>(10);
        }


        //functions
        public void process() 
        {
            while (true)
            {
                while (queue.Count > 0)
                {
                    Bitmap image = queue.Take();

                    AForge.Imaging.BlobCounter bc = new AForge.Imaging.BlobCounter(image);
                    Rectangle[] r = bc.GetObjectsRectangles();
                    FeatureStatistics a = new FeatureStatistics();
                    a.blobCount = r.Length;
                    ap.add(a);
                    Program.mainForm.pictureBox1.Image = image;
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
