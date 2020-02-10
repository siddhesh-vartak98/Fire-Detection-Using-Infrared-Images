namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ExtractChannel : BaseFilter
    {
        private short channel;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;

        public ExtractChannel()
        {
            this.channel = 2;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format16bppGrayScale;
            this.formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format16bppGrayScale;
        }

        public ExtractChannel(short channel) : this()
        {
            this.Channel = channel;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            int width = sourceData.Width;
            int height = sourceData.Height;
            int num3 = Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
            if (((this.channel == 3) && (num3 != 4)) && (num3 != 8))
            {
                throw new InvalidImagePropertiesException("Can not extract alpha channel from none ARGB image.");
            }
            if (num3 <= 4)
            {
                int num4 = sourceData.Stride - (width * num3);
                int num5 = destinationData.Stride - width;
                byte* numPtr = (byte*) sourceData.ImageData.ToPointer();
                byte* numPtr2 = (byte*) destinationData.ImageData.ToPointer();
                numPtr += this.channel;
                for (int i = 0; i < height; i++)
                {
                    int num7 = 0;
                    while (num7 < width)
                    {
                        numPtr2[0] = numPtr[0];
                        num7++;
                        numPtr += num3;
                        numPtr2++;
                    }
                    numPtr += num4;
                    numPtr2 += num5;
                }
            }
            else
            {
                num3 /= 2;
                int num8 = (int) sourceData.ImageData.ToPointer();
                int num9 = (int) destinationData.ImageData.ToPointer();
                int stride = sourceData.Stride;
                int num11 = destinationData.Stride;
                for (int j = 0; j < height; j++)
                {
                    ushort* numPtr3 = (ushort*) (num8 + (j * stride));
                    ushort* numPtr4 = (ushort*) (num9 + (j * num11));
                    numPtr3 += this.channel;
                    int num13 = 0;
                    while (num13 < width)
                    {
                        numPtr4[0] = numPtr3[0];
                        num13++;
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
                if (((value != 2) && (value != 1)) && ((value != 0) && (value != 3)))
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

