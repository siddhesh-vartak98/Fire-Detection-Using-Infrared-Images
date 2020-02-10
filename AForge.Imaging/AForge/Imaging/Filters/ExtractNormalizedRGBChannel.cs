namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ExtractNormalizedRGBChannel : BaseFilter
    {
        private short channel;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;

        public ExtractNormalizedRGBChannel()
        {
            this.channel = 2;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format16bppGrayScale;
            this.formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format16bppGrayScale;
        }

        public ExtractNormalizedRGBChannel(short channel) : this()
        {
            this.Channel = channel;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            int num4;
            int width = sourceData.Width;
            int height = sourceData.Height;
            int num3 = Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
            if (num3 <= 4)
            {
                int num5 = sourceData.Stride - (width * num3);
                int num6 = destinationData.Stride - width;
                byte* numPtr = (byte*) sourceData.ImageData.ToPointer();
                byte* numPtr2 = (byte*) destinationData.ImageData.ToPointer();
                for (int i = 0; i < height; i++)
                {
                    int num8 = 0;
                    while (num8 < width)
                    {
                        num4 = (numPtr[2] + numPtr[1]) + numPtr[0];
                        numPtr2[0] = (num4 != 0) ? ((byte) ((0xff * numPtr[this.channel]) / num4)) : ((byte) 0);
                        num8++;
                        numPtr += num3;
                        numPtr2++;
                    }
                    numPtr += num5;
                    numPtr2 += num6;
                }
            }
            else
            {
                num3 /= 2;
                int num9 = (int) sourceData.ImageData.ToPointer();
                int num10 = (int) destinationData.ImageData.ToPointer();
                int stride = sourceData.Stride;
                int num12 = destinationData.Stride;
                for (int j = 0; j < height; j++)
                {
                    ushort* numPtr3 = (ushort*) (num9 + (j * stride));
                    ushort* numPtr4 = (ushort*) (num10 + (j * num12));
                    int num14 = 0;
                    while (num14 < width)
                    {
                        num4 = (numPtr3[2] + numPtr3[1]) + numPtr3[0];
                        numPtr4[0] = (num4 != 0) ? ((ushort) ((0xffff * numPtr3[this.channel]) / num4)) : ((ushort) 0);
                        num14++;
                        numPtr3 += num3;
                        numPtr4++;
                    }
                }
            }
        }

        public short Channel
        {
            get
            {
                return this.channel;
            }
            set
            {
                if (((value != 2) && (value != 1)) && (value != 0))
                {
                    throw new ArgumentException("Invalid channel is specified.");
                }
                this.channel = value;
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

