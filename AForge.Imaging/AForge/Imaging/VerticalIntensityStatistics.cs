namespace AForge.Imaging
{
    using AForge.Math;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class VerticalIntensityStatistics
    {
        private Histogram blue;
        private Histogram gray;
        private Histogram green;
        private Histogram red;

        public VerticalIntensityStatistics(UnmanagedImage image)
        {
            if ((((image.PixelFormat != PixelFormat.Format8bppIndexed) && (image.PixelFormat != PixelFormat.Format16bppGrayScale)) && ((image.PixelFormat != PixelFormat.Format24bppRgb) && (image.PixelFormat != PixelFormat.Format32bppRgb))) && (((image.PixelFormat != PixelFormat.Format32bppArgb) && (image.PixelFormat != PixelFormat.Format48bppRgb)) && (image.PixelFormat != PixelFormat.Format64bppArgb)))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
            this.ProcessImage(image);
        }

        public VerticalIntensityStatistics(Bitmap image)
        {
            if ((((image.PixelFormat != PixelFormat.Format8bppIndexed) && (image.PixelFormat != PixelFormat.Format16bppGrayScale)) && ((image.PixelFormat != PixelFormat.Format24bppRgb) && (image.PixelFormat != PixelFormat.Format32bppRgb))) && (((image.PixelFormat != PixelFormat.Format32bppArgb) && (image.PixelFormat != PixelFormat.Format48bppRgb)) && (image.PixelFormat != PixelFormat.Format64bppArgb)))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            try
            {
                this.ProcessImage(new UnmanagedImage(bitmapData));
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
        }

        public VerticalIntensityStatistics(BitmapData imageData) : this(new UnmanagedImage(imageData))
        {
        }

        private unsafe void ProcessImage(UnmanagedImage image)
        {
            PixelFormat pixelFormat = image.PixelFormat;
            int width = image.Width;
            int height = image.Height;
            switch (pixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                {
                    byte* numPtr = (byte*) image.ImageData.ToPointer();
                    int num3 = image.Stride - width;
                    int[] numArray = new int[height];
                    for (int i = 0; i < height; i++)
                    {
                        int num5 = 0;
                        int num6 = 0;
                        while (num6 < width)
                        {
                            num5 += numPtr[0];
                            num6++;
                            numPtr++;
                        }
                        numArray[i] = num5;
                        numPtr += num3;
                    }
                    this.gray = new Histogram(numArray);
                    return;
                }
                case PixelFormat.Format16bppGrayScale:
                {
                    int num7 = (int) image.ImageData.ToPointer();
                    int stride = image.Stride;
                    int[] numArray2 = new int[height];
                    for (int j = 0; j < height; j++)
                    {
                        ushort* numPtr2 = (ushort*) (num7 + (stride * j));
                        int num10 = 0;
                        int num11 = 0;
                        while (num11 < width)
                        {
                            num10 += numPtr2[0];
                            num11++;
                            numPtr2++;
                        }
                        numArray2[j] = num10;
                    }
                    this.gray = new Histogram(numArray2);
                    return;
                }
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                {
                    byte* numPtr3 = (byte*) image.ImageData.ToPointer();
                    int num12 = (pixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
                    int num13 = image.Stride - (width * num12);
                    int[] numArray3 = new int[height];
                    int[] numArray4 = new int[height];
                    int[] numArray5 = new int[height];
                    for (int k = 0; k < height; k++)
                    {
                        int num15 = 0;
                        int num16 = 0;
                        int num17 = 0;
                        int num18 = 0;
                        while (num18 < width)
                        {
                            num15 += numPtr3[2];
                            num16 += numPtr3[1];
                            num17 += numPtr3[0];
                            num18++;
                            numPtr3 += num12;
                        }
                        numArray3[k] = num15;
                        numArray4[k] = num16;
                        numArray5[k] = num17;
                        numPtr3 += num13;
                    }
                    this.red = new Histogram(numArray3);
                    this.green = new Histogram(numArray4);
                    this.blue = new Histogram(numArray5);
                    return;
                }
                case PixelFormat.Format48bppRgb:
                case PixelFormat.Format64bppArgb:
                {
                    int num19 = (int) image.ImageData.ToPointer();
                    int num20 = image.Stride;
                    int num21 = (pixelFormat == PixelFormat.Format48bppRgb) ? 3 : 4;
                    int[] numArray6 = new int[height];
                    int[] numArray7 = new int[height];
                    int[] numArray8 = new int[height];
                    for (int m = 0; m < height; m++)
                    {
                        ushort* numPtr4 = (ushort*) (num19 + (num20 * m));
                        int num23 = 0;
                        int num24 = 0;
                        int num25 = 0;
                        int num26 = 0;
                        while (num26 < width)
                        {
                            num23 += numPtr4[2];
                            num24 += numPtr4[1];
                            num25 += numPtr4[0];
                            num26++;
                            numPtr4 += num21;
                        }
                        numArray6[m] = num23;
                        numArray7[m] = num24;
                        numArray8[m] = num25;
                    }
                    this.red = new Histogram(numArray6);
                    this.green = new Histogram(numArray7);
                    this.blue = new Histogram(numArray8);
                    break;
                }
            }
        }

        public Histogram Blue
        {
            get
            {
                return this.blue;
            }
        }

        public Histogram Gray
        {
            get
            {
                return this.gray;
            }
        }

        public Histogram Green
        {
            get
            {
                return this.green;
            }
        }

        public bool IsGrayscale
        {
            get
            {
                return (this.gray == null);
            }
        }

        public Histogram Red
        {
            get
            {
                return this.red;
            }
        }
    }
}

