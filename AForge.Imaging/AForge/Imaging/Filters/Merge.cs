namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public sealed class Merge : BaseInPlaceFilter2
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;

        public Merge()
        {
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.InitFormatTranslations();
        }

        public Merge(UnmanagedImage unmanagedOverlayImage) : base(unmanagedOverlayImage)
        {
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.InitFormatTranslations();
        }

        public Merge(Bitmap overlayImage) : base(overlayImage)
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
                    int num3 = (pixelFormat == PixelFormat.Format8bppIndexed) ? 1 : ((pixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4);
                    int num4 = width * num3;
                    int num5 = image.Stride - num4;
                    int num6 = overlay.Stride - num4;
                    byte* numPtr = (byte*) image.ImageData.ToPointer();
                    byte* numPtr2 = (byte*) overlay.ImageData.ToPointer();
                    for (int j = 0; j < height; j++)
                    {
                        int num8 = 0;
                        while (num8 < num4)
                        {
                            if (numPtr2[0] > numPtr[0])
                            {
                                numPtr[0] = numPtr2[0];
                            }
                            num8++;
                            numPtr++;
                            numPtr2++;
                        }
                        numPtr += num5;
                        numPtr2 += num6;
                    }
                    return;
                }
            }
            int num9 = (pixelFormat == PixelFormat.Format16bppGrayScale) ? 1 : ((pixelFormat == PixelFormat.Format48bppRgb) ? 3 : 4);
            int num10 = width * num9;
            int stride = image.Stride;
            int num12 = overlay.Stride;
            int num13 = (int) image.ImageData.ToPointer();
            int num14 = (int) overlay.ImageData.ToPointer();
            for (int i = 0; i < height; i++)
            {
                ushort* numPtr3 = (ushort*) (num13 + (i * stride));
                ushort* numPtr4 = (ushort*) (num14 + (i * num12));
                int num16 = 0;
                while (num16 < num10)
                {
                    if (numPtr4[0] > numPtr3[0])
                    {
                        numPtr3[0] = numPtr4[0];
                    }
                    num16++;
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

