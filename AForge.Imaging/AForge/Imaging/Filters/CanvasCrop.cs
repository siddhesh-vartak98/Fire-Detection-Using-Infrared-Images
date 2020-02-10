namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class CanvasCrop : BaseInPlaceFilter
    {
        private byte fillBlue;
        private byte fillGray;
        private byte fillGreen;
        private byte fillRed;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private Rectangle region;

        private CanvasCrop()
        {
            this.fillRed = 0xff;
            this.fillGreen = 0xff;
            this.fillBlue = 0xff;
            this.fillGray = 0xff;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
        }

        public CanvasCrop(Rectangle region) : this()
        {
            this.region = region;
        }

        public CanvasCrop(Rectangle region, byte fillColorGray) : this()
        {
            this.region = region;
            this.fillGray = fillColorGray;
        }

        public CanvasCrop(Rectangle region, Color fillColorRGB) : this()
        {
            this.region = region;
            this.fillRed = fillColorRGB.R;
            this.fillGreen = fillColorRGB.G;
            this.fillBlue = fillColorRGB.B;
        }

        public CanvasCrop(Rectangle region, Color fillColorRGB, byte fillColorGray) : this()
        {
            this.region = region;
            this.fillRed = fillColorRGB.R;
            this.fillGreen = fillColorRGB.G;
            this.fillBlue = fillColorRGB.B;
            this.fillGray = fillColorGray;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image)
        {
            int num = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int width = image.Width;
            int height = image.Height;
            int num4 = image.Stride - (width * num);
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                for (int i = 0; i < height; i++)
                {
                    int x = 0;
                    while (x < width)
                    {
                        if (!this.region.Contains(x, i))
                        {
                            numPtr[0] = this.fillGray;
                        }
                        x++;
                        numPtr++;
                    }
                    numPtr += num4;
                }
            }
            else
            {
                for (int j = 0; j < height; j++)
                {
                    int num8 = 0;
                    while (num8 < width)
                    {
                        if (!this.region.Contains(num8, j))
                        {
                            numPtr[2] = this.fillRed;
                            numPtr[1] = this.fillGreen;
                            numPtr[0] = this.fillBlue;
                        }
                        num8++;
                        numPtr += num;
                    }
                    numPtr += num4;
                }
            }
        }

        public byte FillColorGray
        {
            get
            {
                return this.fillGray;
            }
            set
            {
                this.fillGray = value;
            }
        }

        public Color FillColorRGB
        {
            get
            {
                return Color.FromArgb(this.fillRed, this.fillGreen, this.fillBlue);
            }
            set
            {
                this.fillRed = value.R;
                this.fillGreen = value.G;
                this.fillBlue = value.B;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public Rectangle Region
        {
            get
            {
                return this.region;
            }
            set
            {
                this.region = value;
            }
        }
    }
}

