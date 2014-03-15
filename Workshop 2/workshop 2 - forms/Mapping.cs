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

            public float blockWidth;
            public float blockHeightAtOnemeter; 
            public float blocksCurrentHeight; 
            public float distanceToWidthSightPathRatio; 
            public float imageWidth;
            public float blockXLocation; 


        //========================


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
        public Point RedBlockLocation;

        //private Point[] GreenBlockLocation;
        public Point currentLocation;
        public int orientation;

        public Mapping() {}

        public void Draw()
        {
            Bitmap map = new Bitmap(260, 300);
            Graphics gfx = Graphics.FromImage(map);
            SolidBrush brush = new SolidBrush(Color.White);
            gfx.FillRectangle(brush, 0, 0, 260, 300);
            drawGrid(map);
            drawRedBlock(map);
            this.AddRovioIcon(map);
            UpdateMap(map);
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

        private void drawRedBlock(Bitmap m)
        {
            Graphics g = Graphics.FromImage(m);
            Pen p = new Pen(Color.Red);
            float answer = (((blockWidth / 2) + blockXLocation) / ((imageWidth/2) / ((((blockHeightAtOnemeter / blocksCurrentHeight) * 100) * distanceToWidthSightPathRatio) / 2)));
 
            g.DrawString(answer.ToString(), new System.Drawing.Font("Arial", 8), Brushes.Black, 10.0f, 10.0f, new System.Drawing.StringFormat());

             g.FillRectangle(new SolidBrush(Color.Green), ((currentLocation.X/2) + answer), ((300-((25.0f / blocksCurrentHeight)*100))), 10, 10);
             g.FillRectangle(new SolidBrush(Color.Red), RedBlockLocation.X,RedBlockLocation.Y,10,10);
        }

        private void drawGreenBlocks(Bitmap m, Point block)
        {

            Graphics g = Graphics.FromImage(m);
            Pen p = new Pen(Color.Green);
           // g.FillRectangles(
        }

        public void UpdateMap(System.Drawing.Image image)
        {

            if (Program.mainForm.InvokeRequired)
            {
                Program.mainForm.Invoke(new System.Windows.Forms.MethodInvoker(delegate { UpdateMap(image); }));
            }
            else
            {
                Program.mainForm.MapViewer.Image = image;
            }
        }

        public delegate void mapImageReady(System.Drawing.Image image);

        public event mapImageReady mapImage;

    }
}