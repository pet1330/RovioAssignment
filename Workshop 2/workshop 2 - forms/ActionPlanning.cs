using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Collections.Concurrent;

namespace Rovio
{
    class ActionPlanning
    {
                //Vars
        public BlockingCollection<FeatureStatistics> queue { get; set; }
        public MyRobot r { get; set; }
        

        //constructor
        public ActionPlanning(MyRobot _r) 
        {
            queue = new BlockingCollection<FeatureStatistics>(10);
            r = _r;
        }


        //functions
        public void process() 
        {
            while (true)
            {
                while (queue.Count > 0)
                {
                    FeatureStatistics stats = queue.Take();
                }
            }
        }

        public void add(FeatureStatistics im)
        {
            queue.Add(im);
        }

        private FeatureStatistics consume()
        {
            return queue.Take();
        }

    }
}
