namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class EuclideanColorFiltering : BaseInPlacePartialFilter
    {
        private RGB center;
        private RGB fill;
        private bool fillOutside;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private short radius;

        public EuclideanColorFiltering()
        {
            this.radius = 100;
            this.center = new RGB(0xff, 0xff, 0xff);
            this.fill = new RGB(0, 0, 0);
            this.fillOutside = true;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        public EuclideanColorFiltering(RGB center, short radius) : this()
        {
            this.center = center;
            this.radius = radius;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            int num = (image.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
            int left = rect.Left;
            int top = rect.Top;
            int num4 = left + rect.Width;
            int num5 = top + rect.Height;
            int num6 = image.Stride - (rect.Width * num);
            byte red = this.center.Red;
            byte green = this.center.Green;
            byte blue = this.center.Blue;
            byte num13 = this.fill.Red;
            byte num14 = this.fill.Green;
            byte num15 = this.fill.Blue;
            byte* numPtr = (byte*) (image.ImageData.ToPointer() + ((top * image.Stride) + (left * num)));
            for (int i = top; i < num5; i++)
            {
                int num17 = left;
                while (num17 < num4)
                {
                    byte num7 = numPtr[2];
                    byte num8 = numPtr[1];
                    byte num9 = numPtr[0];
                    if (((int) Math.Sqrt((Math.Pow((double) (num7 - red), 2.0) + Math.Pow((double) (num8 - green), 2.0)) + Math.Pow((double) (num9 - blue), 2.0))) <= this.radius)
                    {
                        if (!this.fillOutside)
                        {
                            numPtr[2] = num13;
                            numPtr[1] = num14;
                            numPtr[0] = num15;
                        }
                    }
                    else if (this.fillOutside)
                    {
                        numPtr[2] = num13;
                        numPtr[1] = num14;
                        numPtr[0] = num15;
                    }
                    num17++;
                    numPtr += num;
                }
                numPtr += num6;
            }
        }

        public RGB CenterColor
        {
            get
            {
                return this.center;
            }
            set
            {
                this.center = value;
            }
        }

        public RGB FillColor
        {
            get
            {
                return this.fill;
            }
            set
            {
                this.fill = value;
            }
        }

        public bool FillOutside
        {
            get
            {
                return this.fillOutside;
            }
            set
            {
                this.fillOutside = value;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public short Radius
        {
            get
            {
                return this.radius;
            }
            set
            {
                this.radius = Math.Max(0, Math.Min(450, value));
            }
        }
    }
}

