namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class Morph : BaseInPlaceFilter2
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private double sourcePercent;

        public Morph()
        {
            this.sourcePercent = 0.5;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.InitFormatTranslations();
        }

        public Morph(UnmanagedImage unmanagedOverlayImage) : base(unmanagedOverlayImage)
        {
            this.sourcePercent = 0.5;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.InitFormatTranslations();
        }

        public Morph(Bitmap overlayImage) : base(overlayImage)
        {
            this.sourcePercent = 0.5;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.InitFormatTranslations();
        }

        private void InitFormatTranslations()
        {
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, UnmanagedImage overlay)
        {
            int width = image.Width;
            int height = image.Height;
            int num3 = (image.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
            int num4 = width * num3;
            int num5 = image.Stride - num4;
            int num6 = overlay.Stride - num4;
            double num7 = 1.0 - this.sourcePercent;
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            byte* numPtr2 = (byte*) overlay.ImageData.ToPointer();
            for (int i = 0; i < height; i++)
            {
                int num9 = 0;
                while (num9 < num4)
                {
                    numPtr[0] = (byte) ((this.sourcePercent * numPtr[0]) + (num7 * numPtr2[0]));
                    num9++;
                    numPtr++;
                    numPtr2++;
                }
                numPtr += num5;
                numPtr2 += num6;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public double SourcePercent
        {
            get
            {
                return this.sourcePercent;
            }
            set
            {
                this.sourcePercent = Math.Max(0.0, Math.Min(1.0, value));
            }
        }
    }
}

