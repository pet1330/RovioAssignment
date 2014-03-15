using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AForge.Imaging.Filters;
using AForge;

namespace Rovio
{
    class User : BaseRobot
    {
        public User(string address, string user, string password) : base(address, user, password){
        }

        public override void runRovio()
        {
            while (checkConnection())
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
                UpdateVideo(RGBImage);
                if (!run) return;
            }
        }
    }
}