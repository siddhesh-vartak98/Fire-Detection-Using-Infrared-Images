namespace AForge.Imaging
{
    using AForge;
    using AForge.Imaging.Filters;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class SusanCornersDetector : ICornersDetector
    {
        private int differenceThreshold;
        private int geometricalThreshold;
        private static int[] rowRadius = new int[] { 1, 2, 3, 3, 3, 2, 1 };

        public SusanCornersDetector()
        {
            this.differenceThreshold = 0x19;
            this.geometricalThreshold = 0x12;
        }

        public SusanCornersDetector(int differenceThreshold, int geometricalThreshold)
        {
            this.differenceThreshold = 0x19;
            this.geometricalThreshold = 0x12;
            this.differenceThreshold = differenceThreshold;
            this.geometricalThreshold = geometricalThreshold;
        }

        public unsafe List<IntPoint> ProcessImage(UnmanagedImage image)
        {
            if (((image.PixelFormat != PixelFormat.Format8bppIndexed) && (image.PixelFormat != PixelFormat.Format24bppRgb)) && ((image.PixelFormat != PixelFormat.Format32bppRgb) && (image.PixelFormat != PixelFormat.Format32bppArgb)))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
            int width = image.Width;
            int height = image.Height;
            UnmanagedImage image2 = null;
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                image2 = image;
            }
            else
            {
                image2 = Grayscale.CommonAlgorithms.BT709.Apply(image);
            }
            int[,] numArray = new int[height, width];
            int stride = image2.Stride;
            int num4 = stride - width;
            byte* numPtr = (byte*) ((image2.ImageData.ToPointer() + (stride * 3)) + 3);
            int num5 = 3;
            int num6 = height - 3;
            while (num5 < num6)
            {
                int num7 = 3;
                int num8 = width - 3;
                while (num7 < num8)
                {
                    byte num9 = numPtr[0];
                    int num10 = 0;
                    int num11 = 0;
                    int num12 = 0;
                    for (int i = -3; i <= 3; i++)
                    {
                        int num14 = rowRadius[i + 3];
                        byte* numPtr2 = numPtr + (stride * i);
                        for (int j = -num14; j <= num14; j++)
                        {
                            if (Math.Abs((int) (num9 - numPtr2[j])) <= this.differenceThreshold)
                            {
                                num10++;
                                num11 += num7 + j;
                                num12 += num5 + i;
                            }
                        }
                    }
                    if (num10 < this.geometricalThreshold)
                    {
                        num11 /= num10;
                        num12 /= num10;
                        if ((num7 != num11) || (num5 != num12))
                        {
                            num10 = this.geometricalThreshold - num10;
                        }
                        else
                        {
                            num10 = 0;
                        }
                    }
                    else
                    {
                        num10 = 0;
                    }
                    numArray[num5, num7] = num10;
                    num7++;
                    numPtr++;
                }
                numPtr += 6 + num4;
                num5++;
            }
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                image2.Dispose();
            }
            List<IntPoint> list = new List<IntPoint>();
            int y = 2;
            int num17 = height - 2;
            while (y < num17)
            {
                int x = 2;
                int num19 = width - 2;
                while (x < num19)
                {
                    int num20 = numArray[y, x];
                    for (int k = -2; (num20 != 0) && (k <= 2); k++)
                    {
                        for (int m = -2; m <= 2; m++)
                        {
                            if (numArray[y + k, x + m] > num20)
                            {
                                num20 = 0;
                                break;
                            }
                        }
                    }
                    if (num20 != 0)
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

        public int DifferenceThreshold
        {
            get
            {
                return this.differenceThreshold;
            }
            set
            {
                this.differenceThreshold = value;
            }
        }

        public int GeometricalThreshold
        {
            get
            {
                return this.geometricalThreshold;
            }
            set
            {
                this.geometricalThreshold = value;
            }
        }
    }
}

