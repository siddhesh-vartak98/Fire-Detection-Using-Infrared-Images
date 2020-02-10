namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class BlobsFiltering : BaseInPlaceFilter
    {
        private BlobCounter blobCounter;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;

        public BlobsFiltering()
        {
            this.blobCounter = new BlobCounter();
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.blobCounter.FilterBlobs = true;
            this.blobCounter.MinWidth = 1;
            this.blobCounter.MinHeight = 1;
            this.blobCounter.MaxWidth = 0x7fffffff;
            this.blobCounter.MaxHeight = 0x7fffffff;
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            this.formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
        }

        public BlobsFiltering(IBlobsFilter blobsFilter) : this()
        {
            this.blobCounter.BlobsFilter = blobsFilter;
        }

        public BlobsFiltering(int minWidth, int minHeight, int maxWidth, int maxHeight) : this(minWidth, minHeight, maxWidth, maxHeight, false)
        {
        }

        public BlobsFiltering(int minWidth, int minHeight, int maxWidth, int maxHeight, bool coupledSizeFiltering) : this()
        {
            this.blobCounter.MinWidth = minWidth;
            this.blobCounter.MinHeight = minHeight;
            this.blobCounter.MaxWidth = maxWidth;
            this.blobCounter.MaxHeight = maxHeight;
            this.blobCounter.CoupledSizeFiltering = coupledSizeFiltering;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image)
        {
            this.blobCounter.ProcessImage(image);
            int[] objectLabels = this.blobCounter.ObjectLabels;
            int width = image.Width;
            int height = image.Height;
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                int num3 = image.Stride - width;
                int num4 = 0;
                int index = 0;
                while (num4 < height)
                {
                    int num6 = 0;
                    while (num6 < width)
                    {
                        if (objectLabels[index] == 0)
                        {
                            numPtr[0] = 0;
                        }
                        num6++;
                        numPtr++;
                        index++;
                    }
                    numPtr += num3;
                    num4++;
                }
            }
            else
            {
                int num7 = Image.GetPixelFormatSize(image.PixelFormat) / 8;
                int num8 = image.Stride - (width * num7);
                int num9 = 0;
                int num10 = 0;
                while (num9 < height)
                {
                    int num11 = 0;
                    while (num11 < width)
                    {
                        if (objectLabels[num10] == 0)
                        {
                            byte num12;
                            numPtr[0] = (byte) (num12 = 0);
                            numPtr[2] = numPtr[1] = num12;
                        }
                        num11++;
                        numPtr += num7;
                        num10++;
                    }
                    numPtr += num8;
                    num9++;
                }
            }
        }

        public IBlobsFilter BlobsFilter
        {
            get
            {
                return this.blobCounter.BlobsFilter;
            }
            set
            {
                this.blobCounter.BlobsFilter = value;
            }
        }

        public bool CoupledSizeFiltering
        {
            get
            {
                return this.blobCounter.CoupledSizeFiltering;
            }
            set
            {
                this.blobCounter.CoupledSizeFiltering = value;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public int MaxHeight
        {
            get
            {
                return this.blobCounter.MaxHeight;
            }
            set
            {
                this.blobCounter.MaxHeight = value;
            }
        }

        public int MaxWidth
        {
            get
            {
                return this.blobCounter.MaxWidth;
            }
            set
            {
                this.blobCounter.MaxWidth = value;
            }
        }

        public int MinHeight
        {
            get
            {
                return this.blobCounter.MinHeight;
            }
            set
            {
                this.blobCounter.MinHeight = value;
            }
        }

        public int MinWidth
        {
            get
            {
                return this.blobCounter.MinWidth;
            }
            set
            {
                this.blobCounter.MinWidth = value;
            }
        }
    }
}

