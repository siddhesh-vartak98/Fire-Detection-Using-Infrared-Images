namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class Median : BaseUsingCopyPartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private int size;

        public Median()
        {
            this.size = 3;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        public Median(int size) : this()
        {
            this.Size = size;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
        {
            int num10;
            int num11;
            int num12;
            int num14;
            int num = Image.GetPixelFormatSize(source.PixelFormat) / 8;
            int left = rect.Left;
            int top = rect.Top;
            int num4 = left + rect.Width;
            int num5 = top + rect.Height;
            int stride = source.Stride;
            int num7 = destination.Stride;
            int num8 = stride - (rect.Width * num);
            int num9 = num7 - (rect.Width * num);
            int num13 = this.size >> 1;
            byte[] array = new byte[this.size * this.size];
            byte[] buffer2 = new byte[this.size * this.size];
            byte[] buffer3 = new byte[this.size * this.size];
            byte* numPtr = (byte*) source.ImageData.ToPointer();
            byte* numPtr2 = (byte*) destination.ImageData.ToPointer();
            numPtr += (top * stride) + (left * num);
            numPtr2 += (top * num7) + (left * num);
            if (destination.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                for (int i = top; i < num5; i++)
                {
                    int num16 = left;
                    while (num16 < num4)
                    {
                        num14 = 0;
                        num10 = -num13;
                        while (num10 <= num13)
                        {
                            num12 = i + num10;
                            if (num12 >= top)
                            {
                                if (num12 >= num5)
                                {
                                    goto Label_0163;
                                }
                                num11 = -num13;
                                while (num11 <= num13)
                                {
                                    num12 = num16 + num11;
                                    if ((num12 >= left) && (num12 < num4))
                                    {
                                        buffer2[num14++] = numPtr[(num10 * stride) + num11];
                                    }
                                    num11++;
                                }
                            }
                            num10++;
                        }
                    Label_0163:
                        Array.Sort<byte>(buffer2, 0, num14);
                        numPtr2[0] = buffer2[num14 >> 1];
                        num16++;
                        numPtr++;
                        numPtr2++;
                    }
                    numPtr += num8;
                    numPtr2 += num9;
                }
            }
            else
            {
                for (int j = top; j < num5; j++)
                {
                    int num18 = left;
                    while (num18 < num4)
                    {
                        num14 = 0;
                        for (num10 = -num13; num10 <= num13; num10++)
                        {
                            num12 = j + num10;
                            if (num12 >= top)
                            {
                                if (num12 >= num5)
                                {
                                    goto Label_0241;
                                }
                                for (num11 = -num13; num11 <= num13; num11++)
                                {
                                    num12 = num18 + num11;
                                    if ((num12 >= left) && (num12 < num4))
                                    {
                                        byte* numPtr3 = numPtr + ((num10 * stride) + (num11 * num));
                                        array[num14] = numPtr3[2];
                                        buffer2[num14] = numPtr3[1];
                                        buffer3[num14] = numPtr3[0];
                                        num14++;
                                    }
                                }
                            }
                        }
                    Label_0241:
                        Array.Sort<byte>(array, 0, num14);
                        Array.Sort<byte>(buffer2, 0, num14);
                        Array.Sort<byte>(buffer3, 0, num14);
                        num12 = num14 >> 1;
                        numPtr2[2] = array[num12];
                        numPtr2[1] = buffer2[num12];
                        numPtr2[0] = buffer3[num12];
                        num18++;
                        numPtr += num;
                        numPtr2 += num;
                    }
                    numPtr += num8;
                    numPtr2 += num9;
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

        public int Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = Math.Max(3, Math.Min(0x19, value | 1));
            }
        }
    }
}

