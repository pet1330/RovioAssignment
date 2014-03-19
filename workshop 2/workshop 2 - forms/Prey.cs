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
            : base(address, user, password) { }

        public override void runRovio()
        {
            while (run)
            {
                if (checkConnection())
                {
                    Bitmap RGBImage = getImage();
                    FilteredImage = new Bitmap[5];
                  //  this.map.currentLocation = new System.Drawing.Point(120, 190);
                    //this.map.RedBlockLocation = new System.Drawing.Point(0,0);
                   // this.map.orientation = 0;
                    //processImage
                    //------------------------------------------------------------
                    FilteredImage = colourFilter(RGBImage);


                    Console.WriteLine(map.probabilisticMap(150, 150, false));

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
    }
}