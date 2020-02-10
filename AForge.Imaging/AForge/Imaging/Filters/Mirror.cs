namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class Mirror : BaseInPlacePartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
        private bool mirrorX;
        private bool mirrorY;

        public Mirror(bool mirrorX, bool mirrorY)
        {
            this.mirrorX = mirrorX;
            this.MirrorY = mirrorY;
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            int num = (image.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
            int width = rect.Width;
            int height = rect.Height;
            int top = rect.Top;
            int num5 = top + height;
            int left = rect.Left;
            int num7 = left + width;
            int num8 = left * num;
            int num9 = num7 * num;
            int stride = image.Stride;
            if (this.mirrorY)
            {
                byte num13;
                byte* numPtr = (byte*) (image.ImageData.ToPointer() + ((top * stride) + (left * num)));
                byte* numPtr2 = (byte*) (image.ImageData.ToPointer() + ((top * stride) + ((num7 - 1) * num)));
                int num11 = stride - ((width >> 1) * num);
                int num12 = stride + ((width >> 1) * num);
                if (image.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    for (int i = top; i < num5; i++)
                    {
                        int num15 = left;
                        int num16 = left + (width >> 1);
                        while (num15 < num16)
                        {
                            num13 = numPtr[0];
                            numPtr[0] = numPtr2[0];
                            numPtr2[0] = num13;
                            num15++;
                            numPtr++;
                            numPtr2--;
                        }
                        numPtr += num11;
                        numPtr2 += num12;
                    }
                }
                else
                {
                    for (int j = top; j < num5; j++)
                    {
                        int num18 = left;
                        int num19 = left + (width >> 1);
                        while (num18 < num19)
                        {
                            num13 = numPtr[2];
                            numPtr[2] = numPtr2[2];
                            numPtr2[2] = num13;
                            num13 = numPtr[1];
                            numPtr[1] = numPtr2[1];
                            numPtr2[1] = num13;
                            num13 = numPtr[0];
                            numPtr[0] = numPtr2[0];
                            numPtr2[0] = num13;
                            num18++;
                            numPtr += 3;
                            numPtr2 -= 3;
                        }
                        numPtr += num11;
                        numPtr2 += num12;
                    }
                }
            }
            if (this.mirrorX)
            {
                int num20 = stride - (rect.Width * num);
                byte* numPtr3 = (byte*) (image.ImageData.ToPointer() + ((top * stride) + (left * num)));
                byte* numPtr4 = (byte*) (image.ImageData.ToPointer() + (((num5 - 1) * stride) + (left * num)));
                int num22 = top;
                int num23 = top + (height >> 1);
                while (num22 < num23)
                {
                    int num24 = num8;
                    while (num24 < num9)
                    {
                        byte num21 = numPtr3[0];
                        numPtr3[0] = numPtr4[0];
                        numPtr4[0] = num21;
                        num24++;
                        numPtr3++;
                        numPtr4++;
                    }
                    numPtr3 += num20;
                    numPtr4 += (num20 - stride) - stride;
                    num22++;
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

        public bool MirrorX
        {
            get
            {
                return this.mirrorX;
            }
            set
            {
                this.mirrorX = value;
            }
        }

        public bool MirrorY
        {
            get
            {
                return this.mirrorY;
            }
            set
            {
                this.mirrorY = value;
            }
        }
    }
}

