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
        private int cellSize = 10;

        private static double[] mapData = new double[(mapWidth * mapHeight)];
        private static Bitmap robotIcon = global::Rovio.Properties.Resources.TinyRobot;
        AStar pathFinding = new AStar();

        public double blockWidth;
        public double blockHeightAtOnemeter;
        public double blocksCurrentHeight;
        public double distanceToWidthSightPathRatio;
        public double imageWidth;
        public double blockXLocation;

        public static Point currentLocation;
        public int orientation;
        public static double threshold = 1;

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

            for (int i = 0; i < mapWidth; i++)
            {
                set(i, 120, 1);
            }


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
            //drawGrid(map);
            drawRedBlock(map);
            //drawGreenBlock(map);  //DOES NOT WORK YET
            addRovioIcon(map);
            drawMapToScreen(map);
        }

        public void statsInfoUpdate()
        {

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
            if (double.IsNaN(row) || double.IsNaN(col))
                return;

            g.FillRectangle(new SolidBrush(Color.Red), (float)row, (float)col, 7, 4);

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    probabilisticMap(Convert.ToInt32((row + i)), Convert.ToInt32((col + j)), true);
                }
            }


            pathFinding.FindPath(currentLocation, new Point((int)row, (int)col));
            foreach (Point loop in pathFinding.finalPath)
            {
                set(loop.X, loop.Y, 1);
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


        /*
        public List<PathElement> AStar(Point Pstart, Point Pend)
        {
            PathElement start = new PathElement(Pstart.X, Pstart.Y);
            PathElement end = new PathElement(Pend.X, Pend.Y);

            List<PathElement> openList = new List<PathElement>();
            List<PathElement> closeList = new List<PathElement>();
            
            openList.Add(start);

            while (true)
            {
                int tempF = int.MaxValue;
                //Sort by lowest F cost
                openList = openList.OrderBy(o => o.F()).ToList();

                foreach (PathElement item in openList)
                {
                    if (item.F() < tempF)
                    {
                        tempF = item.F();
                    }
                }
               
                if (tempF == int.MaxValue)
                    return null;
                //select first element
                PathElement selected = openList.First();
                closeList.Add(selected);
                openList.Remove(selected);

                //check we've not found the target
                if (selected.xPos == end.xPos && selected.yPos == end.yPos)
                {
                    List<PathElement> finalList = new List<PathElement>();
                    finalList.Add(end);
                    while (selected.parent != null)
                    {
                        finalList.Add(selected);
                        selected = selected.parent;
                    }
                
                    finalList.Add(start);
                    finalList.Reverse();
                    return finalList;
                }


                for (int ii = -1; ii <= 1; ii++)
                {
                    for (int jj = -1; jj <= 1; jj++)
                    {
                        //sets which element we're looking at
                        int currentX = selected.xPos + ii;
                        int currentY = selected.yPos + jj;

                        //check we're still within the board
                        if (currentX < 0)
                        {
                            continue;
                        }
                        if (currentX > mapWidth)
                        {
                            continue;
                        }
                        if (currentY < 0)
                        {
                            continue;
                        }
                        if (currentY > mapHeight)
                        {
                            continue;
                        }

                        //ignore walls
                        if (get(currentY, currentX) > 0.6)
                        {
                            continue;
                        }

                        int xDiff = Math.Abs(currentX - (selected.xPos));
                        int yDiff = Math.Abs(currentY - (selected.yPos));

                        int direction = 0;

                        if (xDiff == 1 && yDiff == 1)
                        {
                            direction = 14;
                        }
                        else
                        {
                            direction = 10;
                        }

                        //find Hueristic 
                        int xh = Math.Abs(currentX - end.xPos);
                        int yh = Math.Abs(currentY - end.yPos);
                        int H = 10 * (xh + yh);
                        int G = selected.G + direction;
                        bool onOpenList = false;

                        foreach (PathElement it in openList)
                        {
                            if (currentX == it.xPos && currentY == it.yPos)
                            {
                                onOpenList = true;
                                //check if parent needs to change
                                if (selected.G + direction < it.G)
                                {
                                    //change parent
                                    it.parent = selected;
                                    //change G to new G
                                    it.G = selected.G + direction;
                                    break;
                                }
                            }
                        }
                        if (!onOpenList)
                        {
                            bool closed = false;
                            //check if element is on closed list
                            foreach (PathElement it in closeList)
                            {
                                if (currentX == it.xPos && currentY == it.yPos)
                                {
                                    closed = true;
                                }
                            }
                            //if on closed list
                            if (closed)
                                //ignore it
                                continue;
                            else
                                //add element to openlist
                                openList.Add(new PathElement(currentX, currentY, G, H, selected));
                        }
                    }
                }
            }
            return null;
        }


        private int AStarHeuristics(Point start, Point end) 
        {
            int xh = Math.Abs(start.X - end.X);
            int yh = Math.Abs(start.Y - end.Y);
            return (10 * (xh + yh));
        }
        */

    }
}