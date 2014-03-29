using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using AForge;
using System.Collections.Concurrent;
using AForge.Imaging.Filters;
using AForge.Imaging;
using System.Drawing.Imaging;
using AForge.Math.Geometry;
using System.IO;

namespace Rovio
{
    abstract class BaseRobot : Robot
    {
        public static readonly object commandLock = new object();

        protected volatile bool run;

        protected STATE currentState = STATE.FIND_PREY;

        protected BaseRobot(string address, string user, string password)
            : base(address, user, password)
        {
            run = true;
            //map = new Mapping();
        }

        public abstract void runRovio();

        public void terminateRovio()
        {
            run = false;
        }

        public bool checkConnection()
        {
            try
            {
                lock (commandLock)
                {
                    API.Movement.GetLibNSVersion();
                }
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        public Bitmap getImage()
        {
            lock (commandLock)
            {
                return this.Camera.Image;
            }
        }

        public void rotateRight45()
        {
            lock (commandLock)
            {
                Request("rev.cgi?Cmd=nav&action=18&drive=18&speed=1&angle=3");
                Mapping.orientation += 48;
            }
            System.Threading.Thread.Sleep(1000);
        }

        public void rotateLeft45()
        {
            lock (commandLock)
            {
                Request("rev.cgi?Cmd=nav&action=18&drive=17&speed=1&angle=3");
                Mapping.orientation -= 48;
            }
            System.Threading.Thread.Sleep(1000);
        }

        public void rotateRight15()
        {
            lock (commandLock)
            {
                Request("rev.cgi?Cmd=nav&action=18&drive=18&speed=1&angle=1");
                Mapping.orientation += 15;
            }
            System.Threading.Thread.Sleep(1000);
        }

        public void rotateLeft15()
        {
            lock (commandLock)
            {
                Request("rev.cgi?Cmd=nav&action=18&drive=17&speed=1&angle=1");
                Mapping.orientation -= 15;
            }
            System.Threading.Thread.Sleep(1000);
        }

        public void StrightLeft()
        {
            lock (commandLock)
            {
                Drive.StraightLeft(1);
            }
            Mapping.TranslateLocationLeft(-1);
        }

        public void driveForward()
        {
            lock (commandLock)
            {
                this.Drive.Forward(6);
            }
            System.Threading.Thread.Sleep(200);
        }
        
        public void driveBackward()
        {
            lock (commandLock)
            {
                this.Drive.Backward(6);
            }
            System.Threading.Thread.Sleep(200);
        }

        protected enum STATE
        {
            CHASE,
            FIND_PREY,
            Prey_Caught,
            Prey_Lost,
        }

    }
}