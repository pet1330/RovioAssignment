using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using AForge;
using System.Collections.Concurrent;
using AForge.Imaging.Filters;
using AForge.Imaging;

namespace Rovio
{
    abstract class BaseRobot : Robot
    {
        protected Bitmap ConnectionLost = global::Rovio.Properties.Resources.ConnectionLost;
        protected const int RED = 0;
        protected const int GREEN = 1;
        protected const int WHITE = 2;
        protected const int YELLOW = 3;
        protected const int BLUE = 4;

        public Mapping map;
        protected System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 8);
        protected float x = 10.0F;
        protected float y = 10.0F;
        protected System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
        protected Bitmap[] FilteredImage;
        protected volatile bool run;

        protected BaseRobot(string address, string user, string password, Mapping _map)
            : base(address, user, password)
        {
            map = _map;
            run = true;
        }

        public abstract void runRovio();

        public void terminateRovio()
        {
            run = false;
        }

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

        protected bool checkConnection()
        {
            try
            {
                API.Movement.GetLibNSVersion();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected Bitmap getImage()
        {
            return this.Camera.Image;
        }

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
                        filt.Luminance = new Range(0.15f, 1.0f);
                        break;
                    case WHITE:
                        filt.Hue = new IntRange(181, 180);
                        filt.Saturation = new Range(0.0f, 0.7f);
                        filt.Luminance = new Range(0.6f, 0.8f);
                        break;
                    case YELLOW:
                        filt.Hue = new IntRange(25, 80);
                        filt.Saturation = new Range(0.2f, 0.9f);
                        filt.Luminance = new Range(0.25f, 0.8f);
                        break;
                    case BLUE:
                        filt.Hue = new IntRange(200, 260);
                        filt.Saturation = new Range(0.1f, 1.0f);
                        filt.Luminance = new Range(0.0f, 0.7f);
                        break;
                    //Default Red
                    default:
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
            UpdateVideo(ExtractRedFeatures(filtered[RED]));
        }

        protected void ActionPlanning(Bitmap[] info)
        {

        }

        private delegate void videoImageReady(System.Drawing.Image image);

        private Bitmap ExtractRedFeatures(Bitmap Filtered)
        {
            BlobCounter bc = new BlobCounter();
            Stats toReturn = new Stats(RED);
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
            map.Draw();
            //User Feedback
            string objectString = Math.Round((25.0f / biggest.Height), 2).ToString();
            string drawString = biggest.Height + " <-- Height    Width --> " + biggest.Width + "\n Image Center = " + (toReturn.RedBlockCenterLocation.X/* - (Filtered.Width / 2)*/);
            g.DrawRectangle(new Pen(Color.Blue), biggest);
            g.DrawString(objectString, drawFont, Brushes.White, toReturn.RedBlockCenterLocation.X, toReturn.RedBlockCenterLocation.Y, drawFormat);
            g.DrawString(drawString, drawFont, Brushes.White, x, y, drawFormat);
            return Filtered;
        }

        private Bitmap ExtractGreenFeatures(Bitmap Filtered)
        {
            BlobCounter bc = new BlobCounter();
            bc.MinHeight = 15;
            bc.MinWidth = 15;

            bc.FilterBlobs = true;
            bc.ObjectsOrder = ObjectsOrder.Size;
            bc.ProcessImage(Filtered);
            Rectangle[] rects = bc.GetObjectsRectangles();
            Rectangle biggest = new Rectangle(0, 0, 0, 0);
            Graphics g = Graphics.FromImage(Filtered);

            foreach (Rectangle r in rects)
            {
                biggest = rects[0];
            }

            Stats toReturn = new Stats(GREEN);
            toReturn.RedBlockDetected = true;
            toReturn.RedBlockCenterLocation = new System.Drawing.Point(((((biggest.Width / 2) + biggest.X))), (biggest.Y + biggest.Height / 2));
            toReturn.RedBlockHeight = biggest.Height;
            toReturn.RedBlockWidth = biggest.Width;
            toReturn.RedBlockDistance = (25.0f / biggest.Height);

            string objectString = (25.0f / biggest.Height).ToString("#.##");
            string drawString = biggest.Height + " <-- Height    Width --> " + biggest.Width + "\n Image Center = " + (toReturn.RedBlockCenterLocation.X - (Filtered.Width / 2));
            g.DrawRectangle(new Pen(Color.Blue), biggest);
            // g.DrawString(objectString, drawFont, Brushes.White, toReturn.RedBlockCenterLocation.X, toReturn.RedBlockCenterLocation.Y, drawFormat);
            //g.DrawString(drawString, drawFont, Brushes.White, x, y, drawFormat);
            return Filtered;
        }
    }
}