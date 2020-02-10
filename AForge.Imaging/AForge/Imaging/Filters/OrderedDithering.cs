namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class OrderedDithering : BaseInPlacePartialFilter
    {
        private int cols;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private byte[,] matrix;
        private int rows;

        public OrderedDithering()
        {
            this.rows = 4;
            this.cols = 4;
            this.matrix = new byte[,] { { 15, 0x8f, 0x2f, 0xaf }, { 0xcf, 0x4f, 0xef, 0x6f }, { 0x3f, 0xbf, 0x1f, 0x9f }, { 0xff, 0x7f, 0xdf, 0x5f } };
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        public OrderedDithering(byte[,] matrix) : this()
        {
            this.rows = matrix.GetLength(0);
            this.cols = matrix.GetLength(1);
            this.matrix = matrix;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            int left = rect.Left;
            int top = rect.Top;
            int num3 = left + rect.Width;
            int num4 = top + rect.Height;
            int num5 = image.Stride - rect.Width;
            byte* numPtr = (byte*) (image.ImageData.ToPointer() + ((top * image.Stride) + left));
            for (int i = top; i < num4; i++)
            {
                int num7 = left;
                while (num7 < num3)
                {
                    numPtr[0] = (numPtr[0] <= this.matrix[i % this.rows, num7 % this.cols]) ? ((byte) 0) : ((byte) 0xff);
                    num7++;
                    numPtr++;
                }
                numPtr += num5;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }
    }
}

