using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

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

        private Bitmap robotIcon = global::Rovio.Properties.Resources.TinyRobot;
        private const int numOfCellsWidth = 26;
        private const int numOfCellsHeight = 30;
        private const int cellSize = 10;
        public Point currentLocation;
        public int orientation;

        public Mapping()
        {
            orientation = SE;
            currentLocation = new Point(50, 50);
        }

        public void Draw()
        {
           Bitmap map = new Bitmap(260, 300);
            using (Graphics gfx = Graphics.FromImage(map))
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                gfx.FillRectangle(brush, 0, 0, 260, 300);
            }
            drawGrid(map);
            this.AddRovioIcon(map);
            mapImage(map);
        }

        private void AddRovioIcon(Bitmap m)
        {
            Graphics g = Graphics.FromImage(m);
            g.DrawImage(rotateImage(robotIcon, orientation), currentLocation.X, currentLocation.Y);
        }

        private Bitmap rotateImage(Image image, float angle)
        {
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
            Pen p = new Pen(Color.Black);

            for (int y = 0; y < numOfCellsHeight; ++y)
            {
                g.DrawLine(p, 0, y * cellSize, numOfCellsWidth * cellSize, y * cellSize);
            }

            for (int x = 0; x < numOfCellsWidth; ++x)
            {
                g.DrawLine(p, x * cellSize, 0, x * cellSize, numOfCellsHeight * cellSize);

            }
        }

        public void UpdateMap(System.Drawing.Image image)
        {
            if (Program.mainForm.InvokeRequired)
            {
                Program.mainForm.Invoke(new System.Windows.Forms.MethodInvoker(delegate { UpdateMap(image); }));
                Program.mainForm.VideoViewer.Image = image;
            }
        }

        public delegate void mapImageReady(System.Drawing.Image image);
        
        public event mapImageReady mapImage;
   
    }
}