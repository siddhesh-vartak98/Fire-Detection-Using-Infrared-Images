namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class Erosion : BaseUsingCopyPartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private short[,] se;
        private int size;

        public Erosion()
        {
            this.se = new short[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
            this.size = 3;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            this.formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
        }

        public Erosion(short[,] se) : this()
        {
            int length = se.GetLength(0);
            if (((length != se.GetLength(1)) || (length < 3)) || ((length > 0x63) || ((length % 2) == 0)))
            {
                throw new ArgumentException("Invalid size of structuring element.");
            }
            this.se = se;
            this.size = length;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData, Rectangle rect)
        {
            PixelFormat pixelFormat = sourceData.PixelFormat;
            int left = rect.Left;
            int top = rect.Top;
            int num3 = left + rect.Width;
            int num4 = top + rect.Height;
            int num5 = this.size >> 1;
            switch (pixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                case PixelFormat.Format24bppRgb:
                {
                    int num6 = (pixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
                    int stride = destinationData.Stride;
                    int num8 = sourceData.Stride;
                    byte* numPtr = (byte*) sourceData.ImageData.ToPointer();
                    byte* numPtr2 = (byte*) destinationData.ImageData.ToPointer();
                    numPtr += left * num6;
                    numPtr2 += left * num6;
                    if (pixelFormat == PixelFormat.Format8bppIndexed)
                    {
                        for (int j = top; j < num4; j++)
                        {
                            byte* numPtr3 = numPtr + (j * num8);
                            byte* numPtr4 = numPtr2 + (j * stride);
                            int num17 = left;
                            while (num17 < num3)
                            {
                                byte num10 = 0xff;
                                for (int k = 0; k < this.size; k++)
                                {
                                    int num13 = k - num5;
                                    int num12 = j + num13;
                                    if (num12 >= top)
                                    {
                                        if (num12 >= num4)
                                        {
                                            goto Label_015A;
                                        }
                                        for (int m = 0; m < this.size; m++)
                                        {
                                            int num14 = m - num5;
                                            num12 = num17 + num14;
                                            if (((num12 >= left) && (num12 < num3)) && (this.se[k, m] == 1))
                                            {
                                                byte num11 = numPtr3[(num13 * num8) + num14];
                                                if (num11 < num10)
                                                {
                                                    num10 = num11;
                                                }
                                            }
                                        }
                                    }
                                }
                            Label_015A:
                                numPtr4[0] = num10;
                                num17++;
                                numPtr3++;
                                numPtr4++;
                            }
                        }
                        return;
                    }
                    for (int i = top; i < num4; i++)
                    {
                        byte* numPtr5 = numPtr + (i * num8);
                        byte* numPtr6 = numPtr2 + (i * stride);
                        int num28 = left;
                        while (num28 < num3)
                        {
                            byte num20;
                            byte num21;
                            byte num19 = num20 = (byte) (num21 = 0xff);
                            for (int n = 0; n < this.size; n++)
                            {
                                int num24 = n - num5;
                                int num23 = i + num24;
                                if (num23 >= top)
                                {
                                    if (num23 >= num4)
                                    {
                                        goto Label_027A;
                                    }
                                    for (int num27 = 0; num27 < this.size; num27++)
                                    {
                                        int num25 = num27 - num5;
                                        num23 = num28 + num25;
                                        if (((num23 >= left) && (num23 < num3)) && (this.se[n, num27] == 1))
                                        {
                                            byte* numPtr7 = numPtr5 + ((num24 * num8) + (num25 * 3));
                                            byte num22 = numPtr7[2];
                                            if (num22 < num19)
                                            {
                                                num19 = num22;
                                            }
                                            num22 = numPtr7[1];
                                            if (num22 < num20)
                                            {
                                                num20 = num22;
                                            }
                                            num22 = numPtr7[0];
                                            if (num22 < num21)
                                            {
                                                num21 = num22;
                                            }
                                        }
                                    }
                                }
                            }
                        Label_027A:
                            numPtr6[2] = num19;
                            numPtr6[1] = num20;
                            numPtr6[0] = num21;
                            num28++;
                            numPtr5 += 3;
                            numPtr6 += 3;
                        }
                    }
                    return;
                }
            }
            int num29 = (pixelFormat == PixelFormat.Format16bppGrayScale) ? 1 : 3;
            int num30 = destinationData.Stride / 2;
            int num31 = sourceData.Stride / 2;
            ushort* numPtr8 = (ushort*) sourceData.ImageData.ToPointer();
            ushort* numPtr9 = (ushort*) destinationData.ImageData.ToPointer();
            numPtr8 += left * num29;
            numPtr9 += left * num29;
            if (pixelFormat == PixelFormat.Format16bppGrayScale)
            {
                for (int num32 = top; num32 < num4; num32++)
                {
                    ushort* numPtr10 = numPtr8 + (num32 * num31);
                    ushort* numPtr11 = numPtr9 + (num32 * num30);
                    int num40 = left;
                    while (num40 < num3)
                    {
                        ushort num33 = 0xffff;
                        for (int num38 = 0; num38 < this.size; num38++)
                        {
                            int num36 = num38 - num5;
                            int num35 = num32 + num36;
                            if (num35 >= top)
                            {
                                if (num35 >= num4)
                                {
                                    goto Label_03DE;
                                }
                                for (int num39 = 0; num39 < this.size; num39++)
                                {
                                    int num37 = num39 - num5;
                                    num35 = num40 + num37;
                                    if (((num35 >= left) && (num35 < num3)) && (this.se[num38, num39] == 1))
                                    {
                                        ushort num34 = numPtr10[(num36 * num31) + num37];
                                        if (num34 < num33)
                                        {
                                            num33 = num34;
                                        }
                                    }
                                }
                            }
                        }
                    Label_03DE:
                        numPtr11[0] = num33;
                        num40++;
                        numPtr10++;
                        numPtr11++;
                    }
                }
            }
            else
            {
                for (int num41 = top; num41 < num4; num41++)
                {
                    ushort* numPtr12 = numPtr8 + (num41 * num31);
                    ushort* numPtr13 = numPtr9 + (num41 * num30);
                    int num51 = left;
                    while (num51 < num3)
                    {
                        ushort num43;
                        ushort num44;
                        ushort num42 = num43 = (ushort) (num44 = 0xffff);
                        for (int num49 = 0; num49 < this.size; num49++)
                        {
                            int num47 = num49 - num5;
                            int num46 = num41 + num47;
                            if (num46 >= top)
                            {
                                if (num46 >= num4)
                                {
                                    goto Label_0505;
                                }
                                for (int num50 = 0; num50 < this.size; num50++)
                                {
                                    int num48 = num50 - num5;
                                    num46 = num51 + num48;
                                    if (((num46 >= left) && (num46 < num3)) && (this.se[num49, num50] == 1))
                                    {
                                        ushort* numPtr14 = numPtr12 + ((num47 * num31) + (num48 * 3));
                                        ushort num45 = numPtr14[2];
                                        if (num45 < num42)
                                        {
                                            num42 = num45;
                                        }
                                        num45 = numPtr14[1];
                                        if (num45 < num43)
                                        {
                                            num43 = num45;
                                        }
                                        num45 = numPtr14[0];
                                        if (num45 < num44)
                                        {
                                            num44 = num45;
                                        }
                                    }
                                }
                            }
                        }
                    Label_0505:
                        numPtr13[2] = num42;
                        numPtr13[1] = num43;
                        numPtr13[0] = num44;
                        num51++;
                        numPtr12 += 3;
                        numPtr13 += 3;
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
    }
}

