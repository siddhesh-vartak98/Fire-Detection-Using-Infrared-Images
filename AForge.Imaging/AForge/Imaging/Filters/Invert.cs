namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public sealed class Invert : BaseInPlacePartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        public Invert()
        {
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            this.formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            int num = ((image.PixelFormat == PixelFormat.Format8bppIndexed) || (image.PixelFormat == PixelFormat.Format16bppGrayScale)) ? 1 : 3;
            int top = rect.Top;
            int num3 = top + rect.Height;
            int num4 = rect.Left * num;
            int num5 = num4 + (rect.Width * num);
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            if ((image.PixelFormat == PixelFormat.Format8bppIndexed) || (image.PixelFormat == PixelFormat.Format24bppRgb))
            {
                int num6 = image.Stride - (num5 - num4);
                byte* numPtr2 = numPtr + ((top * image.Stride) + (rect.Left * num));
                for (int i = top; i < num3; i++)
                {
                    int num8 = num4;
                    while (num8 < num5)
                    {
                        numPtr2[0] = (byte) (0xff - numPtr2[0]);
                        num8++;
                        numPtr2++;
                    }
                    numPtr2 += num6;
                }
            }
            else
            {
                int stride = image.Stride;
                numPtr += (top * image.Stride) + ((rect.Left * num) * 2);
                for (int j = top; j < num3; j++)
                {
                    ushort* numPtr3 = (ushort*) numPtr;
                    int num11 = num4;
                    while (num11 < num5)
                    {
                        numPtr3[0] = (ushort) (0xffff - numPtr3[0]);
                        num11++;
                        numPtr3++;
                    }
                    numPtr += stride;
                }
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

