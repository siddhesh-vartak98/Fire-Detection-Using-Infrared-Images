namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using AForge.Math.Random;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class AdditiveNoise : BaseInPlacePartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private IRandomNumberGenerator generator;

        public AdditiveNoise()
        {
            this.generator = new UniformGenerator(new DoubleRange(-10.0, 10.0));
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
        }

        public AdditiveNoise(IRandomNumberGenerator generator) : this()
        {
            this.generator = generator;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            int num = (image.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
            int top = rect.Top;
            int num3 = top + rect.Height;
            int num4 = rect.Left * num;
            int num5 = num4 + (rect.Width * num);
            int num6 = image.Stride - (num5 - num4);
            byte* numPtr = (byte*) (image.ImageData.ToPointer() + ((top * image.Stride) + (rect.Left * num)));
            for (int i = top; i < num3; i++)
            {
                int num8 = num4;
                while (num8 < num5)
                {
                    numPtr[0] = (byte) Math.Max(0.0, Math.Min((double) 255.0, (double) (numPtr[0] + this.generator.Next())));
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

        public IRandomNumberGenerator Generator
        {
            get
            {
                return this.generator;
            }
            set
            {
                this.generator = value;
            }
        }
    }
}

