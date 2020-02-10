namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class CanvasFill : BaseInPlaceFilter
    {
        private byte fillBlue;
        private byte fillGray;
        private byte fillGreen;
        private byte fillRed;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private Rectangle region;

        private CanvasFill()
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

        public CanvasFill(Rectangle region) : this()
        {
            this.region = region;
        }

        public CanvasFill(Rectangle region, byte fillColorGray) : this()
        {
            this.region = region;
            this.fillGray = fillColorGray;
        }

        public CanvasFill(Rectangle region, Color fillColorRGB) : this()
        {
            this.region = region;
            this.fillRed = fillColorRGB.R;
            this.fillGreen = fillColorRGB.G;
            this.fillBlue = fillColorRGB.B;
        }

        public CanvasFill(Rectangle region, Color fillColorRGB, byte fillColorGray) : this()
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
            int num4 = Math.Max(0, this.region.X);
            int num5 = Math.Max(0, this.region.Y);
            if ((num4 < width) && (num5 < height))
            {
                int num6 = Math.Min(width, this.region.Right);
                int num7 = Math.Min(height, this.region.Bottom);
                if ((num6 > num4) && (num7 > num5))
                {
                    int stride = image.Stride;
                    byte* dst = (byte*) ((image.ImageData.ToPointer() + (num5 * stride)) + (num4 * num));
                    if (image.PixelFormat == PixelFormat.Format8bppIndexed)
                    {
                        int count = num6 - num4;
                        for (int i = num5; i < num7; i++)
                        {
                            SystemTools.SetUnmanagedMemory(dst, this.fillGray, count);
                            dst += stride;
                        }
                    }
                    else
                    {
                        int num11 = stride - ((num6 - num4) * num);
                        for (int j = num5; j < num7; j++)
                        {
                            int num13 = num4;
                            while (num13 < num6)
                            {
                                dst[2] = this.fillRed;
                                dst[1] = this.fillGreen;
                                dst[0] = this.fillBlue;
                                num13++;
                                dst += num;
                            }
                            dst += num11;
                        }
                    }
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

