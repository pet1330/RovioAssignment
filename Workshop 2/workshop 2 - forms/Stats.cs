using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rovio
{
    public class Stats
    {

        #region Red Stats
        public bool RedBlockDetected;
        public double RedBlockDistance;
        public Point RedBlockCenterLocation;
        public int RedBlockHeight;
        public int RedBlockWidth;
        #endregion

        #region Green Stats
        public bool GreenBlockDetected;
        public double GreenBlockDistance;
        public Point GreenBlockCenterLocation;
        public int GreenBlockHeight;
        public int GreenBlockWidth;
        #endregion

        #region BlueLine Stats
        public bool BlueLineDetected;
        public double[] BlueLineDistance;
        public Point[] BlueLineLeftPoint;
        public Point[] BlueLineRightPoint;
        public int[] BlueLineThickness;
        #endregion

        #region Yellow Wall Stats
        public bool YellowWallDetected;
        public double[] YellowWallClosestPointDistance;
        public Point[] YellowWallTopLeftCorner;
        public Point[] YellowWallBottomLeftCorner;
        public Point[] YellowWallTopRightCorner;
        public Point[] YellowWallBottomRightCorner;
        public int[] YellowWallHeight;
        public int[] YellowWallWidth;
        #endregion

        #region White Wall Stats
        public bool WhiteWallDetected;
        public double[] WhiteWallClosestPointDistance;
        public Point[] WhiteWallTopLeftCorner;
        public Point[] WhiteWallBottomLeftCorner;
        public Point[] WhiteWallTopRightCorner;
        public Point[] WhiteWallBottomRightCorner;
        public int[] WhiteWallHeight;
        public int[] WhiteWallWidth;
        #endregion
    }
}
