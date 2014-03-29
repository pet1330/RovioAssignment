using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rovio
{
    class fieldMap
    {

        private static readonly object aLock = new object();
        private static List<int> a = new List<int>();
        static int Max = int.MaxValue;
       
        public static int getList()
        {
            lock (aLock)
            {
                return a.Count;
            }
        }

        public static void setList(int toAdd)
        {
            lock (aLock)
            {
                if (a.Count > Max)
                { }
                else
                {
                    a.Add(toAdd);
                }
            }
        }


        public static void setMax(int maxSize)
        {
            Max = maxSize;
        }
    }
}
