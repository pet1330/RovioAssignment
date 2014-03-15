using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rovio
{
    class Stats
    {
        //RED BLOCK
        //---------------------------------------------------------------
        int colour = 0;
        public bool RedBlockDetected = false;
        public float RedBlockDistance;
        public Point RedBlockCenterLocation = new Point(0, 0);
        public int RedBlockHeight = 0;
        public int RedBlockWidth = 0;

        public bool GreenBlockDetected = false;
        public float GreenBlockDistance;
        public Point GreenBlockCenterLocation = new Point(0, 0);
        public int GreenBlockHeight = 0;
        public int GreenBlockWidth = 0;

        public Stats(int colourFilter)
        {
            colour = colourFilter;
        }
    }
}
