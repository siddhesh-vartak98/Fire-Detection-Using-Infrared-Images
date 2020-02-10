namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ConservativeSmoothing : BaseUsingCopyPartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private int size;

        public ConservativeSmoothing()
        {
            this.size = 3;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        public ConservativeSmoothing(int size) : this()
        {
            this.KernelSize = size;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
        {
            int num10;
            int num11;
            int num12;
            byte num16;
            byte num17;
            byte num20;
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
            byte* numPtr = (byte*) source.ImageData.ToPointer();
            byte* numPtr2 = (byte*) destination.ImageData.ToPointer();
            numPtr += (top * stride) + (left * num);
            numPtr2 += (top * num7) + (left * num);
            if (destination.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                for (int i = top; i < num5; i++)
                {
                    int num22 = left;
                    while (num22 < num4)
                    {
                        num16 = 0xff;
                        num17 = 0;
                        num10 = -num13;
                        while (num10 <= num13)
                        {
                            num12 = i + num10;
                            if (num12 >= top)
                            {
                                if (num12 >= num5)
                                {
                                    goto Label_0140;
                                }
                                num11 = -num13;
                                while (num11 <= num13)
                                {
                                    num12 = num22 + num11;
                                    if (((num12 >= left) && (num10 != num11)) && (num12 < num4))
                                    {
                                        num20 = numPtr[(num10 * stride) + num11];
                                        if (num20 < num16)
                                        {
                                            num16 = num20;
                                        }
                                        if (num20 > num17)
                                        {
                                            num17 = num20;
                                        }
                                    }
                                    num11++;
                                }
                            }
                            num10++;
                        }
                    Label_0140:
                        num20 = numPtr[0];
                        numPtr2[0] = (num20 > num17) ? num17 : ((num20 < num16) ? num16 : num20);
                        num22++;
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
                    int num24 = left;
                    while (num24 < num4)
                    {
                        byte num18;
                        byte num19;
                        byte num14 = num16 = (byte) (num18 = 0xff);
                        byte num15 = num17 = (byte) (num19 = 0);
                        for (num10 = -num13; num10 <= num13; num10++)
                        {
                            num12 = j + num10;
                            if (num12 >= top)
                            {
                                if (num12 >= num5)
                                {
                                    goto Label_027D;
                                }
                                for (num11 = -num13; num11 <= num13; num11++)
                                {
                                    num12 = num24 + num11;
                                    if (((num12 >= left) && (num10 != num11)) && (num12 < num4))
                                    {
                                        byte* numPtr3 = numPtr + ((num10 * stride) + (num11 * num));
                                        num20 = numPtr3[2];
                                        if (num20 < num14)
                                        {
                                            num14 = num20;
                                        }
                                        if (num20 > num15)
                                        {
                                            num15 = num20;
                                        }
                                        num20 = numPtr3[1];
                                        if (num20 < num16)
                                        {
                                            num16 = num20;
                                        }
                                        if (num20 > num17)
                                        {
                                            num17 = num20;
                                        }
                                        num20 = numPtr3[0];
                                        if (num20 < num18)
                                        {
                                            num18 = num20;
                                        }
                                        if (num20 > num19)
                                        {
                                            num19 = num20;
                                        }
                                    }
                                }
                            }
                        }
                    Label_027D:
                        num20 = numPtr[2];
                        numPtr2[2] = (num20 > num15) ? num15 : ((num20 < num14) ? num14 : num20);
                        num20 = numPtr[1];
                        numPtr2[1] = (num20 > num17) ? num17 : ((num20 < num16) ? num16 : num20);
                        num20 = numPtr[0];
                        numPtr2[0] = (num20 > num19) ? num19 : ((num20 < num18) ? num18 : num20);
                        num24++;
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

        public int KernelSize
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

