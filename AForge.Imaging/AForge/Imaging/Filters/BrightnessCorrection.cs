namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class BrightnessCorrection : BaseInPlacePartialFilter
    {
        private double adjustValue;
        private HSLLinear baseFilter;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;

        public BrightnessCorrection() : this(0.1)
        {
        }

        public BrightnessCorrection(double adjustValue)
        {
            this.baseFilter = new HSLLinear();
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.AdjustValue = adjustValue;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        protected override void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            this.baseFilter.ApplyInPlace(image, rect);
        }

        public double AdjustValue
        {
            get
            {
                return this.adjustValue;
            }
            set
            {
                this.adjustValue = Math.Max(-1.0, Math.Min(1.0, value));
                if (this.adjustValue > 0.0)
                {
                    this.baseFilter.InLuminance = new DoubleRange(0.0, 1.0 - this.adjustValue);
                    this.baseFilter.OutLuminance = new DoubleRange(this.adjustValue, 1.0);
                }
                else
                {
                    this.baseFilter.InLuminance = new DoubleRange(-this.adjustValue, 1.0);
                    this.baseFilter.OutLuminance = new DoubleRange(0.0, 1.0 + this.adjustValue);
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

