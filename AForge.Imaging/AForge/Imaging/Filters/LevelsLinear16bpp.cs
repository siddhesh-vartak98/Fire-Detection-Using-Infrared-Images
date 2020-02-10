namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class LevelsLinear16bpp : BaseInPlacePartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
        private IntRange inBlue = new IntRange(0, 0xffff);
        private IntRange inGreen = new IntRange(0, 0xffff);
        private IntRange inRed = new IntRange(0, 0xffff);
        private ushort[] mapBlue = new ushort[0x10000];
        private ushort[] mapGreen = new ushort[0x10000];
        private ushort[] mapRed = new ushort[0x10000];
        private IntRange outBlue = new IntRange(0, 0xffff);
        private IntRange outGreen = new IntRange(0, 0xffff);
        private IntRange outRed = new IntRange(0, 0xffff);

        public LevelsLinear16bpp()
        {
            this.CalculateMap(this.inRed, this.outRed, this.mapRed);
            this.CalculateMap(this.inGreen, this.outGreen, this.mapGreen);
            this.CalculateMap(this.inBlue, this.outBlue, this.mapBlue);
            this.formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            this.formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
            this.formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
            this.formatTranslations[PixelFormat.Format64bppPArgb] = PixelFormat.Format64bppPArgb;
        }

        private void CalculateMap(IntRange inRange, IntRange outRange, ushort[] map)
        {
            double num = 0.0;
            double num2 = 0.0;
            if (inRange.Max != inRange.Min)
            {
                num = ((double) (outRange.Max - outRange.Min)) / ((double) (inRange.Max - inRange.Min));
                num2 = outRange.Min - (num * inRange.Min);
            }
            for (int i = 0; i < 0x10000; i++)
            {
                ushort max = (ushort) i;
                if (max >= inRange.Max)
                {
                    max = (ushort) outRange.Max;
                }
                else if (max <= inRange.Min)
                {
                    max = (ushort) outRange.Min;
                }
                else
                {
                    max = (ushort) ((num * max) + num2);
                }
                map[i] = max;
            }
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            int num = Image.GetPixelFormatSize(image.PixelFormat) / 0x10;
            int left = rect.Left;
            int top = rect.Top;
            int num4 = left + rect.Width;
            int num5 = top + rect.Height;
            int stride = image.Stride;
            int width = rect.Width;
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            if (image.PixelFormat == PixelFormat.Format16bppGrayScale)
            {
                for (int i = top; i < num5; i++)
                {
                    ushort* numPtr2 = (ushort*) ((numPtr + (i * image.Stride)) + (left * 2));
                    int num7 = left;
                    while (num7 < num4)
                    {
                        numPtr2[0] = this.mapGreen[numPtr2[0]];
                        num7++;
                        numPtr2++;
                    }
                }
            }
            else
            {
                for (int j = top; j < num5; j++)
                {
                    ushort* numPtr3 = (ushort*) ((numPtr + (j * image.Stride)) + ((left * num) * 2));
                    int num9 = left;
                    while (num9 < num4)
                    {
                        numPtr3[2] = this.mapRed[numPtr3[2]];
                        numPtr3[1] = this.mapGreen[numPtr3[1]];
                        numPtr3[0] = this.mapBlue[numPtr3[0]];
                        num9++;
                        numPtr3 += num;
                    }
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

