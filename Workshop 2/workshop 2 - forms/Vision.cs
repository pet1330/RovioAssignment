using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AForge.Imaging.Filters;
using AForge;
using AForge.Imaging;

namespace Rovio
{
    class Vision
    {
        public const int RED = 0;
        public const int GREEN = 1;
        public const int WHITE = 2;
        public const int YELLOW = 3;
        public const int BLUE = 4;

        //Image adapted from http://www.google.com/images/errors/robot.png   This images was created and belongs to Google
        protected Bitmap ConnectionLost = global::Rovio.Properties.Resources.ConnectionLost;
        protected System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 8);
        protected float x = 10.0F;
        protected float y = 10.0F;
        protected System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
        private bool run = true;

        public Vision() { }

        public void runVision()
        {
            while (run)
            {
                if (checkConnection())
                {
                    Bitmap RGBImage = getImage();
                    ExtractFeatrures(colourFilter(RGBImage));
                    // UpdateVideo(RGBImage);
                    if (!run) return;
                }
                else
                {
                    //Give the thread some time to connect in case it is the first connection
                    System.Threading.Thread.Sleep(100);
                    if (!checkConnection())
                    {
                        UpdateVideo(ConnectionLost);
                    }
                }
            }
        }

        private Bitmap[] FilteredImage;

        protected Bitmap[] colourFilter(Bitmap image)
        {
            HSLFiltering filt = new HSLFiltering();
            filt.FillOutsideRange = true;
            FilteredImage = new Bitmap[5];
            for (int i = RED; i <= BLUE; i++)
            {
                switch (i)
                {
                    case RED:
                        filt.Hue = new IntRange(345, 15);
                        filt.Saturation = new Range(0.4f, 1.0f);
                        filt.Luminance = new Range(0.15f, 1.0f);
                        break;
                    case GREEN:
                        filt.Hue = new IntRange(90, 115);
                        filt.Saturation = new Range(0.3f, 1.0f);
                        filt.Luminance = new Range(0.15f, 0.9f);
                        break;
                    case WHITE:
                        filt.Hue = new IntRange(181, 180);
                        filt.Saturation = new Range(0.0f, 0.7f);
                        filt.Luminance = new Range(0.52f, 0.8f);
                        //61
                        break;
                    case YELLOW:
                        filt.Hue = new IntRange(25, 80);
                        filt.Saturation = new Range(0.2f, 0.9f);
                        filt.Luminance = new Range(0.25f, 0.8f);
                        break;
                    case BLUE:
                        filt.Hue = new IntRange(180, 240);
                        filt.Saturation = new Range(0.1f, 1.0f);
                        filt.Luminance = new Range(0.1f, 0.7f);
                        break;
                    default /* Red */:
                        filt.Hue = new IntRange(0, 359);
                        filt.Saturation = new Range(0.0f, 1.0f);
                        filt.Luminance = new Range(0.0f, 1.0f);
                        break;
                }
                FilteredImage[i] = filt.Apply(image);
            }
            return FilteredImage;
        }

        protected void ExtractFeatrures(Bitmap[] filtered)
        {
           // UpdateVideo(ExtractRedFeatures(filtered[RED]));
            // ExtractGreenFeatures(filtered[GREEN]);
            //UpdateVideo(ExtractYellowFeatures(filtered[YELLOW]));
            ExtractYellowFeatures(filtered[YELLOW]);
            UpdateVideo(ExtractBlueFeatures(filtered[BLUE]));
            //Bitmap b = ExtractWhiteFeatures(filtered[WHITE], );
            //Merge mer = new Merge(a);
            //UpdateVideo(mer.Apply(b));

        }

        private Bitmap ExtractRedFeatures(Bitmap Filtered)
        {
            BlobCounter bc = new BlobCounter();
            Stats redStats = new Stats();
            bc.MinWidth = 5;
            bc.MinHeight = 5;
            bc.FilterBlobs = true;
            bc.ObjectsOrder = ObjectsOrder.Size;
            bc.ProcessImage(Filtered);
            Rectangle[] rects = bc.GetObjectsRectangles();
            Rectangle biggest = new Rectangle(0, 0, 0, 0);
            Graphics g = Graphics.FromImage(Filtered);

            if ((rects.Length > 0) && (rects[0].Height > 0))
            {
                biggest = rects[0];
            }

            redStats.RedBlockDetected = true;
            redStats.RedBlockCenterLocation = new System.Drawing.Point(((((biggest.Width / 2) + biggest.X))), (biggest.Y + biggest.Height / 2));
            redStats.RedBlockHeight = biggest.Height;
            redStats.RedBlockWidth = biggest.Width;
            redStats.RedBlockDistance = (25.0f / biggest.Height);
            Mapping.queue.Add(redStats);

            string objectString = Math.Round((25.0f / biggest.Height), 2).ToString();
            string drawString = biggest.Height + " <-- Height    Width --> " + biggest.Width + "\n Image Center = " + (redStats.RedBlockCenterLocation.X/* - (Filtered.Width / 2)*/);
            g.DrawRectangle(new Pen(Color.Blue), biggest);
            g.DrawString(objectString, drawFont, Brushes.White, redStats.RedBlockCenterLocation.X, redStats.RedBlockCenterLocation.Y, drawFormat);
            g.DrawString(drawString, drawFont, Brushes.White, x, y, drawFormat);

            return Filtered;
        }

        private Bitmap ExtractGreenFeatures(Bitmap Filtered)
        {
            BlobCounter bc = new BlobCounter();
            Stats greenStats = new Stats();
            bc.MinWidth = 15;
            bc.MinHeight = 15;
            bc.FilterBlobs = true;
            bc.ObjectsOrder = ObjectsOrder.Size;
            bc.ProcessImage(Filtered);
            Rectangle[] rects = bc.GetObjectsRectangles();
            Rectangle biggest = new Rectangle(0, 0, 0, 0);
            Graphics g = Graphics.FromImage(Filtered);

            if ((rects.Length > 0) && (rects[0].Height > 0))
            {
                biggest = rects[0];
            }

            greenStats.GreenBlockDetected = true;
            greenStats.GreenBlockCenterLocation = new System.Drawing.Point(((((biggest.Width / 2) + biggest.X))), (biggest.Y + biggest.Height / 2));
            greenStats.GreenBlockHeight = biggest.Height;
            greenStats.GreenBlockWidth = biggest.Width;
            greenStats.GreenBlockDistance = (130.0f / biggest.Height);
            Mapping.queue.Add(greenStats);

#if DEBUG
            // User Feedback for debug
            string objectString = Math.Round((130.0f / biggest.Height), 2).ToString();
            string drawString = biggest.Height + " <-- Height    Width --> " + biggest.Width + "\n Image Center = " + (greenStats.RedBlockCenterLocation.X);
            g.DrawRectangle(new Pen(Color.Blue), biggest);
            g.DrawString(objectString, drawFont, Brushes.White, greenStats.RedBlockCenterLocation.X, greenStats.RedBlockCenterLocation.Y, drawFormat);
            g.DrawString(drawString, drawFont, Brushes.White, x, y, drawFormat);
#endif
            return Filtered;
        }

        
     private Bitmap ExtractBlueFeatures(Bitmap Filtered)
     {
         BlobCounter bc = new BlobCounter();
         Stats redStats = new Stats();
         bc.MinWidth = 100;
         //bc.MaxHeight = 40;
         bc.FilterBlobs = true;
         bc.ObjectsOrder = ObjectsOrder.Size;
         bc.ProcessImage(Filtered);
         Rectangle[] rects = bc.GetObjectsRectangles();
         Rectangle biggest = new Rectangle(0, 0, 0, 0);
         Graphics g = Graphics.FromImage(Filtered);

         if ((rects.Length > 0) && (rects[0].Height > 0))
         {
             biggest = rects[0];
         }

         redStats.BlueLineDetected = true;
         //redStats.BlueLineAverageThickness = (11.0f / biggest.Height);
         //Console.WriteLine(redStats.BlueLineAverageThickness);
         if(biggest.Height>0)
         Mapping.queue.Add(redStats);

         string objectString = Math.Round((12.0f / biggest.Height), 2).ToString();
         string drawString = biggest.Height + " <-- Height  |  Width --> " + biggest.Width + "\n Image Center = " + (redStats.RedBlockCenterLocation.X);
         g.DrawRectangle(new Pen(Color.Blue), biggest);
         g.DrawString(objectString, drawFont, Brushes.White, redStats.RedBlockCenterLocation.X, redStats.RedBlockCenterLocation.Y, drawFormat);
         g.DrawString(drawString, drawFont, Brushes.White, x, y, drawFormat);

         Console.WriteLine(biggest.Height);

         if (biggest.Height > 0)
         {
             Stats a = new Stats();
             a.BlueLineDetected = true;
             a.BlueLinePerpendicularDistance = (11.0/biggest.Height);
             Mapping.queue.Add(a);
         }


         return Filtered;
     }
        

        /*
        private Bitmap ExtractBlueFeatures(Bitmap Filtered)
        {
            BlobCounter bc = new BlobCounter();
            Graphics g = Graphics.FromImage(Filtered);
            Pen bluePen = new Pen(Color.Red, 1);
            bc.MinWidth = 150;
            bc.MaxHeight = 40;
            bc.FilterBlobs = true;
            bc.ObjectsOrder = ObjectsOrder.Size;
            bc.ProcessImage(Filtered);
            List<IntPoint> topEdge;
            List<IntPoint> bottomEdge;

            Blob[] blob = bc.GetObjectsInformation();

            if (blob.Length > 0)
            {
                bc.GetBlobsTopAndBottomEdges(blob[0], out topEdge, out bottomEdge);
                g.DrawLine(bluePen, (float)topEdge[0].X, (float)topEdge[0].Y, (float)topEdge[topEdge.Count - 1].X, (float)topEdge[topEdge.Count - 1].Y);
                g.DrawLine(bluePen, (float)bottomEdge[0].X, (float)bottomEdge[0].Y, (float)bottomEdge[bottomEdge.Count - 1].X, (float)bottomEdge[bottomEdge.Count - 1].Y);

                List<int> topint = new List<int>(topEdge.Count);
                List<int> bottomint = new List<int>(bottomEdge.Count);

                double smallestListNumber = (topEdge.Count <= bottomEdge.Count) ? topEdge.Count : bottomEdge.Count;
                int[] average = new int[Convert.ToInt32(smallestListNumber)];

                for (int i = 0; i < smallestListNumber; i++)
                {
                    bottomint.Add(bottomEdge[i].Y);
                    topint.Add(topEdge[i].Y);
                    average[i] = Math.Abs((bottomEdge[i].Y) - (topEdge[i].Y));
                }

                double CurrentAverageHeight = (average.Sum() / smallestListNumber);
                double dist = (6.0 / CurrentAverageHeight);
                double thickestWall = average.Max();
                Console.WriteLine("Top = {0}     Bottom = {1}     Height = {2}     Dist = {3}    ", Convert.ToInt32(topint.Average()), Convert.ToInt32(bottomint.Average()), CurrentAverageHeight, dist);

                Stats blueLineStats = new Stats();
                blueLineStats.BlueLineDetected = true;
                blueLineStats.BlueLineStraightDistance = dist;
                blueLineStats.BlueLineAverageThickness = CurrentAverageHeight;
                blueLineStats.BlueLineAverageBottom = bottomint.Average();
                blueLineStats.BlueLineAverageTop = topint.Average();
                blueLineStats.BlueLineLength = smallestListNumber;
                blueLineStats.BlueLineMaxThickness = thickestWall;
                blueLineStats.BlueLinePerpendicularDistance = (11.0 / thickestWall);
                Console.WriteLine(blueLineStats.BlueLinePerpendicularDistance);
                Mapping.queue.Add(blueLineStats);
                return Filtered;
            }
            return Filtered;
        }
        */
        
        /*
        private Bitmap ExtractWhiteFeatures(Bitmap Filtered, Stats blueLine)
        {
            BlobCounter bc = new BlobCounter();
            Graphics g = Graphics.FromImage(Filtered);
            Pen bluePen = new Pen(Color.Yellow, 1);
            bc.MinWidth = 100;
            bc.MinHeight = 30;
            bc.FilterBlobs = true;
            bc.ObjectsOrder = ObjectsOrder.Size;
            bc.ProcessImage(Filtered);
            List<IntPoint> topEdge;
            List<IntPoint> bottomEdge;

            Blob[] blob = bc.GetObjectsInformation();

            if (blob.Length > 0)
            {
                bc.GetBlobsTopAndBottomEdges(blob[0], out topEdge, out bottomEdge);
                g.DrawLine(bluePen, (float)topEdge[0].X, (float)topEdge[0].Y, (float)topEdge[topEdge.Count - 1].X, (float)topEdge[topEdge.Count - 1].Y);
                g.DrawLine(bluePen, (float)bottomEdge[0].X, (float)bottomEdge[0].Y, (float)bottomEdge[bottomEdge.Count - 1].X, (float)bottomEdge[bottomEdge.Count - 1].Y);

                List<int> topint = new List<int>(topEdge.Count);
                List<int> bottomint = new List<int>(bottomEdge.Count);

                double smallestListNumber = (topEdge.Count <= bottomEdge.Count) ? topEdge.Count : bottomEdge.Count;
                int[] average = new int[Convert.ToInt32(smallestListNumber)];

                for (int i = 0; i < smallestListNumber; i++)
                {
                    bottomint.Add(bottomEdge[i].Y);
                    topint.Add(topEdge[i].Y);
                    average[i] = Math.Abs((bottomEdge[i].Y) - (topEdge[i].Y));
                }

                double CurrentAverageHeight = (average.Sum() / smallestListNumber);
                double dist = (6.0 / CurrentAverageHeight);
                double thickestWall = average.Max();
                // Console.WriteLine("Wall= {0,20}             BlueLine= {1,20}", CurrentAverageHeight, blueLine.BlueLineAverageThickness);
                whiteList.Add(CurrentAverageHeight);
                blueList.Add(blueLine.BlueLineAverageThickness);
                Console.WriteLine("Wall= {0,20}             BlueLine= {1,20}", whiteList.Average(), blueList.Average());


                return Filtered;
            }
            else
            {
                Stats toReturn = new Stats();
                toReturn.BlueLineDetected = false;
                return Filtered;
            }
        }
        */
        /*
        private Bitmap ExtractWhiteFeatures(Bitmap Filtered)
        {
            BlobCounter bc = new BlobCounter();
            Stats toReturn = new Stats();
            bc.MinWidth = 5;
            bc.MinHeight = 25;
            bc.MaxHeight = 40;
            bc.FilterBlobs = true;
            bc.ObjectsOrder = ObjectsOrder.Size;
            bc.ProcessImage(Filtered);
            Rectangle[] rects = bc.GetObjectsRectangles();
            Rectangle biggest = new Rectangle(0, 0, 0, 0);
            Graphics g = Graphics.FromImage(Filtered);

            if ((rects.Length > 0) && (rects[0].Height > 0))
            {
                biggest = rects[0];
            }

            toReturn.RedBlockDetected = true;
            toReturn.RedBlockCenterLocation = new System.Drawing.Point(((((biggest.Width / 2) + biggest.X))), (biggest.Y + biggest.Height / 2));
            toReturn.RedBlockHeight = biggest.Height;
            toReturn.RedBlockWidth = biggest.Width;
            toReturn.RedBlockDistance = (25.0f / biggest.Height);

            //Needs to be placed in a stats object and passed to the map to be processed
            //===============================================================
            // this.map.blockWidth = biggest.Width;
            // this.map.blockHeightAtOnemeter = 25.0f;
            // this.map.blocksCurrentHeight = biggest.Height;
            // this.map.distanceToWidthSightPathRatio = 0.92f;
            // this.map.imageWidth = Filtered.Width;
            // this.map.blockXLocation = biggest.X;
            //==============================================================
            //map.Draw();
            //User Feedback
            string objectString = Math.Round((43.0f / biggest.Height), 2).ToString();
            string drawString = biggest.Height + " <-- Height    Width --> " + biggest.Width + "\n Image Center = " + (toReturn.RedBlockCenterLocation.X);
            g.DrawRectangle(new Pen(Color.Blue), biggest);
            g.DrawString(objectString, drawFont, Brushes.White, toReturn.RedBlockCenterLocation.X, toReturn.RedBlockCenterLocation.Y, drawFormat);
            g.DrawString(drawString, drawFont, Brushes.White, x, y, drawFormat);
            //UpdateVideo(Filtered);
            return Filtered;
        }
    */

        private Bitmap ExtractYellowFeatures(Bitmap Filtered)
        {
            BlobCounter bc = new BlobCounter();
            Graphics g = Graphics.FromImage(Filtered);
            Pen bluePen = new Pen(Color.Red, 1);
            bc.MinWidth = 100;
            bc.FilterBlobs = true;
            bc.ObjectsOrder = ObjectsOrder.Size;
            bc.ProcessImage(Filtered);
            List<Blob> blob = new List<Blob>();
            Blob [] blobs = bc.GetObjectsInformation();
            blob.AddRange(blobs);

            for (int i = 0; i < blobs.Length; i++)
            {
                for (int j = 0; j < blobs.Length; j++)
                {
                    if (i == j)
                        continue;

                    if (Math.Abs(blobs[i].Rectangle.Y - blobs[j].Rectangle.Y) < 5)
                    {
                        if (!blob.Contains(blobs[i]))
                            blob.Remove(blob[i]);
                    }
                }
            }

            if (blob.Count == 2)
            {
                Console.WriteLine("South");
                Stats a = new Stats();
                a.IsSouth = true;
                a.YellowWallDetected = true;
                Mapping.queue.Add(a);
            }
            else if (blob.Count == 1)
            {
                Console.WriteLine("North");
                Stats a = new Stats();
                a.IsNorth = true;
                a.YellowWallDetected = true;
                Mapping.queue.Add(a);
            }
            else
            {
                Console.WriteLine("erm... houston, we're lost... ");
            }
            return Filtered;
        }

        /*
        private Bitmap ExtractYellowFeatures(Bitmap Filtered)
        {
            BlobCounter bc = new BlobCounter();
            Stats toReturn = new Stats();
            bc.MinWidth = 5;
            bc.MinHeight = 25;
            bc.MaxHeight = 40;
            bc.FilterBlobs = true;
            bc.ObjectsOrder = ObjectsOrder.Size;
            bc.ProcessImage(Filtered);
            Rectangle[] rects = bc.GetObjectsRectangles();
            Rectangle biggest = new Rectangle(0, 0, 0, 0);
            Graphics g = Graphics.FromImage(Filtered);

            if ((rects.Length > 0) && (rects[0].Height > 0))
            {
                biggest = rects[0];
            }

            toReturn.RedBlockDetected = true;
            toReturn.RedBlockCenterLocation = new System.Drawing.Point(((((biggest.Width / 2) + biggest.X))), (biggest.Y + biggest.Height / 2));
            toReturn.RedBlockHeight = biggest.Height;
            toReturn.RedBlockWidth = biggest.Width;
            toReturn.RedBlockDistance = (25.0f / biggest.Height);

            //Needs to be placed in a stats object and passed to the map to be processed
            //===============================================================
            // this.map.blockWidth = biggest.Width;
            // this.map.blockHeightAtOnemeter = 25.0f;
            // this.map.blocksCurrentHeight = biggest.Height;
            // this.map.distanceToWidthSightPathRatio = 0.92f;
            // this.map.imageWidth = Filtered.Width;
            // this.map.blockXLocation = biggest.X;
            //==============================================================
            //map.Draw();
            //User Feedback
            string objectString = Math.Round((43.0f / biggest.Height), 2).ToString();
            string drawString = biggest.Height + " <-- Height    Width --> " + biggest.Width + "\n Image Center = " + (toReturn.RedBlockCenterLocation.X);
            g.DrawRectangle(new Pen(Color.Blue), biggest);
            g.DrawString(objectString, drawFont, Brushes.White, toReturn.RedBlockCenterLocation.X, toReturn.RedBlockCenterLocation.Y, drawFormat);
            g.DrawString(drawString, drawFont, Brushes.White, x, y, drawFormat);
            //UpdateVideo(Filtered);
            return Filtered;
        }
        */

        protected void UpdateVideo(System.Drawing.Image image)
        {
            if (Program.mainForm.InvokeRequired)
            {
                Program.mainForm.Invoke(new System.Windows.Forms.MethodInvoker(delegate { UpdateVideo(image); }));
            }
            else
            {
                Program.mainForm.VideoViewer.Image = image;
            }
        }

        private delegate void videoImageReady(System.Drawing.Image image);

        private Bitmap getImage()
        {
            return Program.mainForm.getImage();
        }

        private bool checkConnection()
        {
            return Program.mainForm.checkConnection();
        }

    }
}