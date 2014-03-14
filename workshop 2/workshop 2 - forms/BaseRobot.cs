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
        protected const int RED = 0;
        protected const int GREEN = 1;
        protected const int WHITE = 2;
        protected const int YELLOW = 3;
        protected const int BLUE = 4;

        protected System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 8);
        protected float x = 10.0F;
        protected float y = 10.0F;
        protected System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
        protected Bitmap[] FilteredImage;
        protected Mapping map = new Mapping();

        protected BaseRobot(string address, string user, string password) : base(address, user, password) { }

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
            map.Draw();
            return this.Camera.Image;
        }

        public abstract void runRovio();

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

        protected Stats ExtractFeatrures(Bitmap[] filtered)
        {
            return null;
        }

        protected void ActionPlanning(Bitmap[] info)
        {

        }

        private Blob ExtractRedFeatures(Bitmap redFiltered)
        {
            BlobCounter bc = new BlobCounter();
            for (int i = RED; i <= BLUE; i++)
            {
                Stats toReturn = new Stats(RED);
                bc.MinWidth = 5;
                bc.MinHeight = 5;
                bc.FilterBlobs = true;
                bc.ObjectsOrder = ObjectsOrder.Size;
                bc.ProcessImage(redFiltered);
                Rectangle[] rects = bc.GetObjectsRectangles();
                Rectangle biggest = new Rectangle(0, 0, 0, 0);
                Graphics g = Graphics.FromImage(redFiltered);
                if ((rects.Length > 0) && (rects[0].Height > 0))
                {
                    biggest = rects[0];
                }
                
                toReturn.RedBlockDetected = true;
                toReturn.RedBlockCenterLocation = new System.Drawing.Point((((biggest.Width / 2) + biggest.X)), (biggest.Y + biggest.Height / 2));

                string objectString = (25.0f / biggest.Height).ToString("#.##");
                string drawString = biggest.Height + " <-- Height    Width --> " + biggest.Width + "\n Image Center = " + toReturn.RedBlockCenterLocation;

                g.DrawRectangle(new Pen(Color.Blue), biggest);
                g.DrawString(objectString, drawFont, Brushes.White, toReturn.RedBlockCenterLocation.X, toReturn.RedBlockCenterLocation.Y, drawFormat);
                g.DrawString(drawString, drawFont, Brushes.White, x, y, drawFormat);
            }
            return null;
        }
    }
}