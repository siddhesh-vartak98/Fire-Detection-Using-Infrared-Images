namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class HitAndMiss : BaseUsingCopyPartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private Modes mode;
        private short[,] se;
        private int size;

        public HitAndMiss(short[,] se)
        {
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            int length = se.GetLength(0);
            if (((length != se.GetLength(1)) || (length < 3)) || ((length > 0x63) || ((length % 2) == 0)))
            {
                throw new ArgumentException();
            }
            this.se = se;
            this.size = length;
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        public HitAndMiss(short[,] se, Modes mode) : this(se)
        {
            this.mode = mode;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData, Rectangle rect)
        {
            int left = rect.Left;
            int top = rect.Top;
            int num3 = left + rect.Width;
            int num4 = top + rect.Height;
            int stride = sourceData.Stride;
            int num6 = destinationData.Stride;
            int num7 = stride - rect.Width;
            int num8 = num6 - rect.Width;
            int num13 = this.size >> 1;
            byte[] buffer3 = new byte[3];
            buffer3[0] = 0xff;
            buffer3[2] = 0xff;
            byte[] buffer = buffer3;
            byte[] buffer2 = new byte[3];
            int mode = (int) this.mode;
            byte* numPtr = (byte*) sourceData.ImageData.ToPointer();
            byte* numPtr2 = (byte*) destinationData.ImageData.ToPointer();
            numPtr += (top * stride) + left;
            numPtr2 += (top * num6) + left;
            for (int i = top; i < num4; i++)
            {
                int num19 = left;
                while (num19 < num3)
                {
                    buffer2[1] = buffer2[2] = numPtr[0];
                    byte num14 = 0xff;
                    for (int j = 0; j < this.size; j++)
                    {
                        int num9 = j - num13;
                        for (int k = 0; k < this.size; k++)
                        {
                            int num10 = k - num13;
                            short num16 = this.se[j, k];
                            if (num16 != -1)
                            {
                                if ((((i + num9) < top) || ((i + num9) >= num4)) || (((num19 + num10) < left) || ((num19 + num10) >= num3)))
                                {
                                    num14 = 0;
                                    break;
                                }
                                byte num15 = numPtr[(num9 * stride) + num10];
                                if (((num16 != 0) || (num15 != 0)) && ((num16 != 1) || (num15 != 0xff)))
                                {
                                    num14 = 0;
                                    break;
                                }
                            }
                        }
                        if (num14 == 0)
                        {
                            break;
                        }
                    }
                    numPtr2[0] = (num14 == 0xff) ? buffer[mode] : buffer2[mode];
                    num19++;
                    numPtr++;
                    numPtr2++;
                }
                numPtr += num7;
                numPtr2 += num8;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public Modes Mode
        {
            get
            {
                return this.mode;
            }
            set
            {
                this.mode = value;
            }
        }

        public enum Modes
        {
            HitAndMiss,
            Thinning,
            Thickening
        }
    }
}

