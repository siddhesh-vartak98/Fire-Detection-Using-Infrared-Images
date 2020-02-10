namespace AForge.Imaging
{
    using AForge;
    using AForge.Math;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ImageStatisticsHSL
    {
        private ContinuousHistogram luminance;
        private ContinuousHistogram luminanceWithoutBlack;
        private int pixels;
        private int pixelsWithoutBlack;
        private ContinuousHistogram saturation;
        private ContinuousHistogram saturationWithoutBlack;

        public ImageStatisticsHSL(UnmanagedImage image)
        {
            this.ProcessImage(image);
        }

        public ImageStatisticsHSL(Bitmap image)
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

        public ImageStatisticsHSL(BitmapData imageData)
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
            RGB rgb = new RGB();
            HSL hsl = new HSL();
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
                    HSL.FromRGB(rgb, hsl);
                    numArray[(int) (hsl.Saturation * 255.0)]++;
                    numArray2[(int) (hsl.Luminance * 255.0)]++;
                    this.pixels++;
                    if (hsl.Luminance != 0.0)
                    {
                        numArray3[(int) (hsl.Saturation * 255.0)]++;
                        numArray4[(int) (hsl.Luminance * 255.0)]++;
                        this.pixelsWithoutBlack++;
                    }
                    num6++;
                    numPtr += num3;
                }
                numPtr += num4;
            }
            this.saturation = new ContinuousHistogram(numArray, new DoubleRange(0.0, 1.0));
            this.luminance = new ContinuousHistogram(numArray2, new DoubleRange(0.0, 1.0));
            this.saturationWithoutBlack = new ContinuousHistogram(numArray3, new DoubleRange(0.0, 1.0));
            this.luminanceWithoutBlack = new ContinuousHistogram(numArray4, new DoubleRange(0.0, 1.0));
        }

        public ContinuousHistogram Luminance
        {
            get
            {
                return this.luminance;
            }
        }

        public ContinuousHistogram LuminanceWithoutBlack
        {
            get
            {
                return this.luminanceWithoutBlack;
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

        public ContinuousHistogram Saturation
        {
            get
            {
                return this.saturation;
            }
        }

        public ContinuousHistogram SaturationWithoutBlack
        {
            get
            {
                return this.saturationWithoutBlack;
            }
        }
    }
}

