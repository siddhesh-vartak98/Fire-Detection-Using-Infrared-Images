namespace AForge.Imaging
{
    using AForge;
    using AForge.Math;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ImageStatisticsYCbCr
    {
        private ContinuousHistogram cbHistogram;
        private ContinuousHistogram cbHistogramWithoutBlack;
        private ContinuousHistogram crHistogram;
        private ContinuousHistogram crHistogramWithoutBlack;
        private int pixels;
        private int pixelsWithoutBlack;
        private ContinuousHistogram yHistogram;
        private ContinuousHistogram yHistogramWithoutBlack;

        public ImageStatisticsYCbCr(UnmanagedImage image)
        {
            this.ProcessImage(image);
        }

        public ImageStatisticsYCbCr(Bitmap image)
        {
            this.CheckSourceFormat(image.PixelFormat);
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                this.ProcessImage(new UnmanagedImage(bitmapData));
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
        }

        public ImageStatisticsYCbCr(BitmapData imageData)
        {
            this.ProcessImage(new UnmanagedImage(imageData));
        }

        private void CheckSourceFormat(PixelFormat pixelFormat)
        {
            if (((pixelFormat != PixelFormat.Format24bppRgb) && (pixelFormat != PixelFormat.Format32bppRgb)) && (pixelFormat != PixelFormat.Format32bppArgb))
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
            int[] numArray = new int[0x100];
            int[] numArray2 = new int[0x100];
            int[] numArray3 = new int[0x100];
            int[] numArray4 = new int[0x100];
            int[] numArray5 = new int[0x100];
            int[] numArray6 = new int[0x100];
            RGB rgb = new RGB();
            YCbCr ycbcr = new YCbCr();
            int num3 = (image.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
            int num4 = image.Stride - (width * num3);
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            for (int i = 0; i < height; i++)
            {
                int num6 = 0;
                while (num6 < width)
                {
                    rgb.Red = numPtr[2];
                    rgb.Green = numPtr[1];
                    rgb.Blue = numPtr[0];
                    YCbCr.FromRGB(rgb, ycbcr);
                    numArray[(int) (ycbcr.Y * 255.0)]++;
                    numArray2[(int) ((ycbcr.Cb + 0.5) * 255.0)]++;
                    numArray3[(int) ((ycbcr.Cr + 0.5) * 255.0)]++;
                    this.pixels++;
                    if (((ycbcr.Y != 0.0) || (ycbcr.Cb != 0.0)) || (ycbcr.Cr != 0.0))
                    {
                        numArray4[(int) (ycbcr.Y * 255.0)]++;
                        numArray5[(int) ((ycbcr.Cb + 0.5) * 255.0)]++;
                        numArray6[(int) ((ycbcr.Cr + 0.5) * 255.0)]++;
                        this.pixelsWithoutBlack++;
                    }
                    num6++;
                    numPtr += num3;
                }
                numPtr += num4;
            }
            this.yHistogram = new ContinuousHistogram(numArray, new DoubleRange(0.0, 1.0));
            this.cbHistogram = new ContinuousHistogram(numArray2, new DoubleRange(-0.5, 0.5));
            this.crHistogram = new ContinuousHistogram(numArray3, new DoubleRange(-0.5, 0.5));
            this.yHistogramWithoutBlack = new ContinuousHistogram(numArray4, new DoubleRange(0.0, 1.0));
            this.cbHistogramWithoutBlack = new ContinuousHistogram(numArray5, new DoubleRange(-0.5, 0.5));
            this.crHistogramWithoutBlack = new ContinuousHistogram(numArray6, new DoubleRange(-0.5, 0.5));
        }

        public ContinuousHistogram Cb
        {
            get
            {
                return this.cbHistogram;
            }
        }

        public ContinuousHistogram CbWithoutBlack
        {
            get
            {
                return this.cbHistogramWithoutBlack;
            }
        }

        public ContinuousHistogram Cr
        {
            get
            {
                return this.crHistogram;
            }
        }

        public ContinuousHistogram CrWithoutBlack
        {
            get
            {
                return this.crHistogramWithoutBlack;
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

        public ContinuousHistogram Y
        {
            get
            {
                return this.yHistogram;
            }
        }

        public ContinuousHistogram YWithoutBlack
        {
            get
            {
                return this.yHistogramWithoutBlack;
            }
        }
    }
}

