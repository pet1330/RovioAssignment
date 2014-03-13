using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AForge.Imaging.Filters;
using AForge;
using System.Windows.Forms;
using AForge.Imaging;

namespace Rovio
{
    class Prey : BaseRobot
    {
        public Prey(string address, string user, string password)
            : base(address, user, password)
        {
            ColourFilters colourfilter = new ColourFilters();
        }

        public override void runRovio()
        {
            while (checkConnection())
            {
                Bitmap RGBImage = getImage();

                //processImage
                //------------------------------------------------------------

                //RGBImage = colourFilter(RGBImage);
                //Feature Extract
                //------------------------------------------------------------

                BlobCounter bc = new BlobCounter();
                bc.MinWidth = 50;
                bc.MinHeight = 5;
                bc.MaxHeight = 50;
                bc.FilterBlobs = true;
                bc.ObjectsOrder = ObjectsOrder.Size;
                bc.ProcessImage(RGBImage);
                Rectangle[] rects = bc.GetObjectsRectangles();
                Rectangle biggest = new Rectangle(0, 0, 0, 0);
                Graphics g = Graphics.FromImage(RGBImage);

                foreach (Blob blob in bc.GetObjectsInformation())
                {
                    List<IntPoint> edgePoints = bc.GetBlobsEdgePoints(blob);
                    List<IntPoint> top;
                    List<IntPoint> bottom;
                    bc.GetBlobsTopAndBottomEdges(blob, out top, out bottom);
                }

                foreach (Rectangle r in rects)
                {
                    biggest = rects[0];
                    g.DrawRectangle(new Pen(Color.Green, 3), r);
                }


                int objectCeter = 0;
                if (biggest.Width > 70)
                {
                    objectCeter = (((biggest.Width / 2) + biggest.X) - RGBImage.Width / 2);
                }

                g.DrawRectangle(new Pen(Color.Blue), biggest);
                g.DrawRectangle(new Pen(Color.White), new Rectangle(objectCeter, 50, 2, 2));

                string drawString = biggest.Height + " <-- Height    Width --> " + biggest.Width + "\n Image Center = " + objectCeter;
                System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 8);
                System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                float x = 10.0F;
                float y = 10.0F;
                System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
                g.DrawString(drawString, drawFont, drawBrush, x, y, drawFormat);
                drawFont.Dispose();
                drawBrush.Dispose();
                Program.mainForm.VideoViewer.Image = RGBImage;
            }
        }
    }
}