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
        private int rotation = 0;

        public predator(string address, string user, string password)
            : base(address, user, password)
        {
        }

        public override void runRovio()
        {
            System.Threading.Thread.Sleep(1000);
            while (run)
            {

                switch (currentState)
                {
                    case STATE.FIND_PREY:
                        currentState = FIND_PREY();
                        break;
                    case STATE.CHASE:
                        currentState = CHASE();
                        break;
                    case STATE.Prey_Lost:
                        currentState = PREY_LOST();
                        break;
                    default:
                        break;
                }
                if (currentState == STATE.Prey_Caught)
                {
                    break;
                }
            }
        }

        private STATE FIND_PREY()
        {
            if (Mapping.redBlockDetected())
            {
                return STATE.CHASE;
            }

            if (rotation < 360)
            {
                rotateRight45();
                System.Threading.Thread.Sleep(1500);
                return STATE.FIND_PREY;
            }
            else
            {
                rotation = 0;
                return ChangeLocation();
            }
        }

        private STATE CHASE()
        {
            if (Mapping.PreyCaught())
            {
                return STATE.Prey_Caught;
            }


            while (run)
            {
                CenterPrey();
                if (Mapping.PreyCaught())
                {
                    return STATE.Prey_Caught;
                }
                else if (Mapping.redBlockDetected())
                {
                    driveForward();
                }
                else
                {
                    return STATE.Prey_Lost;
                }
            }
            return STATE.FIND_PREY;
        }

        private STATE PREY_LOST()
        {
            if (Mapping.redLostOnLeft())
            {
                rotateLeft15();
                System.Threading.Thread.Sleep(1500);
            }
            else
            {
                rotateRight15();
                System.Threading.Thread.Sleep(1500);
            }

            if (Mapping.redBlockDetected())
            {
                return STATE.CHASE;
            }
            else
            {
                return STATE.FIND_PREY;
            }
        }

        private void CenterPrey()
        {
            if (Mapping.lastStats.RedBlockCenterLocation.X > 300)
            {
                rotateRight15();
                System.Threading.Thread.Sleep(1000);
            }

            if (Mapping.lastStats.RedBlockCenterLocation.X < 50)
            {
                rotateLeft15();
                System.Threading.Thread.Sleep(1000);
            }
        }

        private STATE ChangeLocation()
        {
            AStar nav = new AStar();
            nav.FindPath(Mapping.currentLocation, new System.Drawing.Point(Math.Abs(Mapping.currentLocation.X - 260), Math.Abs(Mapping.currentLocation.Y - 300)));

            foreach (System.Drawing.Point n in nav.path)
            {
                Mapping.set(n, 1);

            }
            return STATE.FIND_PREY;
        }
    }
}