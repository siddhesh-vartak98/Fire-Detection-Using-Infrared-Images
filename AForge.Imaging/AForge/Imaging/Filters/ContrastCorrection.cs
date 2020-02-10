namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ContrastCorrection : BaseInPlacePartialFilter
    {
        private HSLLinear baseFilter;
        private double factor;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;

        public ContrastCorrection() : this(1.25)
        {
        }

        public ContrastCorrection(double factor)
        {
            this.baseFilter = new HSLLinear();
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.Factor = factor;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        protected override void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            this.baseFilter.ApplyInPlace(image, rect);
        }

        public double Factor
        {
            get
            {
                return this.factor;
            }
            set
            {
                this.factor = Math.Max(1E-06, value);
                this.baseFilter.InLuminance = new DoubleRange(0.0, 1.0);
                this.baseFilter.OutLuminance = new DoubleRange(0.0, 1.0);
                if (this.factor > 1.0)
                {
                    this.baseFilter.InLuminance = new DoubleRange(0.5 - (0.5 / this.factor), 0.5 + (0.5 / this.factor));
                }
                else
                {
                    this.baseFilter.OutLuminance = new DoubleRange(0.5 - (0.5 * this.factor), 0.5 + (0.5 * this.factor));
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

