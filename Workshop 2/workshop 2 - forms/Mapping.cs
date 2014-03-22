﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using System.Collections.Concurrent;
using System.Threading;

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
        private int cellSize = 10;

        private static double[] mapData = new double[(mapWidth * mapHeight)];
        private static Bitmap robotIcon = global::Rovio.Properties.Resources.TinyRobot;
        
        public double imageWidth = 352;

        public static Point currentLocation;
        public int orientation = 90;
        public static double threshold = 1;

        public static BlockingCollection<Rovio.Stats> queue = new BlockingCollection<Rovio.Stats>(30);

        public void runMap()
        {
            while (true)
            {
                foreach (Stats stats in queue.GetConsumingEnumerable())
                {
                    if (stats.RedBlockDetected)
                    {
                        UpdateRedBlock(stats);
                    }

                    if (stats.GreenBlockDetected)
                    {
                        UpdateGreenBlock(stats);
                    }

                    if (stats.YellowWallDetected)
                    {
                        UpdateYellowWall(stats);
                    }

                    if (stats.WhiteWallDetected)
                    {
                        UpdateWhiteWall(stats);
                    }

                    if (stats.BlueLineDetected)
                    {
                        UpdateBlueLine(stats);
                    }
                    Draw();
                }
            }
        }

        private void UpdateRedBlock(Stats stats)
        {
            if (stats.RedBlockHeight == 0)
                return;

            int h = PXtoCM((double)25.0, (double)stats.RedBlockHeight, (double)stats.RedBlockWidth, (double)stats.RedBlockCenterLocation.X);
            
            int v = (int)(0 - ((25.0 / stats.RedBlockHeight)*100));
            
            double realDist = Math.Sqrt(Math.Pow(v, 2) + Math.Pow(h, 2));

            Point newLocation = RotateLocation(new Point(h,v));

            probabilisticMap((newLocation.X + currentLocation.X), (newLocation.Y + currentLocation.Y), true);

            //DrawRedBlock((newLocation.X + currentLocation.X), (newLocation.Y + currentLocation.Y));
        }

        private void UpdateGreenBlock(Stats stats)
        {
            if (stats.GreenBlockHeight == 0)
                return;

            int h = PXtoCM((double)130.0, (double)stats.GreenBlockHeight, (double)stats.GreenBlockWidth, (double)stats.GreenBlockCenterLocation.X);

            int v = (int)(0 - ((130.0 / stats.GreenBlockHeight) * 100));

            double realDist = Math.Sqrt(Math.Pow(v, 2) + Math.Pow(h, 2));

            Point newLocation = RotateLocation(new Point(h, v));

            probabilisticMap((newLocation.X + currentLocation.X), (newLocation.Y + currentLocation.Y), true);
        }
        
        private void UpdateYellowWall(Stats stats)
        {
        }
        
        private void UpdateWhiteWall(Stats stats)
        {
        }
        
        private void UpdateBlueLine(Stats stats) 
        {
        }

        public Mapping()
        {
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    if (i < 30 || i >= 229)
                    {
                        if (j < 100 || j >= 199)
                        {
                            mapData[((i * mapWidth) + j)] = 1;
                        }
                    }
                    set(i, j, 0.5);
                }
            }

          //  for (int i = 0; i < mapWidth; i++)
          //  {
          //      set(i, 120, 1);
         //   }

            //SET THE CURRENT LOCATION %CL
            currentLocation = new Point(100, 200);
            Draw();
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
            addRovioIcon(map);
            drawMapToScreen(map);
        }

        public static double get(Point toGet)
        {
            return get(toGet.X, toGet.Y);
        }

        public static void set(Point toSet, double input)
        {
            set(toSet.X, toSet.Y, input);
        }

        public static double get(int x, int y)
        {
            if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight)
                return 1;

            if (x < 30 || x >= 229)
            {
                if (y < 100 || y >= 199)
                {
                    return 1;
                }
            }
            return mapData[((x * mapWidth) + y)];
        }

        public static void set(int x, int y, double input)
        {
            if (x < 30 || x >= 230)
            {
                if (y < 100 || y >= 200)
                {
                    return;
                }
            }
            if (input >= 0 && input <= 1)
            {
                mapData[((x * mapWidth) + y)] = input;
            }
        }

        private static double statesProbability(bool world, bool sensor)
        {
            if (world && sensor)
            {
                return 0.55;
            }
            else if (world && !sensor)
            {
                return 0.45;
            }
            else if (!world && sensor)
            {
                return 0.45;
            }
            else //if (!world && !sensor)
            {
                return 0.55;
            }
        }
        
        public static double probabilisticMap(int x,int y,bool sensor)
        {
            double  mapProb = get(x,y);
            bool world = (mapProb < threshold);

            double newProb = (statesProbability(world, sensor) * mapProb) /((statesProbability(world, sensor) * mapProb) + (statesProbability(!world, sensor) * ((1 - mapProb))));

           if ((newProb > 0.05) && (newProb < 0.95))
            {
                set(x, y, newProb);
            }
            return newProb;
        }

        private void annotate(Bitmap m)
        {
            Graphics g = Graphics.FromImage(m);
            Pen p = new Pen(Color.White);
            
            for (int x = 0; x <= mapWidth; ++x)
            {
                for (int y = 0; y <= mapHeight; ++y)
                {
                    int value = Convert.ToInt32(255 - ((get(x, y) * 255)));

                    p.Color = Color.FromArgb(value, value, value);
                    g.DrawRectangle(p, x, y, 1, 1);
                }
            }
        }

        private double sin(double angle)
        {
            return Math.Sin((angle * (Math.PI / 180)));
        }

        private double cos(double angle)
        {
            return Math.Cos((angle * (Math.PI / 180)));
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

        private Point RotateLocation(Point old)
        {
            double rotatedX = (old.X * cos(360 - orientation)) + (old.Y * sin(360 - orientation));
            double rotatedY = (-old.X * sin(360 - orientation)) + old.Y * cos(360 - orientation);
            Point toReturn = new Point(Convert.ToInt32(rotatedX), Convert.ToInt32(rotatedY));
            return toReturn;
        }

        private int PXtoCM(double heightAtOneMetre, double blocksHeight, double blockWidth, double blockXLocation)
        {
            double dist = ((heightAtOneMetre / blocksHeight) * 100);
            
            double row = (((blockWidth / 2.0) + (blockXLocation)) / ((imageWidth) / ((dist * 0.92))));

            if (((blockWidth / 2.0) + (blockXLocation)) <= (imageWidth / 2))
            {
                row = (0 - (((dist * 0.92) / 2.0) - row));
            }
            else
            {
                row = (0 + (((dist * 0.92) / 2.0) - ((dist * 0.92) - row)));
            }
            return (int)row;
        }

        private Point calculateGreenBlockLocation(float blockHightAtOneMeter, double blocksCurrentHeight, double blockWidth, double blockXLocation)
        {
            double dist = 1.0f;
            if (blocksCurrentHeight != 0)
            {
                dist = ((blockHightAtOneMeter / blocksCurrentHeight) * 100);
            }

            double a = ((dist * 0.92));
            a = ((imageWidth) / a);
            a = (((blockWidth / 2.0) + (blockXLocation)) / a);
            double row = 0;
            
            if (((blockWidth / 2.0) + (blockXLocation)) <= (imageWidth / 2))
            {
                row = (currentLocation.X - (((dist * 0.92) / 2.0) - a));
            }
            else
            {
                row = (currentLocation.X + (((dist * 0.92) / 2.0) - ((dist * 0.92) - a)));
            }

            double realDist = Math.Sqrt(Math.Pow(dist, 2) + Math.Pow(a, 2));

            double col = (currentLocation.Y - dist);
            if (double.IsNaN(row) || double.IsNaN(col))
            {
                return new Point(-1, -1);
            }
            else
            {
                return new Point((int)row, (int)col);
            }
        }
     
        /*
        private void drawRedBlock(Bitmap m)
        {
            Graphics g = Graphics.FromImage(m);
            Pen p = new Pen(Color.Red);

            Point poi = calculateRedBlockLocation();
            g.FillRectangle(new SolidBrush(Color.Red), (float)poi.X, (float)poi.Y, 7, 4);

            for (int i = -3; i < 4; i++)
            {
                for (int j = -2; j < 2; j++)
                {
                    probabilisticMap(Convert.ToInt32((poi.X + i)), Convert.ToInt32((poi.Y + j)), true);
                }
            }
        }
        
        private void drawGreenBlock(Bitmap m)
        {
            Graphics g = Graphics.FromImage(m);
            Pen p = new Pen(Color.Red);
            double dist = 1.0f;

            if (blocksCurrentHeight != 0)
            {
                dist = ((130.0f / blocksCurrentHeight) * 100f);
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

            g.FillRectangle(new SolidBrush(Color.Green),(float)row, (float)col, 35, 35);

        }
        */
        
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
    }
}