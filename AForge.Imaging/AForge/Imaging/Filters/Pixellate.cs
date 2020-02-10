namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class Pixellate : BaseInPlacePartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private int pixelHeight;
        private int pixelWidth;

        public Pixellate()
        {
            this.pixelWidth = 8;
            this.pixelHeight = 8;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
        }

        public Pixellate(int pixelSize) : this()
        {
            this.PixelSize = pixelSize;
        }

        public Pixellate(int pixelWidth, int pixelHeight) : this()
        {
            this.PixelWidth = pixelWidth;
            this.PixelHeight = pixelHeight;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            int num6;
            int num7;
            int num9;
            int num10;
            int num11;
            int num = (image.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
            int top = rect.Top;
            int num3 = top + rect.Height;
            int width = rect.Width;
            int num5 = image.Stride - (width * num);
            int length = ((width - 1) / this.pixelWidth) + 1;
            int num13 = ((width - 1) % this.pixelWidth) + 1;
            byte* numPtr = (byte*) (image.ImageData.ToPointer() + ((top * image.Stride) + (rect.Left * num)));
            byte* numPtr2 = numPtr;
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                int[] array = new int[length];
                int num14 = top;
                int num15 = top;
                while (num14 < num3)
                {
                    Array.Clear(array, 0, length);
                    num6 = 0;
                    while ((num6 < this.pixelHeight) && (num14 < num3))
                    {
                        num9 = 0;
                        while (num9 < width)
                        {
                            array[num9 / this.pixelWidth] += numPtr[0];
                            num9++;
                            numPtr++;
                        }
                        numPtr += num5;
                        num6++;
                        num14++;
                    }
                    num10 = num6 * this.pixelWidth;
                    num11 = num6 * num13;
                    num7 = 0;
                    while (num7 < (length - 1))
                    {
                        array[num7] /= num10;
                        num7++;
                    }
                    array[num7] /= num11;
                    num6 = 0;
                    while ((num6 < this.pixelHeight) && (num15 < num3))
                    {
                        num9 = 0;
                        while (num9 < width)
                        {
                            numPtr2[0] = (byte) array[num9 / this.pixelWidth];
                            num9++;
                            numPtr2++;
                        }
                        numPtr2 += num5;
                        num6++;
                        num15++;
                    }
                }
            }
            else
            {
                int[] numArray2 = new int[length * 3];
                int num16 = top;
                int num17 = top;
                while (num16 < num3)
                {
                    int num8;
                    Array.Clear(numArray2, 0, length * 3);
                    num6 = 0;
                    while ((num6 < this.pixelHeight) && (num16 < num3))
                    {
                        num9 = 0;
                        while (num9 < width)
                        {
                            num8 = (num9 / this.pixelWidth) * 3;
                            numArray2[num8] += numPtr[2];
                            numArray2[num8 + 1] += numPtr[1];
                            numArray2[num8 + 2] += numPtr[0];
                            num9++;
                            numPtr += 3;
                        }
                        numPtr += num5;
                        num6++;
                        num16++;
                    }
                    num10 = num6 * this.pixelWidth;
                    num11 = num6 * num13;
                    num7 = 0;
                    num8 = 0;
                    while (num7 < (length - 1))
                    {
                        numArray2[num8] /= num10;
                        numArray2[num8 + 1] /= num10;
                        numArray2[num8 + 2] /= num10;
                        num7++;
                        num8 += 3;
                    }
                    numArray2[num8] /= num11;
                    numArray2[num8 + 1] /= num11;
                    numArray2[num8 + 2] /= num11;
                    num6 = 0;
                    while ((num6 < this.pixelHeight) && (num17 < num3))
                    {
                        num9 = 0;
                        while (num9 < width)
                        {
                            num8 = (num9 / this.pixelWidth) * 3;
                            numPtr2[2] = (byte) numArray2[num8];
                            numPtr2[1] = (byte) numArray2[num8 + 1];
                            numPtr2[0] = (byte) numArray2[num8 + 2];
                            num9++;
                            numPtr2 += 3;
                        }
                        numPtr2 += num5;
                        num6++;
                        num17++;
                    }
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

        public int PixelHeight
        {
            get
            {
                return this.pixelHeight;
            }
            set
            {
                this.pixelHeight = Math.Max(2, Math.Min(0x20, value));
            }
        }

        public int PixelSize
        {
            set
            {
                this.pixelWidth = this.pixelHeight = Math.Max(2, Math.Min(0x20, value));
            }
        }

        public int PixelWidth
        {
            get
            {
                return this.pixelWidth;
            }
            set
            {
                this.pixelWidth = Math.Max(2, Math.Min(0x20, value));
            }
        }
    }
}

