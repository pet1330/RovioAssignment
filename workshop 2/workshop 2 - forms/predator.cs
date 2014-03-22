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
            while (run)
            {
                if (checkConnection())
                {
                    Bitmap RGBImage = getImage();
                    FilteredImage = new Bitmap[5];

                    FilteredImage = colourFilter(RGBImage);

                    ExtractFeatrures(FilteredImage);

                   UpdateVideo(RGBImage);
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