namespace AForge.Imaging
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class DocumentSkewChecker
    {
        private double[] cosMap;
        private int houghHeight;
        private short[,] houghMap;
        private ArrayList lines = new ArrayList();
        private int localPeakRadius = 4;
        private short maxMapIntensity;
        private double maxSkewToDetect;
        private bool needToInitialize = true;
        private double[] sinMap;
        private int stepsPerDegree;
        private double thetaStep;

        public DocumentSkewChecker()
        {
            this.StepsPerDegree = 10;
            this.MaxSkewToDetect = 30.0;
        }

        private void CollectLines(short minLineIntensity)
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
                    if (intensity < minLineIntensity)
                    {
                        continue;
                    }
                    bool flag = false;
                    int num7 = i - this.localPeakRadius;
                    int num8 = i + this.localPeakRadius;
                    while (num7 < num8)
                    {
                        if (num7 >= 0)
                        {
                            if ((num7 >= length) || flag)
                            {
                                goto Label_00C8;
                            }
                            int num9 = j - this.localPeakRadius;
                            int num10 = j + this.localPeakRadius;
                            while (num9 < num10)
                            {
                                if (num9 >= 0)
                                {
                                    if (num9 >= num2)
                                    {
                                        break;
                                    }
                                    if (this.houghMap[num7, num9] > intensity)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                num9++;
                            }
                        }
                        num7++;
                    }
                Label_00C8:
                    if (!flag)
                    {
                        this.lines.Add(new HoughLine((90.0 - this.maxSkewToDetect) + (((double) i) / ((double) this.stepsPerDegree)), (short) (j - num4), intensity, ((double) intensity) / ((double) this.maxMapIntensity)));
                    }
                }
            }
            this.lines.Sort();
        }

        private HoughLine[] GetMostIntensiveLines(int count)
        {
            int num = Math.Min(count, this.lines.Count);
            HoughLine[] array = new HoughLine[num];
            this.lines.CopyTo(0, array, 0, num);
            return array;
        }

        public double GetSkewAngle(UnmanagedImage image)
        {
            return this.GetSkewAngle(image, new Rectangle(0, 0, image.Width, image.Height));
        }

        public double GetSkewAngle(Bitmap image)
        {
            return this.GetSkewAngle(image, new Rectangle(0, 0, image.Width, image.Height));
        }

        public double GetSkewAngle(BitmapData imageData)
        {
            return this.GetSkewAngle(new UnmanagedImage(imageData), new Rectangle(0, 0, imageData.Width, imageData.Height));
        }

        public unsafe double GetSkewAngle(UnmanagedImage image, Rectangle rect)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
            this.InitHoughMap();
            int width = image.Width;
            int height = image.Height;
            int num3 = width / 2;
            int num4 = height / 2;
            rect.Intersect(new Rectangle(0, 0, width, height));
            int num5 = -num3 + rect.Left;
            int num6 = -num4 + rect.Top;
            int num7 = (width - num3) - (width - rect.Right);
            int num8 = ((height - num4) - (height - rect.Bottom)) - 1;
            int num9 = image.Stride - rect.Width;
            int num10 = (int) Math.Sqrt((double) ((num3 * num3) + (num4 * num4)));
            int num11 = num10 * 2;
            this.houghMap = new short[this.houghHeight, num11];
            byte* numPtr = (byte*) ((image.ImageData.ToPointer() + (rect.Top * image.Stride)) + rect.Left);
            byte* numPtr2 = numPtr + image.Stride;
            for (int i = num6; i < num8; i++)
            {
                int num13 = num5;
                while (num13 < num7)
                {
                    if ((numPtr[0] < 0x80) && (numPtr2[0] >= 0x80))
                    {
                        for (int k = 0; k < this.houghHeight; k++)
                        {
                            int num15 = ((int) ((this.cosMap[k] * num13) - (this.sinMap[k] * i))) + num10;
                            if ((num15 >= 0) && (num15 < num11))
                            {
                                short num1 = this.houghMap[k, num15];
                                num1[0] = (short) (num1[0] + 1);
                            }
                        }
                    }
                    num13++;
                    numPtr++;
                    numPtr2++;
                }
                numPtr += num9;
                numPtr2 += num9;
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
            this.CollectLines((short) (width / 10));
            HoughLine[] mostIntensiveLines = this.GetMostIntensiveLines(5);
            double num18 = 0.0;
            double num19 = 0.0;
            foreach (HoughLine line in mostIntensiveLines)
            {
                if (line.RelativeIntensity > 0.5)
                {
                    num18 += line.Theta * line.RelativeIntensity;
                    num19 += line.RelativeIntensity;
                }
            }
            if (mostIntensiveLines.Length > 0)
            {
                num18 /= num19;
            }
            return (num18 - 90.0);
        }

        public double GetSkewAngle(Bitmap image, Rectangle rect)
        {
            double skewAngle;
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            try
            {
                skewAngle = this.GetSkewAngle(new UnmanagedImage(bitmapData), rect);
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
            return skewAngle;
        }

        public double GetSkewAngle(BitmapData imageData, Rectangle rect)
        {
            return this.GetSkewAngle(new UnmanagedImage(imageData), rect);
        }

        private void InitHoughMap()
        {
            if (this.needToInitialize)
            {
                this.needToInitialize = false;
                this.houghHeight = (int) ((2.0 * this.maxSkewToDetect) * this.stepsPerDegree);
                this.thetaStep = (((2.0 * this.maxSkewToDetect) * 3.1415926535897931) / 180.0) / ((double) this.houghHeight);
                this.sinMap = new double[this.houghHeight];
                this.cosMap = new double[this.houghHeight];
                double num = 90.0 - this.maxSkewToDetect;
                for (int i = 0; i < this.houghHeight; i++)
                {
                    this.sinMap[i] = Math.Sin(((num * 3.1415926535897931) / 180.0) + (i * this.thetaStep));
                    this.cosMap[i] = Math.Cos(((num * 3.1415926535897931) / 180.0) + (i * this.thetaStep));
                }
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

        [Obsolete("The property is deprecated and setting it has not any effect. Use MaxSkewToDetect property instead.")]
        public double MaxBeta
        {
            get
            {
                return this.maxSkewToDetect;
            }
            set
            {
            }
        }

        public double MaxSkewToDetect
        {
            get
            {
                return this.maxSkewToDetect;
            }
            set
            {
                this.maxSkewToDetect = Math.Max(0.0, Math.Min(45.0, value));
                this.needToInitialize = true;
            }
        }

        [Obsolete("The property is deprecated and setting it has not any effect. Use MaxSkewToDetect property instead.")]
        public double MinBeta
        {
            get
            {
                return -this.maxSkewToDetect;
            }
            set
            {
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
                this.needToInitialize = true;
            }
        }
    }
}

