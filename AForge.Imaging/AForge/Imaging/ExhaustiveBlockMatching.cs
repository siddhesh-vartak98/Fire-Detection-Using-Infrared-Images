namespace AForge.Imaging
{
    using AForge;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ExhaustiveBlockMatching : IBlockMatching
    {
        private int blockSize;
        private int searchRadius;
        private float similarityThreshold;

        public ExhaustiveBlockMatching()
        {
            this.blockSize = 0x10;
            this.searchRadius = 12;
            this.similarityThreshold = 0.9f;
        }

        public ExhaustiveBlockMatching(int blockSize, int searchRadius)
        {
            this.blockSize = 0x10;
            this.searchRadius = 12;
            this.similarityThreshold = 0.9f;
            this.blockSize = blockSize;
            this.searchRadius = searchRadius;
        }

        public unsafe List<BlockMatch> ProcessImage(UnmanagedImage sourceImage, List<IntPoint> coordinates, UnmanagedImage searchImage)
        {
            if ((sourceImage.Width != searchImage.Width) || (sourceImage.Height != searchImage.Height))
            {
                throw new InvalidImagePropertiesException("Source and search images sizes must match");
            }
            if ((sourceImage.PixelFormat != PixelFormat.Format8bppIndexed) && (sourceImage.PixelFormat != PixelFormat.Format24bppRgb))
            {
                throw new UnsupportedImageFormatException("Source images can be graysclae (8 bpp indexed) or color (24 bpp) image only");
            }
            if (sourceImage.PixelFormat != searchImage.PixelFormat)
            {
                throw new InvalidImagePropertiesException("Source and search images must have same pixel format");
            }
            int count = coordinates.Count;
            List<BlockMatch> list = new List<BlockMatch>();
            int width = sourceImage.Width;
            int height = sourceImage.Height;
            int stride = sourceImage.Stride;
            int num5 = (sourceImage.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
            int num6 = this.blockSize / 2;
            int num7 = 2 * this.searchRadius;
            int num8 = this.blockSize * num5;
            int num9 = stride - (this.blockSize * num5);
            int num10 = ((this.blockSize * this.blockSize) * num5) * 0xff;
            int num11 = (int) (this.similarityThreshold * num10);
            byte* numPtr = (byte*) sourceImage.ImageData.ToPointer();
            byte* numPtr2 = (byte*) searchImage.ImageData.ToPointer();
            for (int i = 0; i < count; i++)
            {
                int x = coordinates[i].X;
                int y = coordinates[i].Y;
                if ((((x - num6) >= 0) && ((x + num6) < width)) && (((y - num6) >= 0) && ((y + num6) < height)))
                {
                    int num15 = (x - num6) - this.searchRadius;
                    int num16 = (y - num6) - this.searchRadius;
                    int num17 = x;
                    int num18 = y;
                    int num19 = 0x7fffffff;
                    for (int j = 0; j < num7; j++)
                    {
                        if (((num16 + j) >= 0) && (((num16 + j) + this.blockSize) < height))
                        {
                            for (int k = 0; k < num7; k++)
                            {
                                int num22 = num15 + k;
                                int num23 = num16 + j;
                                if ((num22 >= 0) && ((num23 + this.blockSize) < width))
                                {
                                    byte* numPtr3 = (numPtr + ((y - num6) * stride)) + ((x - num6) * num5);
                                    byte* numPtr4 = (numPtr2 + (num23 * stride)) + (num22 * num5);
                                    int num24 = 0;
                                    for (int m = 0; m < this.blockSize; m++)
                                    {
                                        int num26 = 0;
                                        while (num26 < num8)
                                        {
                                            int num27 = numPtr3[0] - numPtr4[0];
                                            if (num27 > 0)
                                            {
                                                num24 += num27;
                                            }
                                            else
                                            {
                                                num24 -= num27;
                                            }
                                            num26++;
                                            numPtr3++;
                                            numPtr4++;
                                        }
                                        numPtr3 += num9;
                                        numPtr4 += num9;
                                    }
                                    if (num24 < num19)
                                    {
                                        num19 = num24;
                                        num17 = num22 + num6;
                                        num18 = num23 + num6;
                                    }
                                }
                            }
                        }
                    }
                    int num28 = num10 - num19;
                    if (num28 >= num11)
                    {
                        list.Add(new BlockMatch(new IntPoint(x, y), new IntPoint(num17, num18), ((float) num28) / ((float) num10)));
                    }
                }
            }
            list.Sort(new MatchingsSorter());
            return list;
        }

        public List<BlockMatch> ProcessImage(Bitmap sourceImage, List<IntPoint> coordinates, Bitmap searchImage)
        {
            List<BlockMatch> list;
            BitmapData bitmapData = sourceImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            BitmapData data2 = searchImage.LockBits(new Rectangle(0, 0, searchImage.Width, searchImage.Height), ImageLockMode.ReadOnly, searchImage.PixelFormat);
            try
            {
                list = this.ProcessImage(new UnmanagedImage(bitmapData), coordinates, new UnmanagedImage(data2));
            }
            finally
            {
                sourceImage.UnlockBits(bitmapData);
                searchImage.UnlockBits(data2);
            }
            return list;
        }

        public List<BlockMatch> ProcessImage(BitmapData sourceImageData, List<IntPoint> coordinates, BitmapData searchImageData)
        {
            return this.ProcessImage(new UnmanagedImage(sourceImageData), coordinates, new UnmanagedImage(searchImageData));
        }

        public int BlockSize
        {
            get
            {
                return this.blockSize;
            }
            set
            {
                this.blockSize = value;
            }
        }

        public int SearchRadius
        {
            get
            {
                return this.searchRadius;
            }
            set
            {
                this.searchRadius = value;
            }
        }

        public float SimilarityThreshold
        {
            get
            {
                return this.similarityThreshold;
            }
            set
            {
                this.similarityThreshold = Math.Min(1f, Math.Max(0f, value));
            }
        }

        private class MatchingsSorter : IComparer<BlockMatch>
        {
            public int Compare(BlockMatch x, BlockMatch y)
            {
                float num = y.Similarity - x.Similarity;
                if (num > 0f)
                {
                    return 1;
                }
                if (num >= 0f)
                {
                    return 0;
                }
                return -1;
            }
        }
    }
}

