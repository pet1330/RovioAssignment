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
    class predator : BaseRobot
    {
        public predator(string address, string user, string password) : base(address, user, password) { }

        public override void runRovio()
        {
            while (checkConnection())
            {
                Bitmap RGBImage = getImage();
                FilteredImage = new Bitmap[5];
                //processImage
                //------------------------------------------------------------

                FilteredImage = colourFilter(RGBImage);

                //Feature Extract
                //------------------------------------------------------------
                

                //------------------------------------------------------------



                //------------------------------------------------------------



                //------------------------------------------------------------


                // Output to Screen
                //------------------------------------------------------------
                foreach (Bitmap item in FilteredImage)
                {
                    System.Threading.Thread.Sleep(500);
                    Program.mainForm.VideoViewer.Image = item;
                }
                //Program.mainForm.VideoViewer.Image = RGBImage;
            }
        }
    }
}