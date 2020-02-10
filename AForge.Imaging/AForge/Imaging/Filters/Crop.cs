namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class Crop : BaseTransformationFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
        private System.Drawing.Rectangle rect;

        public Crop(System.Drawing.Rectangle rect)
        {
            this.rect = rect;
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            this.formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            this.formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
            this.formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
        }

        protected override Size CalculateNewImageSize(UnmanagedImage sourceData)
        {
            return new Size(this.rect.Width, this.rect.Height);
        }

        protected override unsafe void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            System.Drawing.Rectangle rect = this.rect;
            rect.Intersect(new System.Drawing.Rectangle(0, 0, sourceData.Width, sourceData.Height));
            int left = rect.Left;
            int top = rect.Top;
            int num3 = rect.Bottom - 1;
            int width = rect.Width;
            int stride = sourceData.Stride;
            int num6 = destinationData.Stride;
            int num7 = Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
            int count = width * num7;
            byte* src = (byte*) ((sourceData.ImageData.ToPointer() + (top * stride)) + (left * num7));
            byte* dst = (byte*) destinationData.ImageData.ToPointer();
            if (this.rect.Top < 0)
            {
                dst -= num6 * this.rect.Top;
            }
            if (this.rect.Left < 0)
            {
                dst -= num7 * this.rect.Left;
            }
            for (int i = top; i <= num3; i++)
            {
                SystemTools.CopyUnmanagedMemory(dst, src, count);
                src += stride;
                dst += num6;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public System.Drawing.Rectangle Rectangle
        {
            get
            {
                return this.rect;
            }
            set
            {
                this.rect = value;
            }
        }
    }
}

