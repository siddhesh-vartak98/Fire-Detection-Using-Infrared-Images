namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;

    public sealed class GrayscaleToRGB : BaseFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        public GrayscaleToRGB()
        {
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format24bppRgb;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            int width = sourceData.Width;
            int height = sourceData.Height;
            int num3 = sourceData.Stride - width;
            int num4 = destinationData.Stride - (width * 3);
            byte* numPtr = (byte*) sourceData.ImageData.ToPointer();
            byte* numPtr2 = (byte*) destinationData.ImageData.ToPointer();
            for (int i = 0; i < height; i++)
            {
                int num6 = 0;
                while (num6 < width)
                {
                    byte num7;
                    numPtr2[0] = num7 = numPtr[0];
                    numPtr2[2] = numPtr2[1] = num7;
                    num6++;
                    numPtr++;
                    numPtr2 += 3;
                }
                numPtr += num3;
                numPtr2 += num4;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }
    }
}

