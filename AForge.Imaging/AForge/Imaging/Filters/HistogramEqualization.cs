namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class HistogramEqualization : BaseInPlacePartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        public HistogramEqualization()
        {
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        private byte[] Equalize(int[] histogram, long numPixel)
        {
            byte[] buffer = new byte[0x100];
            float num = 255f / ((float) numPixel);
            float num2 = histogram[0] * num;
            buffer[0] = (byte) num2;
            for (int i = 1; i < 0x100; i++)
            {
                num2 += histogram[i] * num;
                buffer[i] = (byte) num2;
            }
            return buffer;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            int num = (image.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : ((image.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4);
            int left = rect.Left;
            int top = rect.Top;
            int num4 = left + rect.Width;
            int num5 = top + rect.Height;
            int stride = image.Stride;
            int num7 = stride - (rect.Width * num);
            int num8 = (num4 - left) * (num5 - top);
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                byte* numPtr = (byte*) (image.ImageData.ToPointer() + ((top * stride) + left));
                int[] histogram = new int[0x100];
                for (int i = top; i < num5; i++)
                {
                    int num10 = left;
                    while (num10 < num4)
                    {
                        histogram[numPtr[0]]++;
                        num10++;
                        numPtr++;
                    }
                    numPtr += num7;
                }
                byte[] buffer = this.Equalize(histogram, (long) num8);
                numPtr = (byte*) (image.ImageData.ToPointer() + ((top * stride) + left));
                for (int j = top; j < num5; j++)
                {
                    int num12 = left;
                    while (num12 < num4)
                    {
                        numPtr[0] = buffer[numPtr[0]];
                        num12++;
                        numPtr++;
                    }
                    numPtr += num7;
                }
            }
            else
            {
                byte* numPtr2 = (byte*) (image.ImageData.ToPointer() + ((top * stride) + (left * num)));
                int[] numArray2 = new int[0x100];
                int[] numArray3 = new int[0x100];
                int[] numArray4 = new int[0x100];
                for (int k = top; k < num5; k++)
                {
                    int num14 = left;
                    while (num14 < num4)
                    {
                        numArray2[numPtr2[2]]++;
                        numArray3[numPtr2[1]]++;
                        numArray4[numPtr2[0]]++;
                        num14++;
                        numPtr2 += num;
                    }
                    numPtr2 += num7;
                }
                byte[] buffer2 = this.Equalize(numArray2, (long) num8);
                byte[] buffer3 = this.Equalize(numArray3, (long) num8);
                byte[] buffer4 = this.Equalize(numArray4, (long) num8);
                numPtr2 = (byte*) (image.ImageData.ToPointer() + ((top * stride) + (left * num)));
                for (int m = top; m < num5; m++)
                {
                    int num16 = left;
                    while (num16 < num4)
                    {
                        numPtr2[2] = buffer2[numPtr2[2]];
                        numPtr2[1] = buffer3[numPtr2[1]];
                        numPtr2[0] = buffer4[numPtr2[0]];
                        num16++;
                        numPtr2 += num;
                    }
                    numPtr2 += num7;
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
    }
}

