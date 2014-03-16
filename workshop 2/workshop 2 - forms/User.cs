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
                this.map.currentLocation = new System.Drawing.Point(130, 150);
                this.map.orientation = 0;
               // this.map.Draw();
                //processImage
                //------------------------------------------------------------
                FilteredImage = colourFilter(RGBImage);

                //FeatureExtract
                //------------------------------------------------------------
                // RGBImage = DetectCorners(FilteredImage[RED]);
                ExtractFeatrures(FilteredImage);
                //------------------------------------------------------------

                // Output to Screen
                //------------------------------------------------------------   
                //UpdateVideo(RGBImage);
                if (!run) return;
            }
            //Reach this line connection is lost
            UpdateVideo(ConnectionLost);
        }
    }
}