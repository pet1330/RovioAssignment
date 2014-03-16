using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AForge.Imaging.Filters;
using AForge;
using AForge.Imaging;
using System.Windows.Forms;

namespace Rovio
{
    class predator : BaseRobot
    {
        public predator(string address, string user, string password)
            : base(address, user, password)
        {
        }

        public override void runRovio()
        {
            while (checkConnection())
            {
                Bitmap RGBImage = getImage();
                FilteredImage = new Bitmap[5];
                this.map.currentLocation = new System.Drawing.Point(120, 190);
                //this.map.RedBlockLocation = new System.Drawing.Point(0,0);
                this.map.orientation = 90;
                //processImage
                //------------------------------------------------------------
                FilteredImage = colourFilter(RGBImage);
                
                //FeatureExtract
                //------------------------------------------------------------
               // RGBImage = DetectCorners(FilteredImage[RED]);
                ExtractFeatrures(FilteredImage);
                this.map.Draw();
                //------------------------------------------------------------

                // Output to Screen
                //------------------------------------------------------------   
                //UpdateVideo(RGBImage);
                if (!run) return;
            }
        }
    }
}