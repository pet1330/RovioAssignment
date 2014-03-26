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
            while (true)
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
                if (currentState == STATE.Prey_Caught) { break; }
            }
        }


        private STATE FIND_PREY()
        {
            if (Mapping.redDetected())
            {
                return STATE.CHASE;
            }

            if (rotation < 360)
            {
                rotation += 45;
                rotateRight45();
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

            CenterPrey();
            while (true)
            {
                if (Mapping.PreyCaught())
                {
                    return STATE.Prey_Caught;
                }
                else if (Mapping.redDetected())
                {
                    driveForward();
                }
                else
                {
                    return STATE.Prey_Lost;
                }
            }
        }

        private STATE PREY_LOST()
        {
            if (Mapping.redLostOnLeft())
            {
                rotation-= 45;
                rotateLeft45();
            }
            else
            {
                rotation += 45;
                rotateRight45();
            }

            if (Mapping.redDetected())
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
            TODO LOGIC CODE HERE
        }

        private STATE ChangeLocation()
        {
            TODO LOGIC CODE HERE
            return STATE.FIND_PREY;
        }

    }
}