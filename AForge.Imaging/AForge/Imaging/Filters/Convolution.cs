namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class Convolution : BaseUsingCopyPartialFilter
    {
        private int divisor;
        private bool dynamicDivisorForEdges;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private int[,] kernel;
        private int size;
        private int threshold;

        protected Convolution()
        {
            this.divisor = 1;
            this.dynamicDivisorForEdges = true;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            this.formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
            this.formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
        }

        public Convolution(int[,] kernel) : this()
        {
            this.Kernel = kernel;
            this.divisor = 0;
            int num = 0;
            int length = kernel.GetLength(0);
            while (num < length)
            {
                int num3 = 0;
                int num4 = kernel.GetLength(1);
                while (num3 < num4)
                {
                    this.divisor += kernel[num, num3];
                    num3++;
                }
                num++;
            }
            if (this.divisor == 0)
            {
                this.divisor = 1;
            }
        }

        public Convolution(int[,] kernel, int divisor) : this()
        {
            this.Kernel = kernel;
            this.Divisor = divisor;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
        {
            int num6;
            int num7;
            int num8;
            int num9;
            int num10;
            int num11;
            long num13;
            long num14;
            long num15;
            long divisor;
            int num18;
            int num = Image.GetPixelFormatSize(source.PixelFormat) / 8;
            int left = rect.Left;
            int top = rect.Top;
            int num4 = left + rect.Width;
            int num5 = top + rect.Height;
            int num12 = this.size >> 1;
            int num17 = this.size * this.size;
            if (num <= 4)
            {
                int stride = source.Stride;
                int num20 = destination.Stride;
                int num21 = stride - (rect.Width * num);
                int num22 = num20 - (rect.Width * num);
                byte* numPtr = (byte*) source.ImageData.ToPointer();
                byte* numPtr2 = (byte*) destination.ImageData.ToPointer();
                numPtr += (top * stride) + (left * num);
                numPtr2 += (top * num20) + (left * num);
                if (destination.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    for (int i = top; i < num5; i++)
                    {
                        int num24 = left;
                        while (num24 < num4)
                        {
                            num14 = divisor = num18 = 0;
                            num6 = 0;
                            while (num6 < this.size)
                            {
                                num10 = num6 - num12;
                                num8 = i + num10;
                                if (num8 >= top)
                                {
                                    if (num8 >= num5)
                                    {
                                        goto Label_017A;
                                    }
                                    num7 = 0;
                                    while (num7 < this.size)
                                    {
                                        num11 = num7 - num12;
                                        num8 = num24 + num11;
                                        if ((num8 >= left) && (num8 < num4))
                                        {
                                            num9 = this.kernel[num6, num7];
                                            divisor += num9;
                                            num14 += num9 * numPtr[(num10 * stride) + num11];
                                            num18++;
                                        }
                                        num7++;
                                    }
                                }
                                num6++;
                            }
                        Label_017A:
                            if (num18 == num17)
                            {
                                divisor = this.divisor;
                            }
                            else if (!this.dynamicDivisorForEdges)
                            {
                                divisor = this.divisor;
                            }
                            if (divisor != 0L)
                            {
                                num14 /= divisor;
                            }
                            num14 += this.threshold;
                            numPtr2[0] = (num14 > 0xffL) ? ((byte) 0xffL) : ((num14 < 0L) ? ((byte) 0L) : ((byte) num14));
                            num24++;
                            numPtr++;
                            numPtr2++;
                        }
                        numPtr += num21;
                        numPtr2 += num22;
                    }
                }
                else
                {
                    for (int j = top; j < num5; j++)
                    {
                        int num26 = left;
                        while (num26 < num4)
                        {
                            num13 = num14 = num15 = divisor = num18 = 0;
                            num6 = 0;
                            while (num6 < this.size)
                            {
                                num10 = num6 - num12;
                                num8 = j + num10;
                                if (num8 >= top)
                                {
                                    if (num8 >= num5)
                                    {
                                        goto Label_02F4;
                                    }
                                    num7 = 0;
                                    while (num7 < this.size)
                                    {
                                        num11 = num7 - num12;
                                        num8 = num26 + num11;
                                        if ((num8 >= left) && (num8 < num4))
                                        {
                                            num9 = this.kernel[num6, num7];
                                            byte* numPtr3 = numPtr + ((num10 * stride) + (num11 * num));
                                            divisor += num9;
                                            num13 += num9 * numPtr3[2];
                                            num14 += num9 * numPtr3[1];
                                            num15 += num9 * numPtr3[0];
                                            num18++;
                                        }
                                        num7++;
                                    }
                                }
                                num6++;
                            }
                        Label_02F4:
                            if (num18 == num17)
                            {
                                divisor = this.divisor;
                            }
                            else if (!this.dynamicDivisorForEdges)
                            {
                                divisor = this.divisor;
                            }
                            if (divisor != 0L)
                            {
                                num13 /= divisor;
                                num14 /= divisor;
                                num15 /= divisor;
                            }
                            num13 += this.threshold;
                            num14 += this.threshold;
                            num15 += this.threshold;
                            numPtr2[2] = (num13 > 0xffL) ? ((byte) 0xffL) : ((num13 < 0L) ? ((byte) 0L) : ((byte) num13));
                            numPtr2[1] = (num14 > 0xffL) ? ((byte) 0xffL) : ((num14 < 0L) ? ((byte) 0L) : ((byte) num14));
                            numPtr2[0] = (num15 > 0xffL) ? ((byte) 0xffL) : ((num15 < 0L) ? ((byte) 0L) : ((byte) num15));
                            if (num == 4)
                            {
                                numPtr2[3] = numPtr[3];
                            }
                            num26++;
                            numPtr += num;
                            numPtr2 += num;
                        }
                        numPtr += num21;
                        numPtr2 += num22;
                    }
                }
            }
            else
            {
                num /= 2;
                int num27 = destination.Stride / 2;
                int num28 = source.Stride / 2;
                ushort* numPtr4 = (ushort*) source.ImageData.ToPointer();
                ushort* numPtr5 = (ushort*) destination.ImageData.ToPointer();
                numPtr4 += left * num;
                numPtr5 += left * num;
                if (source.PixelFormat == PixelFormat.Format16bppGrayScale)
                {
                    for (int k = top; k < num5; k++)
                    {
                        ushort* numPtr7 = numPtr4 + (k * num28);
                        ushort* numPtr8 = numPtr5 + (k * num27);
                        int num30 = left;
                        while (num30 < num4)
                        {
                            num14 = divisor = num18 = 0;
                            num6 = 0;
                            while (num6 < this.size)
                            {
                                num10 = num6 - num12;
                                num8 = k + num10;
                                if (num8 >= top)
                                {
                                    if (num8 >= num5)
                                    {
                                        goto Label_0535;
                                    }
                                    num7 = 0;
                                    while (num7 < this.size)
                                    {
                                        num11 = num7 - num12;
                                        num8 = num30 + num11;
                                        if ((num8 >= left) && (num8 < num4))
                                        {
                                            num9 = this.kernel[num6, num7];
                                            divisor += num9;
                                            num14 += num9 * numPtr7[(num10 * num28) + num11];
                                            num18++;
                                        }
                                        num7++;
                                    }
                                }
                                num6++;
                            }
                        Label_0535:
                            if (num18 == num17)
                            {
                                divisor = this.divisor;
                            }
                            else if (!this.dynamicDivisorForEdges)
                            {
                                divisor = this.divisor;
                            }
                            if (divisor != 0L)
                            {
                                num14 /= divisor;
                            }
                            num14 += this.threshold;
                            numPtr8[0] = (num14 > 0xffffL) ? ((ushort) 0xffffL) : ((num14 < 0L) ? ((ushort) 0L) : ((ushort) num14));
                            num30++;
                            numPtr7++;
                            numPtr8++;
                        }
                    }
                }
                else
                {
                    for (int m = top; m < num5; m++)
                    {
                        ushort* numPtr9 = numPtr4 + (m * num28);
                        ushort* numPtr10 = numPtr5 + (m * num27);
                        int num32 = left;
                        while (num32 < num4)
                        {
                            num13 = num14 = num15 = divisor = num18 = 0;
                            for (num6 = 0; num6 < this.size; num6++)
                            {
                                num10 = num6 - num12;
                                num8 = m + num10;
                                if (num8 >= top)
                                {
                                    if (num8 >= num5)
                                    {
                                        goto Label_06BE;
                                    }
                                    for (num7 = 0; num7 < this.size; num7++)
                                    {
                                        num11 = num7 - num12;
                                        num8 = num32 + num11;
                                        if ((num8 >= left) && (num8 < num4))
                                        {
                                            num9 = this.kernel[num6, num7];
                                            ushort* numPtr6 = numPtr9 + ((num10 * num28) + (num11 * num));
                                            divisor += num9;
                                            num13 += num9 * numPtr6[2];
                                            num14 += num9 * numPtr6[1];
                                            num15 += num9 * numPtr6[0];
                                            num18++;
                                        }
                                    }
                                }
                            }
                        Label_06BE:
                            if (num18 == num17)
                            {
                                divisor = this.divisor;
                            }
                            else if (!this.dynamicDivisorForEdges)
                            {
                                divisor = this.divisor;
                            }
                            if (divisor != 0L)
                            {
                                num13 /= divisor;
                                num14 /= divisor;
                                num15 /= divisor;
                            }
                            num13 += this.threshold;
                            num14 += this.threshold;
                            num15 += this.threshold;
                            numPtr10[2] = (num13 > 0xffffL) ? ((ushort) 0xffffL) : ((num13 < 0L) ? ((ushort) 0L) : ((ushort) num13));
                            numPtr10[1] = (num14 > 0xffffL) ? ((ushort) 0xffffL) : ((num14 < 0L) ? ((ushort) 0L) : ((ushort) num14));
                            numPtr10[0] = (num15 > 0xffffL) ? ((ushort) 0xffffL) : ((num15 < 0L) ? ((ushort) 0L) : ((ushort) num15));
                            if (num == 4)
                            {
                                numPtr10[3] = numPtr9[3];
                            }
                            num32++;
                            numPtr9 += num;
                            numPtr10 += num;
                        }
                    }
                }
            }
        }

        public int Divisor
        {
            get
            {
                return this.divisor;
            }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentException("Divisor can not be equal to zero.");
                }
                this.divisor = value;
            }
        }

        public bool DynamicDivisorForEdges
        {
            get
            {
                return this.dynamicDivisorForEdges;
            }
            set
            {
                this.dynamicDivisorForEdges = value;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public int[,] Kernel
        {
            get
            {
                return this.kernel;
            }
            set
            {
                int length = value.GetLength(0);
                if (((length != value.GetLength(1)) || (length < 3)) || ((length > 0x63) || ((length % 2) == 0)))
                {
                    throw new ArgumentException("Invalid kernel size.");
                }
                this.kernel = value;
                this.size = length;
            }
        }

        public int Threshold
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

