namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class HueModifier : BaseInPlacePartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private int hue;

        public HueModifier()
        {
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            this.formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
        }

        public HueModifier(int hue) : this()
        {
            this.hue = hue;
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
            byte* numPtr = (byte*) (image.ImageData.ToPointer() + ((top * image.Stride) + (left * num)));
            for (int i = top; i < num5; i++)
            {
                int num8 = left;
                while (num8 < num4)
                {
                    rgb.Red = numPtr[2];
                    rgb.Green = numPtr[1];
                    rgb.Blue = numPtr[0];
                    HSL.FromRGB(rgb, hsl);
                    hsl.Hue = this.hue;
                    HSL.ToRGB(hsl, rgb);
                    numPtr[2] = rgb.Red;
                    numPtr[1] = rgb.Green;
                    numPtr[0] = rgb.Blue;
                    num8++;
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

        public int Hue
        {
            get
            {
                return this.hue;
            }
            set
            {
                this.hue = Math.Max(0, Math.Min(0x167, value));
            }
        }
    }
}

