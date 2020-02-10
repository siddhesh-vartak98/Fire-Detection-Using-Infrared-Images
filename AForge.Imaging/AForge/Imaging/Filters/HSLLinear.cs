namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class HSLLinear : BaseInPlacePartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
        private DoubleRange inLuminance = new DoubleRange(0.0, 1.0);
        private DoubleRange inSaturation = new DoubleRange(0.0, 1.0);
        private DoubleRange outLuminance = new DoubleRange(0.0, 1.0);
        private DoubleRange outSaturation = new DoubleRange(0.0, 1.0);

        public HSLLinear()
        {
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            int num = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int left = rect.Left;
            int top = rect.Top;
            int num4 = left + rect.Width;
            int num5 = top + rect.Height;
            int num6 = image.Stride - (rect.Width * num);
            RGB rgb = new RGB();
            HSL hsl = new HSL();
            double num7 = 0.0;
            double num8 = 0.0;
            double num9 = 0.0;
            double num10 = 0.0;
            if (this.inLuminance.Max != this.inLuminance.Min)
            {
                num7 = (this.outLuminance.Max - this.outLuminance.Min) / (this.inLuminance.Max - this.inLuminance.Min);
                num8 = this.outLuminance.Min - (num7 * this.inLuminance.Min);
            }
            if (this.inSaturation.Max != this.inSaturation.Min)
            {
                num9 = (this.outSaturation.Max - this.outSaturation.Min) / (this.inSaturation.Max - this.inSaturation.Min);
                num10 = this.outSaturation.Min - (num9 * this.inSaturation.Min);
            }
            byte* numPtr = (byte*) (image.ImageData.ToPointer() + ((top * image.Stride) + (left * num)));
            for (int i = top; i < num5; i++)
            {
                int num12 = left;
                while (num12 < num4)
                {
                    rgb.Red = numPtr[2];
                    rgb.Green = numPtr[1];
                    rgb.Blue = numPtr[0];
                    HSL.FromRGB(rgb, hsl);
                    if (hsl.Luminance >= this.inLuminance.Max)
                    {
                        hsl.Luminance = this.outLuminance.Max;
                    }
                    else if (hsl.Luminance <= this.inLuminance.Min)
                    {
                        hsl.Luminance = this.outLuminance.Min;
                    }
                    else
                    {
                        hsl.Luminance = (num7 * hsl.Luminance) + num8;
                    }
                    if (hsl.Saturation >= this.inSaturation.Max)
                    {
                        hsl.Saturation = this.outSaturation.Max;
                    }
                    else if (hsl.Saturation <= this.inSaturation.Min)
                    {
                        hsl.Saturation = this.outSaturation.Min;
                    }
                    else
                    {
                        hsl.Saturation = (num9 * hsl.Saturation) + num10;
                    }
                    HSL.ToRGB(hsl, rgb);
                    numPtr[2] = rgb.Red;
                    numPtr[1] = rgb.Green;
                    numPtr[0] = rgb.Blue;
                    num12++;
                    numPtr += num;
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

        public DoubleRange InLuminance
        {
            get
            {
                return this.inLuminance;
            }
            set
            {
                this.inLuminance = value;
            }
        }

        public DoubleRange InSaturation
        {
            get
            {
                return this.inSaturation;
            }
            set
            {
                this.inSaturation = value;
            }
        }

        public DoubleRange OutLuminance
        {
            get
            {
                return this.outLuminance;
            }
            set
            {
                this.outLuminance = value;
            }
        }

        public DoubleRange OutSaturation
        {
            get
            {
                return this.outSaturation;
            }
            set
            {
                this.outSaturation = value;
            }
        }
    }
}

