namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public abstract class ErrorDiffusionDithering : BaseInPlacePartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
        protected int startX;
        protected int startY;
        protected int stopX;
        protected int stopY;
        protected int stride;
        private byte threshold = 0x80;
        protected int x;
        protected int y;

        protected ErrorDiffusionDithering()
        {
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        protected abstract unsafe void Diffuse(int error, byte* ptr);
        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            this.startX = rect.Left;
            this.startY = rect.Top;
            this.stopX = this.startX + rect.Width;
            this.stopY = this.startY + rect.Height;
            this.stride = image.Stride;
            int num = this.stride - rect.Width;
            byte* ptr = (byte*) (image.ImageData.ToPointer() + ((this.startY * this.stride) + this.startX));
            this.y = this.startY;
            while (this.y < this.stopY)
            {
                this.x = this.startX;
                while (this.x < this.stopX)
                {
                    int num3;
                    int num2 = ptr[0];
                    if (num2 >= this.threshold)
                    {
                        ptr[0] = 0xff;
                        num3 = num2 - 0xff;
                    }
                    else
                    {
                        ptr[0] = 0;
                        num3 = num2;
                    }
                    this.Diffuse(num3, ptr);
                    this.x++;
                    ptr++;
                }
                ptr += num;
                this.y++;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public byte ThresholdValue
        {
            get
            {
                return this.threshold;
            }
            set
            {
                this.threshold = value;
            }
        }
    }
}

