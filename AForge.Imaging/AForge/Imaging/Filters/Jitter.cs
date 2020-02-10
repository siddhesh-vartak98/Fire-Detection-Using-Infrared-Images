namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class Jitter : BaseUsingCopyPartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private int radius;
        private Random rand;

        public Jitter()
        {
            this.radius = 2;
            this.rand = new Random();
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        public Jitter(int radius) : this()
        {
            this.Radius = radius;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
        {
            int num = Image.GetPixelFormatSize(source.PixelFormat) / 8;
            int left = rect.Left;
            int top = rect.Top;
            int num4 = left + rect.Width;
            int num5 = top + rect.Height;
            int stride = source.Stride;
            int num7 = destination.Stride;
            int num8 = num7 - (rect.Width * num);
            int maxValue = (this.radius * 2) + 1;
            byte* src = (byte*) source.ImageData.ToPointer();
            byte* dst = (byte*) destination.ImageData.ToPointer();
            if (stride == num7)
            {
                SystemTools.CopyUnmanagedMemory(dst, src, stride * source.Height);
            }
            else
            {
                int count = source.Width * num;
                int num13 = 0;
                int height = source.Height;
                while (num13 < height)
                {
                    SystemTools.CopyUnmanagedMemory(dst + (num7 * num13), src + (stride * num13), count);
                    num13++;
                }
            }
            dst += (top * num7) + (left * num);
            for (int i = top; i < num5; i++)
            {
                for (int j = left; j < num4; j++)
                {
                    int num9 = (j + this.rand.Next(maxValue)) - this.radius;
                    int num10 = (i + this.rand.Next(maxValue)) - this.radius;
                    if (((num9 >= left) && (num10 >= top)) && ((num9 < num4) && (num10 < num5)))
                    {
                        byte* numPtr3 = (src + (num10 * stride)) + (num9 * num);
                        int num17 = 0;
                        while (num17 < num)
                        {
                            dst[0] = numPtr3[0];
                            num17++;
                            dst++;
                            numPtr3++;
                        }
                    }
                    else
                    {
                        dst += num;
                    }
                }
                dst += num8;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public int Radius
        {
            get
            {
                return this.radius;
            }
            set
            {
                this.radius = Math.Max(1, Math.Min(10, value));
            }
        }
    }
}

