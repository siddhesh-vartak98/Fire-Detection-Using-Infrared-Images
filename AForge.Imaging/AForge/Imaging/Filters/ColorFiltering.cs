namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ColorFiltering : BaseInPlacePartialFilter
    {
        private IntRange blue;
        private byte fillB;
        private byte fillG;
        private bool fillOutsideRange;
        private byte fillR;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private IntRange green;
        private IntRange red;

        public ColorFiltering()
        {
            this.red = new IntRange(0, 0xff);
            this.green = new IntRange(0, 0xff);
            this.blue = new IntRange(0, 0xff);
            this.fillOutsideRange = true;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        public ColorFiltering(IntRange red, IntRange green, IntRange blue) : this()
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            int num = (image.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
            int left = rect.Left;
            int top = rect.Top;
            int num4 = left + rect.Width;
            int num5 = top + rect.Height;
            int num6 = image.Stride - (rect.Width * num);
            byte* numPtr = (byte*) (image.ImageData.ToPointer() + ((top * image.Stride) + (left * num)));
            for (int i = top; i < num5; i++)
            {
                int num11 = left;
                while (num11 < num4)
                {
                    byte num7 = numPtr[2];
                    byte num8 = numPtr[1];
                    byte num9 = numPtr[0];
                    if ((((num7 >= this.red.Min) && (num7 <= this.red.Max)) && ((num8 >= this.green.Min) && (num8 <= this.green.Max))) && ((num9 >= this.blue.Min) && (num9 <= this.blue.Max)))
                    {
                        if (!this.fillOutsideRange)
                        {
                            numPtr[2] = this.fillR;
                            numPtr[1] = this.fillG;
                            numPtr[0] = this.fillB;
                        }
                    }
                    else if (this.fillOutsideRange)
                    {
                        numPtr[2] = this.fillR;
                        numPtr[1] = this.fillG;
                        numPtr[0] = this.fillB;
                    }
                    num11++;
                    numPtr += num;
                }
                numPtr += num6;
            }
        }

        public IntRange Blue
        {
            get
            {
                return this.blue;
            }
            set
            {
                this.blue = value;
            }
        }

        public RGB FillColor
        {
            get
            {
                return new RGB(this.fillR, this.fillG, this.fillB);
            }
            set
            {
                this.fillR = value.Red;
                this.fillG = value.Green;
                this.fillB = value.Blue;
            }
        }

        public bool FillOutsideRange
        {
            get
            {
                return this.fillOutsideRange;
            }
            set
            {
                this.fillOutsideRange = value;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public IntRange Green
        {
            get
            {
                return this.green;
            }
            set
            {
                this.green = value;
            }
        }

        public IntRange Red
        {
            get
            {
                return this.red;
            }
            set
            {
                this.red = value;
            }
        }
    }
}

