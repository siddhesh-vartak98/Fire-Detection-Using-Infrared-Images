namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;

    public class Grayscale : BaseFilter
    {
        public readonly double BlueCoefficient;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
        public readonly double GreenCoefficient;
        public readonly double RedCoefficient;

        public Grayscale(double cr, double cg, double cb)
        {
            this.RedCoefficient = cr;
            this.GreenCoefficient = cg;
            this.BlueCoefficient = cb;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format16bppGrayScale;
            this.formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format16bppGrayScale;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            int width = sourceData.Width;
            int height = sourceData.Height;
            PixelFormat pixelFormat = sourceData.PixelFormat;
            switch (pixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                {
                    int num3 = (pixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
                    int num4 = sourceData.Stride - (width * num3);
                    int num5 = destinationData.Stride - width;
                    byte* numPtr = (byte*) sourceData.ImageData.ToPointer();
                    byte* numPtr2 = (byte*) destinationData.ImageData.ToPointer();
                    for (int j = 0; j < height; j++)
                    {
                        int num7 = 0;
                        while (num7 < width)
                        {
                            numPtr2[0] = (byte) (((this.RedCoefficient * numPtr[2]) + (this.GreenCoefficient * numPtr[1])) + (this.BlueCoefficient * numPtr[0]));
                            num7++;
                            numPtr += num3;
                            numPtr2++;
                        }
                        numPtr += num4;
                        numPtr2 += num5;
                    }
                    return;
                }
            }
            int num8 = (pixelFormat == PixelFormat.Format48bppRgb) ? 3 : 4;
            int num9 = (int) sourceData.ImageData.ToPointer();
            int num10 = (int) destinationData.ImageData.ToPointer();
            int stride = sourceData.Stride;
            int num12 = destinationData.Stride;
            for (int i = 0; i < height; i++)
            {
                ushort* numPtr3 = (ushort*) (num9 + (i * stride));
                ushort* numPtr4 = (ushort*) (num10 + (i * num12));
                int num14 = 0;
                while (num14 < width)
                {
                    numPtr4[0] = (ushort) (((this.RedCoefficient * numPtr3[2]) + (this.GreenCoefficient * numPtr3[1])) + (this.BlueCoefficient * numPtr3[0]));
                    num14++;
                    numPtr3 += num8;
                    numPtr4++;
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

        public static class CommonAlgorithms
        {
            public static readonly Grayscale BT709 = new Grayscale(0.2125, 0.7154, 0.0721);
            public static readonly Grayscale RMY = new Grayscale(0.5, 0.419, 0.081);
            public static readonly Grayscale Y = new Grayscale(0.299, 0.587, 0.114);
        }
    }
}

