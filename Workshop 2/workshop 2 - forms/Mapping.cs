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

        public double blockWidth;
        public double blockHeightAtOnemeter;
        public double blocksCurrentHeight;
        public double distanceToWidthSightPathRatio;
        public double imageWidth;
        public double blockXLocation;

        private Bitmap robotIcon = global::Rovio.Properties.Resources.TinyRobot;

        public Point currentLocation;
        public int orientation;

        public Mapping()
        {
        }

        public double sin(double angle)
        {
            return Math.Sin((angle * (Math.PI / 180)));
        }

        public double cos(double angle)
        {
            return Math.Cos((angle * (Math.PI / 180)));
        }

        public void Draw()
        {
            Bitmap map = new Bitmap(260, 300);
            Graphics gfx = Graphics.FromImage(map);
            SolidBrush brush = new SolidBrush(Color.White);
            gfx.FillRectangle(brush, 0, 0, 260, 300);
            drawGrid(map);
            drawRedBlock(map);
            this.AddRovioIcon(map);
            DrawMap(map);
        }

        private Point RotateLocation(Point old)
        {
            double rotatedX = (old.X * cos(orientation)) + (old.Y * sin(orientation));
            double rotatedY = (-old.X * sin(orientation)) + old.Y * cos(orientation);
            Point toReturn = new Point(Convert.ToInt32(rotatedX), Convert.ToInt32(rotatedY));
            return toReturn;
        }

        private void AddRovioIcon(Bitmap m)
        {
            Graphics g = Graphics.FromImage(m);
            int x = (currentLocation.X - ((robotIcon.Width) / 2));
            int y = (currentLocation.Y - ((robotIcon.Height) / 2));
            g.DrawImage(rotateImage(robotIcon, orientation), x, y);
        }

        private Bitmap rotateImage(Image image, double angle)
        {
            //create a new empty bitmap to hold rotated image
            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(rotatedBmp);

            //Put the rotation point in the center of the image
            g.TranslateTransform((float)image.Width / 2, (float)image.Height / 2);

            //rotate the image
            g.RotateTransform((float)angle);

            //move the image back
            g.TranslateTransform(-(float)image.Width / 2, -(float)image.Height / 2);

            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0, 0));

            return rotatedBmp;
        }

        private void drawGrid(Bitmap m)
        {
            int numOfCellsWidth = 26;
            int numOfCellsHeight = 30;
            int cellSize = 10;

            Graphics g = Graphics.FromImage(m);
            Pen p = new Pen(Color.Black);
            for (int y = 0; y <= numOfCellsHeight; ++y)
            {
                g.DrawLine(p, 0, y * cellSize, numOfCellsWidth * cellSize, y * cellSize);
            }

            for (int x = 0; x <= numOfCellsWidth; ++x)
            {
                g.DrawLine(p, x * cellSize, 0, x * cellSize, numOfCellsHeight * cellSize);

            }

            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, 30, 100);
            g.FillRectangle(new SolidBrush(Color.Black), m.Width - 30, 0, 30, 100);
            g.FillRectangle(new SolidBrush(Color.Black), 0, m.Height - 100, 30, 100);
            g.FillRectangle(new SolidBrush(Color.Black), m.Width - 30, m.Height - 100, 30, 100);

        }

        private void drawRedBlock(Bitmap m)
        {
            Graphics g = Graphics.FromImage(m);
            Pen p = new Pen(Color.Red);
            double dist = 1.0f;

            if (blocksCurrentHeight != 0)
            {
                dist = ((25.0f / blocksCurrentHeight) * 100f);
            }

            double a = ((dist * 0.92));
            a = ((imageWidth) / a);
            a = (((blockWidth / 2.0) + (blockXLocation)) / a);

            double row = 0;

            if (((blockWidth / 2.0f) + (blockXLocation)) <= (imageWidth / 2))
            {
                row = (currentLocation.X - (((dist * 0.92) / 2.0) - a));
            }
            else
            {
                row = (currentLocation.X + (((dist * 0.92) / 2.0) - ((dist * 0.92) - a)));
            }

            double col = (currentLocation.Y - dist);
            g.FillRectangle(new SolidBrush(Color.Red), (float)row, (float)col, 10, 10);
        }

        private void drawGreenBlocks(Bitmap m, Point block)
        {
            Graphics g = Graphics.FromImage(m);
            Pen p = new Pen(Color.Green);

        }

        private void DrawMap(System.Drawing.Image image)
        {

            if (Program.mainForm.InvokeRequired)
            {
                Program.mainForm.Invoke(new System.Windows.Forms.MethodInvoker(delegate { DrawMap(image); }));
            }
            else
            {
                Program.mainForm.MapViewer.Image = image;
            }
        }

        private delegate void mapImageReady(System.Drawing.Image image);
    }
}