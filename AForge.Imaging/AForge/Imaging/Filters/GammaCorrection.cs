namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class GammaCorrection : BaseInPlacePartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private double gamma;
        private byte[] table;

        public GammaCorrection() : this(2.2)
        {
        }

        public GammaCorrection(double gamma)
        {
            this.table = new byte[0x100];
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.Gamma = gamma;
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            int num = (image.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
            int num2 = rect.Left * num;
            int top = rect.Top;
            int num4 = num2 + (rect.Width * num);
            int num5 = top + rect.Height;
            int num6 = image.Stride - (rect.Width * num);
            byte* numPtr = (byte*) (image.ImageData.ToPointer() + ((top * image.Stride) + num2));
            for (int i = top; i < num5; i++)
            {
                int num8 = num2;
                while (num8 < num4)
                {
                    numPtr[0] = this.table[numPtr[0]];
                    num8++;
                    numPtr++;
                }
                numPtr += num6;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public double Gamma
        {
            get
            {
                return this.gamma;
            }
            set
            {
                this.gamma = Math.Max(0.1, Math.Min(5.0, value));
                double y = 1.0 / this.gamma;
                for (int i = 0; i < 0x100; i++)
                {
                    this.table[i] = (byte) Math.Min(0xff, (int) ((Math.Pow(((double) i) / 255.0, y) * 255.0) + 0.5));
                }
            }
        }
    }
}

