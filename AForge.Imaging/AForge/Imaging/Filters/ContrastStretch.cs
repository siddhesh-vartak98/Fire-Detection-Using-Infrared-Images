namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ContrastStretch : BaseInPlacePartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        public ContrastStretch()
        {
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            int num = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int left = rect.Left;
            int top = rect.Top;
            int num4 = left + rect.Width;
            int num5 = top + rect.Height;
            int stride = image.Stride;
            int num7 = stride - (rect.Width * num);
            LevelsLinear linear = new LevelsLinear();
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                numPtr += (top * stride) + left;
                byte min = 0xff;
                byte max = 0;
                for (int i = top; i < num5; i++)
                {
                    int num11 = left;
                    while (num11 < num4)
                    {
                        byte num12 = numPtr[0];
                        if (num12 < min)
                        {
                            min = num12;
                        }
                        if (num12 > max)
                        {
                            max = num12;
                        }
                        num11++;
                        numPtr++;
                    }
                    numPtr += num7;
                }
                linear.InGray = new IntRange(min, max);
            }
            else
            {
                numPtr += (top * stride) + (left * num);
                byte num13 = 0xff;
                byte num14 = 0xff;
                byte num15 = 0xff;
                byte num16 = 0;
                byte num17 = 0;
                byte num18 = 0;
                for (int j = top; j < num5; j++)
                {
                    int num20 = left;
                    while (num20 < num4)
                    {
                        byte num21 = numPtr[2];
                        if (num21 < num13)
                        {
                            num13 = num21;
                        }
                        if (num21 > num16)
                        {
                            num16 = num21;
                        }
                        num21 = numPtr[1];
                        if (num21 < num14)
                        {
                            num14 = num21;
                        }
                        if (num21 > num17)
                        {
                            num17 = num21;
                        }
                        num21 = numPtr[0];
                        if (num21 < num15)
                        {
                            num15 = num21;
                        }
                        if (num21 > num18)
                        {
                            num18 = num21;
                        }
                        num20++;
                        numPtr += num;
                    }
                    numPtr += num7;
                }
                linear.InRed = new IntRange(num13, num16);
                linear.InGreen = new IntRange(num14, num17);
                linear.InBlue = new IntRange(num15, num18);
            }
            linear.ApplyInPlace(image, rect);
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

