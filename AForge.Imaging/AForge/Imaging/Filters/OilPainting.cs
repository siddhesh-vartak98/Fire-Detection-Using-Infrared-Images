namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class OilPainting : BaseUsingCopyPartialFilter
    {
        private int brushSize;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;

        public OilPainting()
        {
            this.brushSize = 5;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        public OilPainting(int brushSize) : this()
        {
            this.BrushSize = brushSize;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
        {
            int num10;
            int num11;
            int num12;
            byte num14;
            byte num15;
            int num = Image.GetPixelFormatSize(source.PixelFormat) / 8;
            int left = rect.Left;
            int top = rect.Top;
            int num4 = left + rect.Width;
            int num5 = top + rect.Height;
            int stride = source.Stride;
            int num7 = destination.Stride;
            int num8 = stride - (rect.Width * num);
            int num9 = stride - (rect.Width * num);
            int num13 = this.brushSize >> 1;
            int[] array = new int[0x100];
            byte* numPtr = (byte*) source.ImageData.ToPointer();
            byte* numPtr2 = (byte*) destination.ImageData.ToPointer();
            numPtr += (top * stride) + (left * num);
            numPtr2 += (top * num7) + (left * num);
            if (destination.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                for (int i = top; i < num5; i++)
                {
                    int num17 = left;
                    while (num17 < num4)
                    {
                        Array.Clear(array, 0, 0x100);
                        num10 = -num13;
                        while (num10 <= num13)
                        {
                            num12 = i + num10;
                            if (num12 >= top)
                            {
                                if (num12 >= num5)
                                {
                                    goto Label_014B;
                                }
                                num11 = -num13;
                                while (num11 <= num13)
                                {
                                    num12 = num17 + num11;
                                    if ((num12 >= left) && (num12 < num4))
                                    {
                                        num14 = numPtr[(num10 * stride) + num11];
                                        array[num14]++;
                                    }
                                    num11++;
                                }
                            }
                            num10++;
                        }
                    Label_014B:
                        num15 = 0;
                        num11 = 0;
                        num10 = 0;
                        while (num10 < 0x100)
                        {
                            if (array[num10] > num11)
                            {
                                num15 = (byte) num10;
                                num11 = array[num10];
                            }
                            num10++;
                        }
                        numPtr2[0] = num15;
                        num17++;
                        numPtr++;
                        numPtr2++;
                    }
                    numPtr += num8;
                    numPtr2 += num9;
                }
            }
            else
            {
                int[] numArray2 = new int[0x100];
                int[] numArray3 = new int[0x100];
                int[] numArray4 = new int[0x100];
                for (int j = top; j < num5; j++)
                {
                    int num19 = left;
                    while (num19 < num4)
                    {
                        Array.Clear(array, 0, 0x100);
                        Array.Clear(numArray2, 0, 0x100);
                        Array.Clear(numArray3, 0, 0x100);
                        Array.Clear(numArray4, 0, 0x100);
                        num10 = -num13;
                        while (num10 <= num13)
                        {
                            num12 = j + num10;
                            if (num12 >= top)
                            {
                                if (num12 >= num5)
                                {
                                    goto Label_032B;
                                }
                                num11 = -num13;
                                while (num11 <= num13)
                                {
                                    num12 = num19 + num11;
                                    if ((num12 >= left) && (num12 < num4))
                                    {
                                        byte* numPtr3 = numPtr + ((num10 * stride) + (num11 * num));
                                        num14 = (byte) (((0.2125 * numPtr3[2]) + (0.7154 * numPtr3[1])) + (0.0721 * numPtr3[0]));
                                        array[num14]++;
                                        numArray2[num14] += numPtr3[2];
                                        numArray3[num14] += numPtr3[1];
                                        numArray4[num14] += numPtr3[0];
                                    }
                                    num11++;
                                }
                            }
                            num10++;
                        }
                    Label_032B:
                        num15 = 0;
                        num11 = 0;
                        for (num10 = 0; num10 < 0x100; num10++)
                        {
                            if (array[num10] > num11)
                            {
                                num15 = (byte) num10;
                                num11 = array[num10];
                            }
                        }
                        numPtr2[2] = (byte) (numArray2[num15] / array[num15]);
                        numPtr2[1] = (byte) (numArray3[num15] / array[num15]);
                        numPtr2[0] = (byte) (numArray4[num15] / array[num15]);
                        num19++;
                        numPtr += num;
                        numPtr2 += num;
                    }
                    numPtr += num8;
                    numPtr2 += num9;
                }
            }
        }

        public int BrushSize
        {
            get
            {
                return this.brushSize;
            }
            set
            {
                this.brushSize = Math.Max(3, Math.Min(0x15, value | 1));
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

