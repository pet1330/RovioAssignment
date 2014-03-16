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
        }

        public override void runRovio()
        {
            while (checkConnection() && run)
            {
                Bitmap RGBImage = getImage();
                FilteredImage = new Bitmap[5];
                //processImage
                //------------------------------------------------------------
                FilteredImage = colourFilter(RGBImage);

                //FeatureExtract
                //------------------------------------------------------------
                //RGBImage = DetectCorners(FilteredImage[RED]);

                //------------------------------------------------------------

                // Output to Screen
                //------------------------------------------------------------   
                Program.mainForm.VideoViewer.Image = RGBImage;
            }
            //Reach this line connection is lost
            UpdateVideo(ConnectionLost);
        }
    }
}