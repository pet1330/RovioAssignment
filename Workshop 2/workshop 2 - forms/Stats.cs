using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rovio
{
    class Stats
    {
        int colour = 0;
        public bool RedBlockDetected = false;
        public float RedBlockDistance;
        public Point RedBlockCenterLocation = new Point(0, 0);
        
        public Stats(int colourFilter)
        {
        
        
        }
    }
}
