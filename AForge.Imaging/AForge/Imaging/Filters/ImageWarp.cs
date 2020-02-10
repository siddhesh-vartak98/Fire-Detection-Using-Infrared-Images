namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ImageWarp : BaseFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
        private IntPoint[,] warpMap;

        public ImageWarp(IntPoint[,] warpMap)
        {
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            this.WarpMap = warpMap;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage source, UnmanagedImage destination)
        {
            int num = Image.GetPixelFormatSize(source.PixelFormat) / 8;
            int width = source.Width;
            int height = source.Height;
            int num4 = Math.Min(width, this.warpMap.GetLength(1));
            int num5 = Math.Min(height, this.warpMap.GetLength(0));
            int stride = source.Stride;
            int num7 = destination.Stride;
            int num8 = num7 - (num4 * num);
            byte* numPtr = (byte*) source.ImageData.ToPointer();
            byte* dst = (byte*) destination.ImageData.ToPointer();
            for (int i = 0; i < num5; i++)
            {
                for (int j = 0; j < num4; j++)
                {
                    int num9 = j + this.warpMap[i, j].X;
                    int num10 = i + this.warpMap[i, j].Y;
                    if (((num9 >= 0) && (num10 >= 0)) && ((num9 < width) && (num10 < height)))
                    {
                        byte* numPtr3 = (numPtr + (num10 * stride)) + (num9 * num);
                        int num13 = 0;
                        while (num13 < num)
                        {
                            dst[0] = numPtr3[0];
                            num13++;
                            dst++;
                            numPtr3++;
                        }
                    }
                    else
                    {
                        int num14 = 0;
                        while (num14 < num)
                        {
                            dst[0] = 0;
                            num14++;
                            dst++;
                        }
                    }
                }
                if (width != num4)
                {
                    SystemTools.CopyUnmanagedMemory(dst, (numPtr + (i * stride)) + (num4 * num), (width - num4) * num);
                }
                dst += num8;
            }
            int num15 = num5;
            while (num15 < height)
            {
                SystemTools.CopyUnmanagedMemory(dst, numPtr + (num15 * stride), width * num);
                num15++;
                dst += num7;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public IntPoint[,] WarpMap
        {
            get
            {
                return this.warpMap;
            }
            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("Warp map can not be set to null.");
                }
                this.warpMap = value;
            }
        }
    }
}

