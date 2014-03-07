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
        public static BlockingCollection<Stats> queue = new BlockingCollection<Stats>(10);
        public static AutoResetEvent Notifier = new AutoResetEvent(false);

        //constructor
        public ActionPlanning(){}

        //functions
        public void process() 
        {
            while (true)
            {
                ActionPlanning.Notifier.WaitOne();
                ProcessImage.Notifier.WaitOne();
                foreach (Stats stats in queue.GetConsumingEnumerable())//it will block here automatically waiting from new items to be added and it will not take cpu down 
                {
                    Console.WriteLine(stats.blobCount);
                    if (stats.blobCount > 20) 
                    {
                    }
                }
            }
        }
    }
}
