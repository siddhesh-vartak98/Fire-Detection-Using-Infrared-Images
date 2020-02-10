namespace AForge.Imaging
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class HoughLineTransformation
    {
        private double[] cosMap;
        private int houghHeight;
        private short[,] houghMap;
        private ArrayList lines = new ArrayList();
        private int localPeakRadius = 4;
        private short maxMapIntensity;
        private short minLineIntensity = 10;
        private double[] sinMap;
        private int stepsPerDegree;
        private double thetaStep;

        public HoughLineTransformation()
        {
            this.StepsPerDegree = 1;
        }

        private void CollectLines()
        {
            int length = this.houghMap.GetLength(0);
            int num2 = this.houghMap.GetLength(1);
            int num4 = num2 >> 1;
            this.lines.Clear();
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < num2; j++)
                {
                    short intensity = this.houghMap[i, j];
                    if (intensity >= this.minLineIntensity)
                    {
                        bool flag = false;
                        int num7 = i - this.localPeakRadius;
                        int num8 = i + this.localPeakRadius;
                        while (num7 < num8)
                        {
                            if (flag)
                            {
                                break;
                            }
                            int num9 = num7;
                            int num10 = j;
                            if (num9 < 0)
                            {
                                num9 = length + num9;
                                num10 = num2 - num10;
                            }
                            if (num9 >= length)
                            {
                                num9 -= length;
                                num10 = num2 - num10;
                            }
                            int num11 = num10 - this.localPeakRadius;
                            int num12 = num10 + this.localPeakRadius;
                            while (num11 < num12)
                            {
                                if (num11 >= 0)
                                {
                                    if (num11 >= num2)
                                    {
                                        break;
                                    }
                                    if (this.houghMap[num9, num11] > intensity)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                num11++;
                            }
                            num7++;
                        }
                        if (!flag)
                        {
                            this.lines.Add(new HoughLine(((double) i) / ((double) this.stepsPerDegree), (short) (j - num4), intensity, ((double) intensity) / ((double) this.maxMapIntensity)));
                        }
                    }
                }
            }
            this.lines.Sort();
        }

        public HoughLine[] GetLinesByRelativeIntensity(double minRelativeIntensity)
        {
            int count = 0;
            int num2 = this.lines.Count;
            while ((count < num2) && (((HoughLine) this.lines[count]).RelativeIntensity >= minRelativeIntensity))
            {
                count++;
            }
            return this.GetMostIntensiveLines(count);
        }

        public HoughLine[] GetMostIntensiveLines(int count)
        {
            int num = Math.Min(count, this.lines.Count);
            HoughLine[] array = new HoughLine[num];
            this.lines.CopyTo(0, array, 0, num);
            return array;
        }

        public void ProcessImage(UnmanagedImage image)
        {
            this.ProcessImage(image, new Rectangle(0, 0, image.Width, image.Height));
        }

        public void ProcessImage(Bitmap image)
        {
            this.ProcessImage(image, new Rectangle(0, 0, image.Width, image.Height));
        }

        public void ProcessImage(BitmapData imageData)
        {
            this.ProcessImage(new UnmanagedImage(imageData), new Rectangle(0, 0, imageData.Width, imageData.Height));
        }

        public unsafe void ProcessImage(UnmanagedImage image, Rectangle rect)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
            int width = image.Width;
            int height = image.Height;
            int num3 = width / 2;
            int num4 = height / 2;
            rect.Intersect(new Rectangle(0, 0, width, height));
            int num5 = -num3 + rect.Left;
            int num6 = -num4 + rect.Top;
            int num7 = (width - num3) - (width - rect.Right);
            int num8 = (height - num4) - (height - rect.Bottom);
            int num9 = image.Stride - rect.Width;
            int num10 = (int) Math.Sqrt((double) ((num3 * num3) + (num4 * num4)));
            int num11 = num10 * 2;
            this.houghMap = new short[this.houghHeight, num11];
            byte* numPtr = (byte*) ((image.ImageData.ToPointer() + (rect.Top * image.Stride)) + rect.Left);
            for (int i = num6; i < num8; i++)
            {
                int num13 = num5;
                while (num13 < num7)
                {
                    if (numPtr[0] != 0)
                    {
                        for (int k = 0; k < this.houghHeight; k++)
                        {
                            int num15 = ((int) Math.Round((double) ((this.cosMap[k] * num13) - (this.sinMap[k] * i)))) + num10;
                            if ((num15 >= 0) && (num15 < num11))
                            {
                                short num1 = this.houghMap[k, num15];
                                num1[0] = (short) (num1[0] + 1);
                            }
                        }
                    }
                    num13++;
                    numPtr++;
                }
                numPtr += num9;
            }
            this.maxMapIntensity = 0;
            for (int j = 0; j < this.houghHeight; j++)
            {
                for (int m = 0; m < num11; m++)
                {
                    if (this.houghMap[j, m] > this.maxMapIntensity)
                    {
                        this.maxMapIntensity = this.houghMap[j, m];
                    }
                }
            }
            this.CollectLines();
        }

        public void ProcessImage(Bitmap image, Rectangle rect)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            try
            {
                this.ProcessImage(new UnmanagedImage(bitmapData), rect);
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
        }

        public void ProcessImage(BitmapData imageData, Rectangle rect)
        {
            this.ProcessImage(new UnmanagedImage(imageData), rect);
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

        public int LinesCount
        {
            get
            {
                return this.lines.Count;
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

        public short MinLineIntensity
        {
            get
            {
                return this.minLineIntensity;
            }
            set
            {
                this.minLineIntensity = value;
            }
        }

        public int StepsPerDegree
        {
            get
            {
                return this.stepsPerDegree;
            }
            set
            {
                this.stepsPerDegree = Math.Max(1, Math.Min(10, value));
                this.houghHeight = 180 * this.stepsPerDegree;
                this.thetaStep = 3.1415926535897931 / ((double) this.houghHeight);
                this.sinMap = new double[this.houghHeight];
                this.cosMap = new double[this.houghHeight];
                for (int i = 0; i < this.houghHeight; i++)
                {
                    this.sinMap[i] = Math.Sin(i * this.thetaStep);
                    this.cosMap[i] = Math.Cos(i * this.thetaStep);
                }
            }
        }
    }
}

