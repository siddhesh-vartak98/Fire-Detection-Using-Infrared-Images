namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ConnectedComponentsLabeling : BaseFilter
    {
        private BlobCounterBase blobCounter = new AForge.Imaging.BlobCounter();
        private static Color[] colorTable = new Color[] { 
            Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Violet, Color.Brown, Color.Olive, Color.Cyan, Color.Magenta, Color.Gold, Color.Indigo, Color.Ivory, Color.HotPink, Color.DarkRed, Color.DarkGreen, Color.DarkBlue, 
            Color.DarkSeaGreen, Color.Gray, Color.DarkKhaki, Color.DarkGray, Color.LimeGreen, Color.Tomato, Color.SteelBlue, Color.SkyBlue, Color.Silver, Color.Salmon, Color.SaddleBrown, Color.RosyBrown, Color.PowderBlue, Color.Plum, Color.PapayaWhip, Color.Orange
         };
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        public ConnectedComponentsLabeling()
        {
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format24bppRgb;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            this.blobCounter.ProcessImage(sourceData);
            int[] objectLabels = this.blobCounter.ObjectLabels;
            int width = sourceData.Width;
            int height = sourceData.Height;
            int num3 = destinationData.Stride - (width * 3);
            byte* numPtr = (byte*) destinationData.ImageData.ToPointer();
            int index = 0;
            for (int i = 0; i < height; i++)
            {
                int num6 = 0;
                while (num6 < width)
                {
                    if (objectLabels[index] != 0)
                    {
                        Color color = colorTable[(objectLabels[index] - 1) % colorTable.Length];
                        numPtr[2] = color.R;
                        numPtr[1] = color.G;
                        numPtr[0] = color.B;
                    }
                    num6++;
                    numPtr += 3;
                    index++;
                }
                numPtr += num3;
            }
        }

        public BlobCounterBase BlobCounter
        {
            get
            {
                return this.blobCounter;
            }
            set
            {
                this.blobCounter = value;
            }
        }

        public static Color[] ColorTable
        {
            get
            {
                return colorTable;
            }
            set
            {
                colorTable = value;
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

        public bool FilterBlobs
        {
            get
            {
                return this.blobCounter.FilterBlobs;
            }
            set
            {
                this.blobCounter.FilterBlobs = value;
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

        public int ObjectCount
        {
            get
            {
                return this.blobCounter.ObjectsCount;
            }
        }
    }
}

