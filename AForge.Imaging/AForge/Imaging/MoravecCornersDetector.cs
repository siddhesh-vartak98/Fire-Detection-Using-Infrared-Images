namespace AForge.Imaging
{
    using AForge;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class MoravecCornersDetector : ICornersDetector
    {
        private int threshold;
        private int windowSize;
        private static int[] xDelta = new int[] { -1, 0, 1, 1, 1, 0, -1, -1 };
        private static int[] yDelta = new int[] { -1, -1, -1, 0, 1, 1, 1, 0 };

        public MoravecCornersDetector()
        {
            this.windowSize = 3;
            this.threshold = 500;
        }

        public MoravecCornersDetector(int threshold) : this(threshold, 3)
        {
        }

        public MoravecCornersDetector(int threshold, int windowSize)
        {
            this.windowSize = 3;
            this.threshold = 500;
            this.Threshold = threshold;
            this.WindowSize = windowSize;
        }

        public unsafe List<IntPoint> ProcessImage(UnmanagedImage image)
        {
            if (((image.PixelFormat != PixelFormat.Format8bppIndexed) && (image.PixelFormat != PixelFormat.Format24bppRgb)) && ((image.PixelFormat != PixelFormat.Format32bppRgb) && (image.PixelFormat != PixelFormat.Format32bppArgb)))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int num4 = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int num5 = this.windowSize / 2;
            int num6 = stride - (this.windowSize * num4);
            int[,] numArray = new int[height, width];
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            int num7 = num5;
            int num8 = height - num5;
            while (num7 < num8)
            {
                int num9 = num5;
                int num10 = width - num5;
                while (num9 < num10)
                {
                    int num11 = 0x7fffffff;
                    for (int i = 0; i < 8; i++)
                    {
                        int num13 = num7 + yDelta[i];
                        int num14 = num9 + xDelta[i];
                        if (((num13 >= num5) && (num13 < num8)) && ((num14 >= num5) && (num14 < num10)))
                        {
                            int num15 = 0;
                            byte* numPtr2 = (numPtr + ((num7 - num5) * stride)) + ((num9 - num5) * num4);
                            byte* numPtr3 = (numPtr + ((num13 - num5) * stride)) + ((num14 - num5) * num4);
                            for (int j = 0; j < this.windowSize; j++)
                            {
                                int num17 = 0;
                                int num18 = this.windowSize * num4;
                                while (num17 < num18)
                                {
                                    int num19 = numPtr2[0] - numPtr3[0];
                                    num15 += num19 * num19;
                                    num17++;
                                    numPtr2++;
                                    numPtr3++;
                                }
                                numPtr2 += num6;
                                numPtr3 += num6;
                            }
                            if (num15 < num11)
                            {
                                num11 = num15;
                            }
                        }
                    }
                    if (num11 < this.threshold)
                    {
                        num11 = 0;
                    }
                    numArray[num7, num9] = num11;
                    num9++;
                }
                num7++;
            }
            List<IntPoint> list = new List<IntPoint>();
            int y = num5;
            int num21 = height - num5;
            while (y < num21)
            {
                int x = num5;
                int num23 = width - num5;
                while (x < num23)
                {
                    int num24 = numArray[y, x];
                    for (int k = -num5; (num24 != 0) && (k <= num5); k++)
                    {
                        for (int m = -num5; m <= num5; m++)
                        {
                            if (numArray[y + k, x + m] > num24)
                            {
                                num24 = 0;
                                break;
                            }
                        }
                    }
                    if (num24 != 0)
                    {
                        list.Add(new IntPoint(x, y));
                    }
                    x++;
                }
                y++;
            }
            return list;
        }

        public List<IntPoint> ProcessImage(Bitmap image)
        {
            List<IntPoint> list;
            if (((image.PixelFormat != PixelFormat.Format8bppIndexed) && (image.PixelFormat != PixelFormat.Format24bppRgb)) && ((image.PixelFormat != PixelFormat.Format32bppRgb) && (image.PixelFormat != PixelFormat.Format32bppArgb)))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            try
            {
                list = this.ProcessImage(new UnmanagedImage(bitmapData));
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
            return list;
        }

        public List<IntPoint> ProcessImage(BitmapData imageData)
        {
            return this.ProcessImage(new UnmanagedImage(imageData));
        }

        public int Threshold
        {
            get
            {
                return this.threshold;
            }
            set
            {
                this.threshold = value;
            }
        }

        public int WindowSize
        {
            get
            {
                return this.windowSize;
            }
            set
            {
                if ((value & 1) == 0)
                {
                    throw new ArgumentException("The value shoule be odd.");
                }
                this.windowSize = Math.Max(3, Math.Min(15, value));
            }
        }
    }
}

