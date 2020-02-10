namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class LevelsLinear : BaseInPlacePartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
        private IntRange inBlue = new IntRange(0, 0xff);
        private IntRange inGreen = new IntRange(0, 0xff);
        private IntRange inRed = new IntRange(0, 0xff);
        private byte[] mapBlue = new byte[0x100];
        private byte[] mapGreen = new byte[0x100];
        private byte[] mapRed = new byte[0x100];
        private IntRange outBlue = new IntRange(0, 0xff);
        private IntRange outGreen = new IntRange(0, 0xff);
        private IntRange outRed = new IntRange(0, 0xff);

        public LevelsLinear()
        {
            this.CalculateMap(this.inRed, this.outRed, this.mapRed);
            this.CalculateMap(this.inGreen, this.outGreen, this.mapGreen);
            this.CalculateMap(this.inBlue, this.outBlue, this.mapBlue);
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        private void CalculateMap(IntRange inRange, IntRange outRange, byte[] map)
        {
            double num = 0.0;
            double num2 = 0.0;
            if (inRange.Max != inRange.Min)
            {
                num = ((double) (outRange.Max - outRange.Min)) / ((double) (inRange.Max - inRange.Min));
                num2 = outRange.Min - (num * inRange.Min);
            }
            for (int i = 0; i < 0x100; i++)
            {
                byte max = (byte) i;
                if (max >= inRange.Max)
                {
                    max = (byte) outRange.Max;
                }
                else if (max <= inRange.Min)
                {
                    max = (byte) outRange.Min;
                }
                else
                {
                    max = (byte) ((num * max) + num2);
                }
                map[i] = max;
            }
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            int num = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int left = rect.Left;
            int top = rect.Top;
            int num4 = left + rect.Width;
            int num5 = top + rect.Height;
            int num6 = image.Stride - (rect.Width * num);
            byte* numPtr = (byte*) (image.ImageData.ToPointer() + ((top * image.Stride) + (left * num)));
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                for (int i = top; i < num5; i++)
                {
                    int num8 = left;
                    while (num8 < num4)
                    {
                        numPtr[0] = this.mapGreen[numPtr[0]];
                        num8++;
                        numPtr++;
                    }
                    numPtr += num6;
                }
            }
            else
            {
                for (int j = top; j < num5; j++)
                {
                    int num10 = left;
                    while (num10 < num4)
                    {
                        numPtr[2] = this.mapRed[numPtr[2]];
                        numPtr[1] = this.mapGreen[numPtr[1]];
                        numPtr[0] = this.mapBlue[numPtr[0]];
                        num10++;
                        numPtr += num;
                    }
                    numPtr += num6;
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

        public IntRange InBlue
        {
            get
            {
                return this.inBlue;
            }
            set
            {
                this.inBlue = value;
                this.CalculateMap(this.inBlue, this.outBlue, this.mapBlue);
            }
        }

        public IntRange InGray
        {
            get
            {
                return this.inGreen;
            }
            set
            {
                this.inGreen = value;
                this.CalculateMap(this.inGreen, this.outGreen, this.mapGreen);
            }
        }

        public IntRange InGreen
        {
            get
            {
                return this.inGreen;
            }
            set
            {
                this.inGreen = value;
                this.CalculateMap(this.inGreen, this.outGreen, this.mapGreen);
            }
        }

        public IntRange Input
        {
            set
            {
                this.inRed = this.inGreen = this.inBlue = value;
                this.CalculateMap(this.inRed, this.outRed, this.mapRed);
                this.CalculateMap(this.inGreen, this.outGreen, this.mapGreen);
                this.CalculateMap(this.inBlue, this.outBlue, this.mapBlue);
            }
        }

        public IntRange InRed
        {
            get
            {
                return this.inRed;
            }
            set
            {
                this.inRed = value;
                this.CalculateMap(this.inRed, this.outRed, this.mapRed);
            }
        }

        public IntRange OutBlue
        {
            get
            {
                return this.outBlue;
            }
            set
            {
                this.outBlue = value;
                this.CalculateMap(this.inBlue, this.outBlue, this.mapBlue);
            }
        }

        public IntRange OutGray
        {
            get
            {
                return this.outGreen;
            }
            set
            {
                this.outGreen = value;
                this.CalculateMap(this.inGreen, this.outGreen, this.mapGreen);
            }
        }

        public IntRange OutGreen
        {
            get
            {
                return this.outGreen;
            }
            set
            {
                this.outGreen = value;
                this.CalculateMap(this.inGreen, this.outGreen, this.mapGreen);
            }
        }

        public IntRange Output
        {
            set
            {
                this.outRed = this.outGreen = this.outBlue = value;
                this.CalculateMap(this.inRed, this.outRed, this.mapRed);
                this.CalculateMap(this.inGreen, this.outGreen, this.mapGreen);
                this.CalculateMap(this.inBlue, this.outBlue, this.mapBlue);
            }
        }

        public IntRange OutRed
        {
            get
            {
                return this.outRed;
            }
            set
            {
                this.outRed = value;
                this.CalculateMap(this.inRed, this.outRed, this.mapRed);
            }
        }
    }
}

