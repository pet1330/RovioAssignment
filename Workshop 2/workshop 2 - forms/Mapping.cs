using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace Rovio
{
    public class Mapping
    {
        private const int E = 90;
        private const int SE = 135;
        private const int S = 180;
        private const int SW = 225;
        private const int W = 270;
        private const int NW = 315;
        private const int N = 0;
        private const int NE = 45;

        private static int mapWidth = 260;
        private static int mapHeight = 300;
        int cellSize = 10;
        private static double[] mapData = new double[(mapWidth * mapHeight)];

        private static Bitmap robotIcon = global::Rovio.Properties.Resources.TinyRobot;

        public double blockWidth;
        public double blockHeightAtOnemeter;
        public double blocksCurrentHeight;
        public double distanceToWidthSightPathRatio;
        public double imageWidth;
        public double blockXLocation;

        public static Point currentLocation;
        public int orientation;

        public Mapping()
        {
            currentLocation = new Point(100, 100);
            orientation = N;
        }

        public void Draw()
        {
            //Create Blank map
            Bitmap map = new Bitmap(260, 300);
            Graphics g = Graphics.FromImage(map);
            SolidBrush brush = new SolidBrush(Color.White);
            g.FillRectangle(brush, 0, 0, 260, 300);
            annotate(map);
            drawGrid(map);
            drawRedBlock(map);
            shapeMap(map);
            this.addRovioIcon(map);
            drawMapToScreen(map);
        }

        public void updateMap()
        { 
        
        }

        private void shapeMap(Bitmap m) 
        {
            Graphics g = Graphics.FromImage(m);
            SolidBrush brush = new SolidBrush(Color.Black);
            g.FillRectangle(brush, 0, 0, 30, 100);
            g.FillRectangle(brush, m.Width - 30, 0, 30, 100);
            g.FillRectangle(brush, 0, m.Height - 100, 30, 100);
            g.FillRectangle(brush, m.Width - 30, m.Height - 100, 30, 100);
        }

        private void annotate(Bitmap m)
        {

            Graphics g = Graphics.FromImage(m);
            Pen p = new Pen(Color.Red);


            for (int x = 0; x <= mapWidth; ++x)
            {
                for (int y = 0; y <= mapHeight; ++y)
                {
                    int value = Convert.ToInt32(255- ((get(x, y) / 255)));
                    p.Color = Color.FromArgb(value, value, value);
                    g.DrawRectangle(p, x, y, 1, 1);
                }
            }
        }

        private double tan(double angle)
        {
            return Math.Tan((angle * (Math.PI / 180)));
        }

        private double sin(double angle)
        {
            return Math.Sin((angle * (Math.PI / 180)));
        }

        private double cos(double angle)
        {
            return Math.Cos((angle * (Math.PI / 180)));
        }

        private Point RotateLocation(Point old)
        {
            double rotatedX = (old.X * cos(orientation)) + (old.Y * sin(orientation));
            double rotatedY = (-old.X * sin(orientation)) + old.Y * cos(orientation);
            Point toReturn = new Point(Convert.ToInt32(rotatedX), Convert.ToInt32(rotatedY));
            return toReturn;
        }

        private void addRovioIcon(Bitmap m)
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
            Graphics g = Graphics.FromImage(m);
            Pen p = new Pen(Color.Black);
            for (int y = 0; y <= (mapHeight / cellSize); ++y)
            {
                g.DrawLine(p, 0, y * cellSize, (mapWidth / cellSize) * cellSize, y * cellSize);
            }

            for (int x = 0; x <= (mapWidth / cellSize); ++x)
            {
                g.DrawLine(p, x * cellSize, 0, x * cellSize, (mapHeight / cellSize) * cellSize);
            }
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


            double realDist = Math.Sqrt(Math.Pow(dist, 2) + Math.Pow(a, 2));

            double col = (currentLocation.Y - realDist);


            g.FillRectangle(new SolidBrush(Color.Red), (float)row, (float)col, 10, 10);




        }

        private void drawGreenBlocks(Bitmap m, Point block)
        {
            Graphics g = Graphics.FromImage(m);
            Pen p = new Pen(Color.Green);

        }

        private void drawMapToScreen(System.Drawing.Image image)
        {

            if (Program.mainForm.InvokeRequired)
            {
                Program.mainForm.Invoke(new System.Windows.Forms.MethodInvoker(delegate { drawMapToScreen(image); }));
            }
            else
            {
                Program.mainForm.MapViewer.Image = image;
            }
        }

        private delegate void mapImageReady(System.Drawing.Image image);

        public double get(Point toGet)
        {
            return get(toGet.X, toGet.Y);
        }

        public void set(Point toSet, double input)
        {
            set(toSet.X, toSet.Y, input);
        }

        public double get(int x, int y)
        {
            return mapData[((x * mapWidth) + y)];
        }

        public void set(int x, int y, double input)
        {
            mapData[((x * mapWidth) + y)] = input;
        }

    }
}