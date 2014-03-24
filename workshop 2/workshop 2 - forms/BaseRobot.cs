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
        public readonly object commandLock = new object();

        protected volatile bool run;

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


        }

        public void rotateLeft45()
        {


        }

        public void rotateRight90()
        {
            lock (commandLock)
            {
                Request("rev.cgi?Cmd=nav&action=18&drive=18&speed=1&angle=7");
                Mapping.orientation += 90;
            }
        }

        public void rotateLeft90()
        {
            lock (commandLock)
            {
                Request("rev.cgi?Cmd=nav&action=18&drive=17&speed=1&angle=7");
                Mapping.orientation -= 90;
            }
        }


        /*
        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }

            return array;
        }
        */
    }
}