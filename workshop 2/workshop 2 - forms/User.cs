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
        public User(string address, string user, string password)
            : base(address, user, password)
        {
        }

        public override void runRovio()
        {
            for (int i = 0; i < 10; i++)
            {
               // driveForward();    
            }
            
            while (run)
            {

            }
        }
    }
}