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
        public predator(string address, string user, string password) : base(address, user, password) { map.currentLocation = new System.Drawing.Point(100, 100); }

        public override void runRovio()
        {
            while (checkConnection() && (mode==PREDATOR))
            {
                Bitmap RGBImage = getImage();
                FilteredImage = new Bitmap[5];
                //processImage
                //------------------------------------------------------------

                FilteredImage = colourFilter(RGBImage);
                ExtractFeatrures(FilteredImage);
                //Feature Extract
                //------------------------------------------------------------

            }

                //------------------------------------------------------------

            

            //------------------------------------------------------------

        }

        //------------------------------------------------------------

        // Output to Screen
        //------------------------------------------------------------
        
    }
}