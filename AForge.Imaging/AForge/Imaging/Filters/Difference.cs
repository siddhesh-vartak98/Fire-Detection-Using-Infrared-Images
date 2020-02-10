namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public sealed class Difference : BaseInPlaceFilter2
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;

        public Difference()
        {
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.InitFormatTranslations();
        }

        public Difference(UnmanagedImage unmanagedOverlayImage) : base(unmanagedOverlayImage)
        {
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.InitFormatTranslations();
        }

        public Difference(Bitmap overlayImage) : base(overlayImage)
        {
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.InitFormatTranslations();
        }

        private void InitFormatTranslations()
        {
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            this.formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            this.formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
            this.formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, UnmanagedImage overlay)
        {
            int num3;
            PixelFormat pixelFormat = image.PixelFormat;
            int width = image.Width;
            int height = image.Height;
            switch (pixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                {
                    int num4 = (pixelFormat == PixelFormat.Format8bppIndexed) ? 1 : ((pixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4);
                    int num5 = width * num4;
                    int num6 = image.Stride - num5;
                    int num7 = overlay.Stride - num5;
                    byte* numPtr = (byte*) image.ImageData.ToPointer();
                    byte* numPtr2 = (byte*) overlay.ImageData.ToPointer();
                    for (int j = 0; j < height; j++)
                    {
                        int num9 = 0;
                        while (num9 < num5)
                        {
                            num3 = numPtr[0] - numPtr2[0];
                            numPtr[0] = (num3 < 0) ? ((byte) -num3) : ((byte) num3);
                            num9++;
                            numPtr++;
                            numPtr2++;
                        }
                        numPtr += num6;
                        numPtr2 += num7;
                    }
                    return;
                }
            }
            int num10 = (pixelFormat == PixelFormat.Format16bppGrayScale) ? 1 : ((pixelFormat == PixelFormat.Format48bppRgb) ? 3 : 4);
            int num11 = width * num10;
            int stride = image.Stride;
            int num13 = overlay.Stride;
            int num14 = (int) image.ImageData.ToPointer();
            int num15 = (int) overlay.ImageData.ToPointer();
            for (int i = 0; i < height; i++)
            {
                ushort* numPtr3 = (ushort*) (num14 + (i * stride));
                ushort* numPtr4 = (ushort*) (num15 + (i * num13));
                int num17 = 0;
                while (num17 < num11)
                {
                    num3 = numPtr3[0] - numPtr4[0];
                    numPtr3[0] = (num3 < 0) ? ((ushort) -num3) : ((ushort) num3);
                    num17++;
                    numPtr3++;
                    numPtr4++;
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

