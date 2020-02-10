namespace AForge.Imaging
{
    using AForge.Math;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class HorizontalIntensityStatistics
    {
        private Histogram blue;
        private Histogram gray;
        private Histogram green;
        private Histogram red;

        public HorizontalIntensityStatistics(UnmanagedImage image)
        {
            if ((((image.PixelFormat != PixelFormat.Format8bppIndexed) && (image.PixelFormat != PixelFormat.Format16bppGrayScale)) && ((image.PixelFormat != PixelFormat.Format24bppRgb) && (image.PixelFormat != PixelFormat.Format32bppRgb))) && (((image.PixelFormat != PixelFormat.Format32bppArgb) && (image.PixelFormat != PixelFormat.Format48bppRgb)) && (image.PixelFormat != PixelFormat.Format64bppArgb)))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
            this.ProcessImage(image);
        }

        public HorizontalIntensityStatistics(Bitmap image)
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

        public HorizontalIntensityStatistics(BitmapData imageData) : this(new UnmanagedImage(imageData))
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
                    int[] numArray = new int[width];
                    for (int i = 0; i < height; i++)
                    {
                        int index = 0;
                        while (index < width)
                        {
                            numArray[index] += numPtr[0];
                            index++;
                            numPtr++;
                        }
                        numPtr += num3;
                    }
                    this.gray = new Histogram(numArray);
                    return;
                }
                case PixelFormat.Format16bppGrayScale:
                {
                    int num6 = (int) image.ImageData.ToPointer();
                    int stride = image.Stride;
                    int[] numArray2 = new int[width];
                    for (int j = 0; j < height; j++)
                    {
                        ushort* numPtr2 = (ushort*) (num6 + (stride * j));
                        int num9 = 0;
                        while (num9 < width)
                        {
                            numArray2[num9] += numPtr2[0];
                            num9++;
                            numPtr2++;
                        }
                    }
                    this.gray = new Histogram(numArray2);
                    return;
                }
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                {
                    byte* numPtr3 = (byte*) image.ImageData.ToPointer();
                    int num10 = (pixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
                    int num11 = image.Stride - (width * num10);
                    int[] numArray3 = new int[width];
                    int[] numArray4 = new int[width];
                    int[] numArray5 = new int[width];
                    for (int k = 0; k < height; k++)
                    {
                        int num13 = 0;
                        while (num13 < width)
                        {
                            numArray3[num13] += numPtr3[2];
                            numArray4[num13] += numPtr3[1];
                            numArray5[num13] += numPtr3[0];
                            num13++;
                            numPtr3 += num10;
                        }
                        numPtr3 += num11;
                    }
                    this.red = new Histogram(numArray3);
                    this.green = new Histogram(numArray4);
                    this.blue = new Histogram(numArray5);
                    return;
                }
                case PixelFormat.Format48bppRgb:
                case PixelFormat.Format64bppArgb:
                {
                    int num14 = (int) image.ImageData.ToPointer();
                    int num15 = image.Stride;
                    int num16 = (pixelFormat == PixelFormat.Format48bppRgb) ? 3 : 4;
                    int[] numArray6 = new int[width];
                    int[] numArray7 = new int[width];
                    int[] numArray8 = new int[width];
                    for (int m = 0; m < height; m++)
                    {
                        ushort* numPtr4 = (ushort*) (num14 + (num15 * m));
                        int num18 = 0;
                        while (num18 < width)
                        {
                            numArray6[num18] += numPtr4[2];
                            numArray7[num18] += numPtr4[1];
                            numArray8[num18] += numPtr4[0];
                            num18++;
                            numPtr4 += num16;
                        }
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

