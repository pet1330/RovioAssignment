using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rovio
{
    class Mapping
    {
        //compass points
        private const int N = 90;
        private const int NE = 135;
        private const int E = 180;
        private const int SE = 225;
        private const int S = 270;
        private const int SW = 315;
        private const int W = 0;
        private const int NW = 45;

        private Bitmap map;
        private Bitmap robotIcon;
        public Point currentLocation;
        public int orientation;

        public Mapping()
        {
            map = global::Rovio.Properties.Resources.SmallMap;
            robotIcon = global::Rovio.Properties.Resources.TinyRobot;
            orientation = SE;
            currentLocation = new Point(50, 50);
        }

        public void Draw()
        {
            Bitmap local = map;
            drawGrid(local);
            this.AddRovioIcon(ref local);
            Program.mainForm.MapViewer.Image = local;
        }

        private void AddRovioIcon(ref Bitmap m)
        {
            Graphics g = Graphics.FromImage(m);

            g.DrawImage(rotateImage(robotIcon, orientation), currentLocation.X, currentLocation.Y);
        }

        private Bitmap rotateImage(Image image, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            //create a new empty bitmap to hold rotated image
            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(rotatedBmp);

            //Put the rotation point in the center of the image
            g.TranslateTransform((float)image.Width / 2, (float)image.Height / 2);

            //rotate the image
            g.RotateTransform(angle);

            //move the image back
            g.TranslateTransform(-(float)image.Width / 2, -(float)image.Height / 2);

            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0, 0));

            return rotatedBmp;
        }

        private void drawGrid(Bitmap m)
        {
            Graphics g = Graphics.FromImage(m);
            int numOfCells = 120;
            int cellSize = 20;
            Pen p = new Pen(Color.Black);

            for (int y = 0; y < numOfCells; ++y)
            {
                g.DrawLine(p, 0, y * cellSize, numOfCells * cellSize, y * cellSize);
            }

            for (int x = 0; x < numOfCells; ++x)
            {
                g.DrawLine(p, x * cellSize, 0, x * cellSize, numOfCells * cellSize);
            }
        }
    }
}