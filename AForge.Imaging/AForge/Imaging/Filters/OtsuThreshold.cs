namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class OtsuThreshold : BaseInPlacePartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
        private Threshold thresholdFilter = new Threshold();

        public OtsuThreshold()
        {
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        public unsafe int CalculateThreshold(UnmanagedImage image, Rectangle rect)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new UnsupportedImageFormatException("Source pixel format is not supported by the routine.");
            }
            int num = 0;
            int left = rect.Left;
            int top = rect.Top;
            int num4 = left + rect.Width;
            int num5 = top + rect.Height;
            int num6 = image.Stride - rect.Width;
            int[] numArray = new int[0x100];
            double[] numArray2 = new double[0x100];
            byte* numPtr = (byte*) (image.ImageData.ToPointer() + ((top * image.Stride) + left));
            for (int i = top; i < num5; i++)
            {
                int num8 = left;
                while (num8 < num4)
                {
                    numArray[numPtr[0]]++;
                    num8++;
                    numPtr++;
                }
                numPtr += num6;
            }
            int num9 = (num4 - left) * (num5 - top);
            double num10 = 0.0;
            for (int j = 0; j < 0x100; j++)
            {
                numArray2[j] = ((double) numArray[j]) / ((double) num9);
                num10 += numArray2[j] * j;
            }
            double minValue = double.MinValue;
            double num13 = 0.0;
            double num14 = 1.0;
            double num15 = 0.0;
            for (int k = 0; k < 0x100; k++)
            {
                double num17 = num13;
                double num18 = num14;
                double num19 = num15;
                double num20 = (num10 - (num19 * num17)) / num18;
                double num21 = (num17 * (1.0 - num17)) * Math.Pow(num19 - num20, 2.0);
                if (num21 > minValue)
                {
                    minValue = num21;
                    num = k;
                }
                num15 *= num13;
                num13 += numArray2[k];
                num14 -= numArray2[k];
                num15 += k * numArray2[k];
                if (num13 != 0.0)
                {
                    num15 /= num13;
                }
            }
            return num;
        }

        public int CalculateThreshold(Bitmap image, Rectangle rect)
        {
            int num = 0;
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            try
            {
                num = this.CalculateThreshold(data, rect);
            }
            finally
            {
                image.UnlockBits(data);
            }
            return num;
        }

        public int CalculateThreshold(BitmapData image, Rectangle rect)
        {
            return this.CalculateThreshold(new UnmanagedImage(image), rect);
        }

        protected override void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            this.thresholdFilter.ThresholdValue = this.CalculateThreshold(image, rect);
            this.thresholdFilter.ApplyInPlace(image, rect);
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public int ThresholdValue
        {
            get
            {
                return this.thresholdFilter.ThresholdValue;
            }
        }
    }
}

