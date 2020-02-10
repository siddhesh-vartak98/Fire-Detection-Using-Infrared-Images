namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;

    public class BottomHat : BaseInPlaceFilter
    {
        private Closing closing;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private Subtract subtract;

        public BottomHat()
        {
            this.closing = new Closing();
            this.subtract = new Subtract();
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            this.formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
        }

        public BottomHat(short[,] se) : this()
        {
            this.closing = new Closing(se);
        }

        protected override void ProcessFilter(UnmanagedImage image)
        {
            UnmanagedImage image2 = image.Clone();
            this.closing.ApplyInPlace(image);
            this.subtract.UnmanagedOverlayImage = image2;
            this.subtract.ApplyInPlace(image);
            image2.Dispose();
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

