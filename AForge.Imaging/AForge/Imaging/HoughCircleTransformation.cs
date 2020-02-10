namespace AForge.Imaging
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class HoughCircleTransformation
    {
        private ArrayList circles = new ArrayList();
        private int height;
        private short[,] houghMap;
        private int localPeakRadius = 4;
        private short maxMapIntensity;
        private short minCircleIntensity = 10;
        private int radiusToDetect;
        private int width;

        public HoughCircleTransformation(int radiusToDetect)
        {
            this.radiusToDetect = radiusToDetect;
        }

        private void CollectCircles()
        {
            this.circles.Clear();
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {
                    short intensity = this.houghMap[i, j];
                    if (intensity < this.minCircleIntensity)
                    {
                        continue;
                    }
                    bool flag = false;
                    int num4 = i - this.localPeakRadius;
                    int num5 = i + this.localPeakRadius;
                    while (num4 < num5)
                    {
                        if (num4 >= 0)
                        {
                            if (flag || (num4 >= this.height))
                            {
                                goto Label_00B0;
                            }
                            int num6 = j - this.localPeakRadius;
                            int num7 = j + this.localPeakRadius;
                            while (num6 < num7)
                            {
                                if (num6 >= 0)
                                {
                                    if (num6 >= this.width)
                                    {
                                        break;
                                    }
                                    if (this.houghMap[num4, num6] > intensity)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                num6++;
                            }
                        }
                        num4++;
                    }
                Label_00B0:
                    if (!flag)
                    {
                        this.circles.Add(new HoughCircle(j, i, this.radiusToDetect, intensity, ((double) intensity) / ((double) this.maxMapIntensity)));
                    }
                }
            }
            this.circles.Sort();
        }

        private void DrawHoughCircle(int xCenter, int yCenter)
        {
            int x = 0;
            int radiusToDetect = this.radiusToDetect;
            int num3 = (5 - (this.radiusToDetect * 4)) / 4;
            this.SetHoughCirclePoints(xCenter, yCenter, x, radiusToDetect);
            while (x < radiusToDetect)
            {
                x++;
                if (num3 < 0)
                {
                    num3 += (2 * x) + 1;
                }
                else
                {
                    radiusToDetect--;
                    num3 += (2 * (x - radiusToDetect)) + 1;
                }
                this.SetHoughCirclePoints(xCenter, yCenter, x, radiusToDetect);
            }
        }

        public HoughCircle[] GetCirclesByRelativeIntensity(double minRelativeIntensity)
        {
            int count = 0;
            int num2 = this.circles.Count;
            while ((count < num2) && (((HoughCircle) this.circles[count]).RelativeIntensity >= minRelativeIntensity))
            {
                count++;
            }
            return this.GetMostIntensiveCircles(count);
        }

        public HoughCircle[] GetMostIntensiveCircles(int count)
        {
            int num = Math.Min(count, this.circles.Count);
            HoughCircle[] array = new HoughCircle[num];
            this.circles.CopyTo(0, array, 0, num);
            return array;
        }

        public unsafe void ProcessImage(UnmanagedImage image)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
            this.width = image.Width;
            this.height = image.Height;
            int num = image.Stride - this.width;
            this.houghMap = new short[this.height, this.width];
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            for (int i = 0; i < this.height; i++)
            {
                int xCenter = 0;
                while (xCenter < this.width)
                {
                    if (numPtr[0] != 0)
                    {
                        this.DrawHoughCircle(xCenter, i);
                    }
                    xCenter++;
                    numPtr++;
                }
                numPtr += num;
            }
            this.maxMapIntensity = 0;
            for (int j = 0; j < this.height; j++)
            {
                for (int k = 0; k < this.width; k++)
                {
                    if (this.houghMap[j, k] > this.maxMapIntensity)
                    {
                        this.maxMapIntensity = this.houghMap[j, k];
                    }
                }
            }
            this.CollectCircles();
        }

        public void ProcessImage(Bitmap image)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            try
            {
                this.ProcessImage(new UnmanagedImage(bitmapData));
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
        }

        public void ProcessImage(BitmapData imageData)
        {
            this.ProcessImage(new UnmanagedImage(imageData));
        }

        private void SetHoughCirclePoints(int cx, int cy, int x, int y)
        {
            if (x == 0)
            {
                this.SetHoughPoint(cx, cy + y);
                this.SetHoughPoint(cx, cy - y);
                this.SetHoughPoint(cx + y, cy);
                this.SetHoughPoint(cx - y, cy);
            }
            else if (x == y)
            {
                this.SetHoughPoint(cx + x, cy + y);
                this.SetHoughPoint(cx - x, cy + y);
                this.SetHoughPoint(cx + x, cy - y);
                this.SetHoughPoint(cx - x, cy - y);
            }
            else if (x < y)
            {
                this.SetHoughPoint(cx + x, cy + y);
                this.SetHoughPoint(cx - x, cy + y);
                this.SetHoughPoint(cx + x, cy - y);
                this.SetHoughPoint(cx - x, cy - y);
                this.SetHoughPoint(cx + y, cy + x);
                this.SetHoughPoint(cx - y, cy + x);
                this.SetHoughPoint(cx + y, cy - x);
                this.SetHoughPoint(cx - y, cy - x);
            }
        }

        private void SetHoughPoint(int x, int y)
        {
            if (((x >= 0) && (y >= 0)) && ((x < this.width) && (y < this.height)))
            {
                short num1 = this.houghMap[y, x];
                num1[0] = (short) (num1[0] + 1);
            }
        }

        public unsafe Bitmap ToBitmap()
        {
            if (this.houghMap == null)
            {
                throw new ApplicationException("Hough transformation was not done yet.");
            }
            int length = this.houghMap.GetLength(1);
            int height = this.houghMap.GetLength(0);
            Bitmap bitmap = Image.CreateGrayscaleImage(length, height);
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, length, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            int num3 = bitmapdata.Stride - length;
            float num4 = 255f / ((float) this.maxMapIntensity);
            byte* numPtr = (byte*) bitmapdata.Scan0.ToPointer();
            for (int i = 0; i < height; i++)
            {
                int num6 = 0;
                while (num6 < length)
                {
                    numPtr[0] = (byte) Math.Min(0xff, (int) (num4 * this.houghMap[i, num6]));
                    num6++;
                    numPtr++;
                }
                numPtr += num3;
            }
            bitmap.UnlockBits(bitmapdata);
            return bitmap;
        }

        public int CirclesCount
        {
            get
            {
                return this.circles.Count;
            }
        }

        public int LocalPeakRadius
        {
            get
            {
                return this.localPeakRadius;
            }
            set
            {
                this.localPeakRadius = Math.Max(1, Math.Min(10, value));
            }
        }

        public short MaxIntensity
        {
            get
            {
                return this.maxMapIntensity;
            }
        }

        public short MinCircleIntensity
        {
            get
            {
                return this.minCircleIntensity;
            }
            set
            {
                this.minCircleIntensity = value;
            }
        }
    }
}

