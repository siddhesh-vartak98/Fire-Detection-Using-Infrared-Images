namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ChannelFiltering : BaseInPlacePartialFilter
    {
        private IntRange blue;
        private bool blueFillOutsideRange;
        private byte fillB;
        private byte fillG;
        private byte fillR;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private IntRange green;
        private bool greenFillOutsideRange;
        private byte[] mapBlue;
        private byte[] mapGreen;
        private byte[] mapRed;
        private IntRange red;
        private bool redFillOutsideRange;

        public ChannelFiltering() : this(new IntRange(0, 0xff), new IntRange(0, 0xff), new IntRange(0, 0xff))
        {
        }

        public ChannelFiltering(IntRange red, IntRange green, IntRange blue)
        {
            this.red = new IntRange(0, 0xff);
            this.green = new IntRange(0, 0xff);
            this.blue = new IntRange(0, 0xff);
            this.redFillOutsideRange = true;
            this.greenFillOutsideRange = true;
            this.blueFillOutsideRange = true;
            this.mapRed = new byte[0x100];
            this.mapGreen = new byte[0x100];
            this.mapBlue = new byte[0x100];
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        private void CalculateMap(IntRange range, byte fill, bool fillOutsideRange, byte[] map)
        {
            for (int i = 0; i < 0x100; i++)
            {
                if ((i >= range.Min) && (i <= range.Max))
                {
                    map[i] = fillOutsideRange ? ((byte) i) : fill;
                }
                else
                {
                    map[i] = fillOutsideRange ? fill : ((byte) i);
                }
            }
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
                int num8 = left;
                while (num8 < num4)
                {
                    numPtr[2] = this.mapRed[numPtr[2]];
                    numPtr[1] = this.mapGreen[numPtr[1]];
                    numPtr[0] = this.mapBlue[numPtr[0]];
                    num8++;
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
                this.CalculateMap(this.blue, this.fillB, this.blueFillOutsideRange, this.mapBlue);
            }
        }

        public bool BlueFillOutsideRange
        {
            get
            {
                return this.blueFillOutsideRange;
            }
            set
            {
                this.blueFillOutsideRange = value;
                this.CalculateMap(this.blue, this.fillB, this.blueFillOutsideRange, this.mapBlue);
            }
        }

        public byte FillBlue
        {
            get
            {
                return this.fillB;
            }
            set
            {
                this.fillB = value;
                this.CalculateMap(this.blue, this.fillB, this.blueFillOutsideRange, this.mapBlue);
            }
        }

        public byte FillGreen
        {
            get
            {
                return this.fillG;
            }
            set
            {
                this.fillG = value;
                this.CalculateMap(this.green, this.fillG, this.greenFillOutsideRange, this.mapGreen);
            }
        }

        public byte FillRed
        {
            get
            {
                return this.fillR;
            }
            set
            {
                this.fillR = value;
                this.CalculateMap(this.red, this.fillR, this.redFillOutsideRange, this.mapRed);
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
                this.CalculateMap(this.green, this.fillG, this.greenFillOutsideRange, this.mapGreen);
            }
        }

        public bool GreenFillOutsideRange
        {
            get
            {
                return this.greenFillOutsideRange;
            }
            set
            {
                this.greenFillOutsideRange = value;
                this.CalculateMap(this.green, this.fillG, this.greenFillOutsideRange, this.mapGreen);
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
                this.CalculateMap(this.red, this.fillR, this.redFillOutsideRange, this.mapRed);
            }
        }

        public bool RedFillOutsideRange
        {
            get
            {
                return this.redFillOutsideRange;
            }
            set
            {
                this.redFillOutsideRange = value;
                this.CalculateMap(this.red, this.fillR, this.redFillOutsideRange, this.mapRed);
            }
        }
    }
}

