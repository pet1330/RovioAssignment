using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace Rovio
{
    class MyRobot : Robot
    {
        public MyRobot(string address, string user, string password)
            : base(address, user, password)
        {
            try { API.Movement.GetLibNSVersion(); } // a dummy request
            catch (Exception)
            {
                //simple way of getting feedback in the form mode
                System.Windows.Forms.MessageBox.Show("Could not connect to the robot");
                return;
            }

            this.Camera.Resolution = Rovio.API.Camera.ImageResolution.CIF;

            if (Camera.Resolution == Rovio.API.Camera.ImageResolution.CIF)
                cameraDimensions = new Vector2(352, 288);
            else if (Camera.Resolution == Rovio.API.Camera.ImageResolution.QCIF)
                cameraDimensions = new Vector2(176, 114);
            else if (Camera.Resolution == Rovio.API.Camera.ImageResolution.CGA)
                cameraDimensions = new Vector2(320, 240);
            else
                cameraDimensions = new Vector2(640, 480);
        }

        enum Tracking
        {
            Searching,
            OnScreen,
            Approaching,
            Roaming,
        };

        public delegate void ImageReady(Image image);
        public event ImageReady SourceImage;
        Bitmap cameraImage;

        AForge.Imaging.Filters.HSLFiltering greenFilter;
        AForge.Imaging.Filters.HSLFiltering redFilter;
        AForge.Imaging.Filters.HSLFiltering whiteFilter;


        int wallHeight = 30;
        int searchingRotationCount = 0;

        // User control
        bool moving;
       

        // Predator        
        Tracking trackingState;
        Vector2 cameraDimensions = Vector2.Zero;
        System.Drawing.Rectangle preyScreenPosition = System.Drawing.Rectangle.Empty;
        System.Drawing.Rectangle obstacleRectangle = System.Drawing.Rectangle.Empty;
        System.Drawing.Rectangle preyRectangle = System.Drawing.Rectangle.Empty;
        System.Drawing.Rectangle wallLineRectangle = System.Drawing.Rectangle.Empty;
        Bitmap redColourBitmap;
        Bitmap greenColourBitmap;
        Bitmap whiteColourBitmap;
        System.Drawing.Point wallLeftPoint = System.Drawing.Point.Empty;
        System.Drawing.Point wallRightPoint = System.Drawing.Point.Empty;
        int wallLineHeight = 0;

        public void Input(List<int> keys)
        {
           // while (true)
            //{

            if (keys.Count > 0)
                moving = true;
            if (keys.Contains(87))
            {
                Drive.Forward(1);        
            }
            else if (keys.Contains(81))
            {
                Drive.RotateLeft(1);
            }
            else if (keys.Contains(69))
            {
                Drive.RotateRight(1);
            }
            else if (keys.Contains(83))
            {
                Drive.Backward(1);
            }
            else if (keys.Contains(65))
            {
                Drive.DiagForwardLeft(1);
            }
            else if (keys.Contains(68))
            {
                Drive.DiagForwardRight(1);
            }
            else if (keys.Count == 0 && moving)
            {
                Drive.Stop();
                moving = false;
            }
        }
        public void User(object keys)
        {
            while (true)
            {
                SourceImage(Camera.Image);
                Input((List<int>)keys);
            }

        }

        
        
        
        public void SetFilters(object values)
        {
            Dictionary<string, float> dict = (Dictionary<string, float>)values;


            greenFilter = new AForge.Imaging.Filters.HSLFiltering();

            float left = 80;

            float right = 0;

            dict.TryGetValue("greenHueMin", out left);
            dict.TryGetValue("greenHueMax", out right);
            greenFilter.Hue = new AForge.IntRange((int)left, (int)right);

            dict.TryGetValue("greenSatMin", out left);
            dict.TryGetValue("greenSatMax", out right);
            greenFilter.Saturation = new AForge.Range(left, right);

            dict.TryGetValue("greenLumMin", out left);
            dict.TryGetValue("greenLumMax", out right);
            greenFilter.Luminance = new AForge.Range(left, right);

            redFilter = new AForge.Imaging.Filters.HSLFiltering();

            dict.TryGetValue("redHueMin", out left);
            dict.TryGetValue("redHueMax", out right);
            redFilter.Hue = new AForge.IntRange((int)left, (int)right);

            dict.TryGetValue("redSatMin", out left);
            dict.TryGetValue("redSatMax", out right);
            redFilter.Saturation = new AForge.Range(left, right);

            dict.TryGetValue("redLumMin", out left);
            dict.TryGetValue("redLumMax", out right);
            redFilter.Luminance = new AForge.Range(left, right);


            whiteFilter = new AForge.Imaging.Filters.HSLFiltering();
            whiteFilter.Hue = new AForge.IntRange(160, 240);
            whiteFilter.Saturation = new AForge.Range(0.1f, 1.0f);
            whiteFilter.Luminance = new AForge.Range(0.1f, 0.7f);
        }

        private Bitmap ConvertImageFormat(Bitmap bmp)
        {
            Bitmap newRed = new Bitmap((int)cameraDimensions.X, (int)cameraDimensions.Y, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(newRed))
                g.DrawImage(bmp, new System.Drawing.Point(0, 0));

            return newRed;
        }

        private Bitmap ApplyColour(Bitmap overBmp, Bitmap underBmp, System.Drawing.Color colour)
        {
            Bitmap newRed = ConvertImageFormat(overBmp);
            AForge.Imaging.Filters.EuclideanColorFiltering filter = new AForge.Imaging.Filters.EuclideanColorFiltering();
            filter.CenterColor = new AForge.Imaging.RGB(System.Drawing.Color.White); //Pure White
            filter.Radius = 0; //Increase this to allow off-whites
            filter.FillColor = new AForge.Imaging.RGB(colour); //Replacement Colour
            filter.FillOutside = false;
            filter.ApplyInPlace(newRed);
            //AForge.Imaging.Filters.Merge bfilter = new AForge.Imaging.Filters.Merge(underBmp);
            //bfilter.Apply(newRed);

            return newRed;
        }

        public System.Drawing.Point GetPoint(Bitmap bmp, int x, System.Drawing.Rectangle rect)
        {

            int lowestPoint = 0;
            int highestPoint = 0;
            for (int i = rect.Y; i < rect.Y + rect.Height; i++)
            {
                System.Drawing.Color col = bmp.GetPixel(x, i);
                System.Drawing.Color checkCol = System.Drawing.Color.FromArgb(255, 0, 0, 0);
                if (bmp.GetPixel(x, i) != checkCol)
                {
                    if (highestPoint == 0)
                        lowestPoint = i;
                    highestPoint = i;

                }
            }
            wallLineHeight = highestPoint - lowestPoint;
            return new System.Drawing.Point(x, highestPoint);//-(highestPoint-lowestPoint));
        }

        public void PerformPredatorActions()
        {
            cameraImage = Camera.Image;
            obstacleRectangle = DetectObstacle(greenFilter, cameraImage, new Vector2(35), ref greenColourBitmap);
            preyRectangle = DetectObstacle(redFilter, cameraImage, new Vector2(10), ref redColourBitmap);
            wallLineRectangle = DetectObstacle(whiteFilter, cameraImage, new Vector2(100, 0), ref whiteColourBitmap);
            redColourBitmap = ConvertImageFormat(redColourBitmap);
            redColourBitmap = ApplyColour(redColourBitmap, cameraImage, System.Drawing.Color.Red);
            greenColourBitmap = ConvertImageFormat(greenColourBitmap);
            greenColourBitmap = ApplyColour(greenColourBitmap, cameraImage, System.Drawing.Color.LightGreen);
            whiteColourBitmap = ApplyColour(whiteColourBitmap, cameraImage, System.Drawing.Color.Cyan);
            whiteColourBitmap = ConvertImageFormat(whiteColourBitmap);

            wallLeftPoint = GetPoint(whiteColourBitmap, wallLineRectangle.X, wallLineRectangle);
            wallRightPoint = GetPoint(whiteColourBitmap, wallLineRectangle.X+wallLineRectangle.Width-1, wallLineRectangle);
            System.Drawing.Point p = GetPoint(whiteColourBitmap, wallLineRectangle.X + wallLineRectangle.Width - (wallLineRectangle.Width/2), wallLineRectangle);
            AForge.Imaging.Filters.HistogramEqualization hFilter = new AForge.Imaging.Filters.HistogramEqualization();

            
            cameraImage = Greyscale(cameraImage);
            cameraImage = ConvertImageFormat(cameraImage);


            AForge.Imaging.Filters.SimplePosterization jFilter = new AForge.Imaging.Filters.SimplePosterization();
           // cameraImage = jFilter.Apply(cameraImage);

            AForge.Imaging.Filters.Merge mFilter = new AForge.Imaging.Filters.Merge(greenColourBitmap);

            Bitmap mergedColourImages = mFilter.Apply(whiteColourBitmap);

            mFilter = new AForge.Imaging.Filters.Merge(mergedColourImages);
            mergedColourImages = mFilter.Apply(redColourBitmap);


            mFilter = new AForge.Imaging.Filters.Merge(cameraImage);
            cameraImage = mFilter.Apply(mergedColourImages);

            //cameraImage = whiteColourBitmap;
            if (preyRectangle != new System.Drawing.Rectangle(0, 0, 0, 0))
                preyScreenPosition = preyRectangle;
            if (preyRectangle == new System.Drawing.Rectangle(0, 0, 0, 0) && searchingRotationCount < 8)
                trackingState = Tracking.Searching;
            else if (searchingRotationCount >= 8 && trackingState != Tracking.OnScreen)
                trackingState = Tracking.Roaming;
            else
                trackingState = Tracking.OnScreen;
            RobotCommands();
        }

        public void RobotCommands()
        {
            //trackingState = Tracking.OnScreen;

            if (trackingState == Tracking.Searching)
            {
                //Search();
            }
            else if (trackingState == Tracking.Roaming)
            {
                // Roam();
            }
            else
            {
                // Approach();
            }
        }
        public void Predator()
        {
            cameraImage = Camera.Image;
            //SetFilters();
            searchingRotationCount = 8;
            while (true)
            {                            
                if (obstacleRectangle != new System.Drawing.Rectangle(0, 0, 0, 0))
                    cameraImage = DrawRect(cameraImage, obstacleRectangle, System.Drawing.Color.Blue, 3f);
                if (preyRectangle != new System.Drawing.Rectangle(0, 0, 0, 0))
                {
                    searchingRotationCount = 0;
                    cameraImage = DrawRect(cameraImage, preyRectangle, System.Drawing.Color.Pink, 3f);
                }
                //if (wallLineRectangle != new System.Drawing.Rectangle(0, 0, 0, 0))
                //    cameraImage = DrawRect(cameraImage, wallLineRectangle, System.Drawing.Color.Green, 3f);

                DrawLine(cameraImage, wallLeftPoint, wallRightPoint, System.Drawing.Color.Red, wallLineHeight);
                SourceImage(new Bitmap(cameraImage));
                PerformPredatorActions();

                Console.WriteLine(searchingRotationCount);

            } 
        }

        public Bitmap Greyscale(Bitmap image)
        {
            return AForge.Imaging.Filters.Grayscale.CommonAlgorithms.Y.Apply(image);
        }

        public Bitmap Threshold(Bitmap image, int value)
        {
            AForge.Imaging.Filters.Threshold filter = new AForge.Imaging.Filters.Threshold(value);
            return filter.Apply(AForge.Imaging.Filters.Grayscale.CommonAlgorithms.Y.Apply(image));
        }

        public Bitmap Erode(Bitmap image, int threshValue)
        {
            AForge.Imaging.Filters.Erosion filter = new AForge.Imaging.Filters.Erosion();
            return filter.Apply(Threshold(image, threshValue));
        }

        public System.Drawing.Rectangle DetectObstacle(AForge.Imaging.Filters.HSLFiltering filter, Bitmap image, Vector2 minRect, ref Bitmap colourBitmap)
        {
            Bitmap filtered = filter.Apply(image);
            short[,] structuringElement = new short[,] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };

            


            filtered = Threshold(filtered, 1);
            AForge.Imaging.Filters.Opening openingFilter = new AForge.Imaging.Filters.Opening(structuringElement);
            //filtered = openingFilter.Apply(filtered);

            colourBitmap = filtered;
            
            AForge.Imaging.BlobCounter blobs = new AForge.Imaging.BlobCounter(filtered);
            System.Drawing.Rectangle[] rectangles = blobs.GetObjectsRectangles();

            int size = 0;
            int chosen = 0;
            int newSize = 0;
            for (int i = 0; i < rectangles.Length; i++)
            {
                newSize = rectangles[i].Height * rectangles[i].Width;

                if (size < newSize)
                {
                    chosen = i;
                    size = newSize;
                }
            }

            if (rectangles.Length != 0)
            {
                if (rectangles[chosen].Width > minRect.X && rectangles[chosen].Height > minRect.Y)
                {
                    return rectangles[chosen];
                }
            }
            return new System.Drawing.Rectangle(0, 0, 0, 0);
        }

        public Bitmap DrawRect(Bitmap image, System.Drawing.Rectangle rect, System.Drawing.Color colour, float lineWeight)
        {
            Graphics rectPen = Graphics.FromImage(image);
            rectPen.DrawRectangle(new Pen(colour, lineWeight), rect);
            return image;
        }

        public Bitmap DrawLine(Bitmap image, System.Drawing.Point p1, System.Drawing.Point p2, System.Drawing.Color colour, float lineWeight)
        {
            Graphics rectPen = Graphics.FromImage(image);
            rectPen.DrawLine(new Pen(colour, lineWeight), p1, p2);
            return image;
        }


        public void Search()
        {
            for (int i = 0; i < 2; i++)
                if (preyScreenPosition.X < cameraDimensions.X / 2)
                    Drive.RotateLeft(3);
                else
                    Drive.RotateRight(3);

            searchingRotationCount++;
            if (searchingRotationCount > 8)
                trackingState = Tracking.Roaming;
            Drive.Stop();
        }

        public void NewRoaming()
        { 
        
        }

        public void Roam()
        {
            //if (wallHeight > 20)
            //{
            if (IRSensor.Detection || wallLineHeight > 15)
            {
                for (int i = 0; i < 8; i++)
                    Drive.Backward(1);

                preyRectangle = DetectObstacle(redFilter, cameraImage, new Vector2(10), ref redColourBitmap);

                if (wallLeftPoint.Y > wallRightPoint.Y)
                    for (int i = 0; i < 4; i++)
                    {
                        Drive.RotateRight(3);
                    }
                else
                    for (int i = 0; i < 4; i++)
                    {
                        Drive.RotateLeft(3);
                    }
            }

            else if ((obstacleRectangle.Height < 200 || obstacleRectangle.Width < 100))// && (wallLineRectangle.Height > 25 || wallLineRectangle.Height < 40))
                for (int i = 0; i < 3; i++)
                    Drive.Forward(1);
            else
            {
                if (obstacleRectangle.X > cameraDimensions.X / 8)
                    for (int i = 0; i < 4; i++)
                    {
                        Drive.RotateLeft(3);
                    }
                else
                    for (int i = 0; i < 4; i++)
                    {
                        Drive.RotateRight(3);
                    }
                //obstacleRectangle = new System.Drawing.Rectangle(0, 0, 0, 0);
               // PerformPredatorActions();
                //searchingRotationCount = 0;
                Drive.Stop();
            }
            //}
           
           trackingState = Tracking.Roaming;

        }

        public void Approach()
        {
            //trackingState = Tracking.OnScreen;
           // searchingRotationCount = 0;
            if (preyScreenPosition.X < 0 + cameraDimensions.X / 5)
            {
                for (int i = 0; i < 1; i++)
                    Drive.RotateLeft20(2);
                Drive.Stop();
            }
            else if (preyScreenPosition.X > cameraDimensions.X - cameraDimensions.X / 5)
            {
                for (int i = 0; i < 1; i++)
                    Drive.RotateRight20(2);
                Drive.Stop();
            }
            else if (preyRectangle.Width < 80)
            {
                trackingState = Tracking.Approaching;
                Drive.Forward(1);
            }
            else
                Drive.Stop();
        }

        // Old
        /*
        public System.Drawing.Rectangle DetectPrey(Bitmap image, Vector2 minRect)
        {
            AForge.Imaging.Filters.HSLFiltering filter = new AForge.Imaging.Filters.HSLFiltering();
            filter.Hue = new AForge.IntRange(355, 20);
            filter.Saturation = new AForge.Range(0.5f, 1.8f);
            filter.Luminance = new AForge.Range(0.15f, 1.0f);
            Bitmap filtered = filter.Apply(image);

            short[,] structuringElement = new short[,] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
            filtered = Threshold(filtered, 1);
            AForge.Imaging.Filters.Opening openingFilter = new AForge.Imaging.Filters.Opening(structuringElement);
            filtered = openingFilter.Apply(filtered);

            AForge.Imaging.BlobCounter blobs = new AForge.Imaging.BlobCounter(filtered);
            System.Drawing.Rectangle[] rectangles = blobs.GetObjectsRectangles();

            int size = 0;
            int chosen = 0;

            for (int i = 0; i < rectangles.Length; i++)
            {
                int newSize = rectangles[i].Height * rectangles[i].Width;

                if (size < newSize)
                {
                    chosen = i;
                    size = newSize;
                }
            }
            Graphics rect = Graphics.FromImage(image);

            if (rectangles.Length != 0)
            {
                if (rectangles[chosen].Width > minRect.X && rectangles[chosen].Height > minRect.Y)
                {
                    trackingState = Tracking.OnScreen;
                    Hunt(rectangles[chosen]);
                    return rectangles[chosen];
                }
            }
            return new System.Drawing.Rectangle(0, 0, 0, 0);
        }

       


        public Bitmap DetectObject(Bitmap image)
        {            
            AForge.Imaging.Filters.HSLFiltering filter = new AForge.Imaging.Filters.HSLFiltering();
            filter.Hue = new AForge.IntRange(355, 20);
            filter.Saturation = new AForge.Range(0.5f, 1.8f);
            filter.Luminance = new AForge.Range(0.15f, 1.0f);
            Bitmap filtered = filter.Apply(image);

            
            short[,] se = new short[,] {{0, 1, 0}, {1, 1, 1}, {0, 1, 0}};
            filtered = Threshold(filtered, 1);
            AForge.Imaging.Filters.Opening ffFilter = new AForge.Imaging.Filters.Opening(se);
            filtered = ffFilter.Apply(filtered);


            AForge.Imaging.BlobCounter blobs = new AForge.Imaging.BlobCounter(filtered);
            System.Drawing.Rectangle[] rectangles = blobs.GetObjectsRectangles();

            int size = 0;
            int chosen = 0;

            for (int i = 0; i < rectangles.Length; i++)
            {
                int newSize = rectangles[i].Height * rectangles[i].Width;

                if (size < newSize)
                {
                    chosen = i;
                    size = newSize;
                }
            }

           //return filtered;
            Graphics rect = Graphics.FromImage(image);

           

            if (rectangles.Length != 0)
            {
                if (rectangles[chosen].Width > 15 && rectangles[chosen].Height > 10)
                {
                    preyScreenPosition = new System.Drawing.Rectangle(rectangles[chosen].X, rectangles[chosen].Y, rectangles[chosen].Width, rectangles[chosen].Height);
                    trackingState = Tracking.OnScreen;
                    rect.DrawRectangle(new Pen(System.Drawing.Color.Green, 3f), rectangles[chosen]);
                    totalTime = 0;
                    Hunt(rectangles[chosen]);
                }
                else if (checkingOnScreenTimer < 5)
                {
                    checkingOnScreenTimer++;
                }                
            }
            else if (rectangles.Length == 0)
            {
                if (roamingRotationCount < 20)
                    trackingState = Tracking.Searching;
                else
                    trackingState = Tracking.Roaming;
            }

            if (trackingState == Tracking.Searching)
            {
                roamingRotationCount++;
                for (int i = 0; i < 2; i++)
                    if (preyScreenPosition.X < cameraWidth.X/2)
                        Drive.RotateLeft(3);
                    else
                        Drive.RotateRight(3);
                Drive.Stop();

                Console.WriteLine(circleCount);
            }
            return image;
        }

        
         * 
         * */

    }
}
