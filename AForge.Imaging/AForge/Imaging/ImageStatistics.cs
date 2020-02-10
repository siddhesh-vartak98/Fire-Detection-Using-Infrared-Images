namespace AForge.Imaging
{
    using AForge.Math;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ImageStatistics
    {
        private Histogram blue;
        private Histogram blueWithoutBlack;
        private Histogram gray;
        private bool grayscale;
        private Histogram grayWithoutBlack;
        private Histogram green;
        private Histogram greenWithoutBlack;
        private int pixels;
        private int pixelsWithoutBlack;
        private Histogram red;
        private Histogram redWithoutBlack;

        public ImageStatistics(UnmanagedImage image)
        {
            this.ProcessImage(image);
        }

        public ImageStatistics(Bitmap image)
        {
            this.CheckSourceFormat(image.PixelFormat);
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

        public ImageStatistics(BitmapData imageData)
        {
            this.ProcessImage(new UnmanagedImage(imageData));
        }

        private void CheckSourceFormat(PixelFormat pixelFormat)
        {
            if (((pixelFormat != PixelFormat.Format8bppIndexed) && (pixelFormat != PixelFormat.Format24bppRgb)) && ((pixelFormat != PixelFormat.Format32bppRgb) && (pixelFormat != PixelFormat.Format32bppArgb)))
            {
                throw new UnsupportedImageFormatException("Source pixel format is not supported.");
            }
        }

        private unsafe void ProcessImage(UnmanagedImage image)
        {
            this.CheckSourceFormat(image.PixelFormat);
            int width = image.Width;
            int height = image.Height;
            this.pixels = this.pixelsWithoutBlack = 0;
            if (this.grayscale = image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                int[] numArray = new int[0x100];
                int[] numArray2 = new int[0x100];
                int num4 = image.Stride - width;
                byte* numPtr = (byte*) image.ImageData.ToPointer();
                for (int i = 0; i < height; i++)
                {
                    int num6 = 0;
                    while (num6 < width)
                    {
                        byte index = numPtr[0];
                        numArray[index]++;
                        this.pixels++;
                        if (index != 0)
                        {
                            numArray2[index]++;
                            this.pixelsWithoutBlack++;
                        }
                        num6++;
                        numPtr++;
                    }
                    numPtr += num4;
                }
                this.gray = new Histogram(numArray);
                this.grayWithoutBlack = new Histogram(numArray2);
            }
            else
            {
                int[] numArray3 = new int[0x100];
                int[] numArray4 = new int[0x100];
                int[] numArray5 = new int[0x100];
                int[] numArray6 = new int[0x100];
                int[] numArray7 = new int[0x100];
                int[] numArray8 = new int[0x100];
                int num10 = (image.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
                int num11 = image.Stride - (width * num10);
                byte* numPtr2 = (byte*) image.ImageData.ToPointer();
                for (int j = 0; j < height; j++)
                {
                    int num13 = 0;
                    while (num13 < width)
                    {
                        byte num7 = numPtr2[2];
                        byte num8 = numPtr2[1];
                        byte num9 = numPtr2[0];
                        numArray3[num7]++;
                        numArray4[num8]++;
                        numArray5[num9]++;
                        this.pixels++;
                        if (((num7 != 0) || (num8 != 0)) || (num9 != 0))
                        {
                            numArray6[num7]++;
                            numArray7[num8]++;
                            numArray8[num9]++;
                            this.pixelsWithoutBlack++;
                        }
                        num13++;
                        numPtr2 += num10;
                    }
                    numPtr2 += num11;
                }
                this.red = new Histogram(numArray3);
                this.green = new Histogram(numArray4);
                this.blue = new Histogram(numArray5);
                this.redWithoutBlack = new Histogram(numArray6);
                this.greenWithoutBlack = new Histogram(numArray7);
                this.blueWithoutBlack = new Histogram(numArray8);
            }
        }

        public Histogram Blue
        {
            get
            {
                return this.blue;
            }
        }

        public Histogram BlueWithoutBlack
        {
            get
            {
                return this.blueWithoutBlack;
            }
        }

        public Histogram Gray
        {
            get
            {
                return this.gray;
            }
        }

        public Histogram GrayWithoutBlack
        {
            get
            {
                return this.grayWithoutBlack;
            }
        }

        public Histogram Green
        {
            get
            {
                return this.green;
            }
        }

        public Histogram GreenWithoutBlack
        {
            get
            {
                return this.greenWithoutBlack;
            }
        }

        public bool IsGrayscale
        {
            get
            {
                return this.grayscale;
            }
        }

        public int PixelsCount
        {
            get
            {
                return this.pixels;
            }
        }

        public int PixelsCountWithoutBlack
        {
            get
            {
                return this.pixelsWithoutBlack;
            }
        }

        public Histogram Red
        {
            get
            {
                return this.red;
            }
        }

        public Histogram RedWithoutBlack
        {
            get
            {
                return this.redWithoutBlack;
            }
        }
    }
}

