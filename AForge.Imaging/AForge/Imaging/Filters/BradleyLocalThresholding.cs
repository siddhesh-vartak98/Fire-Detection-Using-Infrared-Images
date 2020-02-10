namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;

    public class BradleyLocalThresholding : BaseInPlaceFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
        private float pixelBrightnessDifferenceLimit = 0.15f;
        private int windowSize = 0x29;

        public BradleyLocalThresholding()
        {
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image)
        {
            IntegralImage image2 = IntegralImage.FromBitmap(image);
            int width = image.Width;
            int height = image.Height;
            int num3 = width - 1;
            int num4 = height - 1;
            int num5 = image.Stride - width;
            int num6 = this.windowSize / 2;
            float num7 = 1f - this.pixelBrightnessDifferenceLimit;
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            for (int i = 0; i < height; i++)
            {
                int num9 = i - num6;
                int num10 = i + num6;
                if (num9 < 0)
                {
                    num9 = 0;
                }
                if (num10 > num4)
                {
                    num10 = num4;
                }
                int num11 = 0;
                while (num11 < width)
                {
                    int num12 = num11 - num6;
                    int num13 = num11 + num6;
                    if (num12 < 0)
                    {
                        num12 = 0;
                    }
                    if (num13 > num3)
                    {
                        num13 = num3;
                    }
                    numPtr[0] = (numPtr[0] < ((int) (image2.GetRectangleMeanUnsafe(num12, num9, num13, num10) * num7))) ? ((byte) 0) : ((byte) 0xff);
                    num11++;
                    numPtr++;
                }
                numPtr += num5;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public float PixelBrightnessDifferenceLimit
        {
            get
            {
                return this.pixelBrightnessDifferenceLimit;
            }
            set
            {
                this.pixelBrightnessDifferenceLimit = Math.Max(0f, Math.Min(1f, value));
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
                this.windowSize = Math.Max(3, value | 1);
            }
        }
    }
}

