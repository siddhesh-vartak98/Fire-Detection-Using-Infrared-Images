namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ColorRemapping : BaseInPlacePartialFilter
    {
        private byte[] blueMap;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private byte[] grayMap;
        private byte[] greenMap;
        private byte[] redMap;

        public ColorRemapping()
        {
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.redMap = new byte[0x100];
            this.greenMap = new byte[0x100];
            this.blueMap = new byte[0x100];
            this.grayMap = new byte[0x100];
            for (int i = 0; i < 0x100; i++)
            {
                byte num2;
                byte num3;
                this.grayMap[i] = num2 = (byte) i;
                this.blueMap[i] = num3 = num2;
                this.redMap[i] = this.greenMap[i] = num3;
            }
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        public ColorRemapping(byte[] grayMap) : this()
        {
            this.GrayMap = grayMap;
        }

        public ColorRemapping(byte[] redMap, byte[] greenMap, byte[] blueMap) : this()
        {
            this.RedMap = redMap;
            this.GreenMap = greenMap;
            this.BlueMap = blueMap;
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
                        numPtr[0] = this.grayMap[numPtr[0]];
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
                        numPtr[2] = this.redMap[numPtr[2]];
                        numPtr[1] = this.greenMap[numPtr[1]];
                        numPtr[0] = this.blueMap[numPtr[0]];
                        num10++;
                        numPtr += num;
                    }
                    numPtr += num6;
                }
            }
        }

        public byte[] BlueMap
        {
            get
            {
                return this.blueMap;
            }
            set
            {
                if ((value == null) || (value.Length != 0x100))
                {
                    throw new ArgumentException("A map should be array with 256 value.");
                }
                this.blueMap = value;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public byte[] GrayMap
        {
            get
            {
                return this.grayMap;
            }
            set
            {
                if ((value == null) || (value.Length != 0x100))
                {
                    throw new ArgumentException("A map should be array with 256 value.");
                }
                this.grayMap = value;
            }
        }

        public byte[] GreenMap
        {
            get
            {
                return this.greenMap;
            }
            set
            {
                if ((value == null) || (value.Length != 0x100))
                {
                    throw new ArgumentException("A map should be array with 256 value.");
                }
                this.greenMap = value;
            }
        }

        public byte[] RedMap
        {
            get
            {
                return this.redMap;
            }
            set
            {
                if ((value == null) || (value.Length != 0x100))
                {
                    throw new ArgumentException("A map should be array with 256 value.");
                }
                this.redMap = value;
            }
        }
    }
}

