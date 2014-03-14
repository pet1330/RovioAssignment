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
            while (checkConnection() && (mode == PREY))
            {
                Bitmap RGBImage = getImage();

                Program.mainForm.VideoViewer.Image = RGBImage;
            }
        }
    }
}