namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class CornersMarker : BaseInPlaceFilter
    {
        private ICornersDetector detector;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private Color markerColor;

        public CornersMarker(ICornersDetector detector) : this(detector, Color.White)
        {
        }

        public CornersMarker(ICornersDetector detector, Color markerColor)
        {
            this.markerColor = Color.White;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.detector = detector;
            this.markerColor = markerColor;
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        protected override void ProcessFilter(UnmanagedImage image)
        {
            foreach (IntPoint point in this.detector.ProcessImage(image))
            {
                Drawing.FillRectangle(image, new Rectangle(point.X - 1, point.Y - 1, 3, 3), this.markerColor);
            }
        }

        public ICornersDetector Detector
        {
            get
            {
                return this.detector;
            }
            set
            {
                this.detector = value;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public Color MarkerColor
        {
            get
            {
                return this.markerColor;
            }
            set
            {
                this.markerColor = value;
            }
        }
    }
}

