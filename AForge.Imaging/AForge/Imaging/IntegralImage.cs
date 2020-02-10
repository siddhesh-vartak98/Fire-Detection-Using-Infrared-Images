namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class IntegralImage
    {
        private int height;
        protected uint[,] integralImage;
        private int width;

        protected IntegralImage(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.integralImage = new uint[height + 1, width + 1];
        }

        public static unsafe IntegralImage FromBitmap(UnmanagedImage image)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new ArgumentException("Source image can be graysclae (8 bpp indexed) image only.");
            }
            int width = image.Width;
            int height = image.Height;
            int num3 = image.Stride - width;
            IntegralImage image2 = new IntegralImage(width, height);
            uint[,] integralImage = image2.integralImage;
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            for (int i = 1; i <= height; i++)
            {
                uint num5 = 0;
                int num6 = 1;
                while (num6 <= width)
                {
                    num5 += numPtr[0];
                    integralImage[i, num6] = num5 + integralImage[i - 1, num6];
                    num6++;
                    numPtr++;
                }
                numPtr += num3;
            }
            return image2;
        }

        public static IntegralImage FromBitmap(Bitmap image)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new UnsupportedImageFormatException("Source image can be graysclae (8 bpp indexed) image only.");
            }
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            IntegralImage image2 = FromBitmap(imageData);
            image.UnlockBits(imageData);
            return image2;
        }

        public static IntegralImage FromBitmap(BitmapData imageData)
        {
            return FromBitmap(new UnmanagedImage(imageData));
        }

        public float GetRectangleMean(int x, int y, int radius)
        {
            return this.GetRectangleMean(x - radius, y - radius, x + radius, y + radius);
        }

        public float GetRectangleMean(int x1, int y1, int x2, int y2)
        {
            if (((x2 < 0) || (y2 < 0)) || ((x1 >= this.width) || (y1 >= this.height)))
            {
                return 0f;
            }
            if (x1 < 0)
            {
                x1 = 0;
            }
            if (y1 < 0)
            {
                y1 = 0;
            }
            x2++;
            y2++;
            if (x2 > this.width)
            {
                x2 = this.width;
            }
            if (y2 > this.height)
            {
                y2 = this.height;
            }
            return (float) (((double) (((this.integralImage[y2, x2] + this.integralImage[y1, x1]) - this.integralImage[y2, x1]) - this.integralImage[y1, x2])) / ((double) ((x2 - x1) * (y2 - y1))));
        }

        public float GetRectangleMeanUnsafe(int x, int y, int radius)
        {
            return this.GetRectangleMeanUnsafe(x - radius, y - radius, x + radius, y + radius);
        }

        public float GetRectangleMeanUnsafe(int x1, int y1, int x2, int y2)
        {
            x2++;
            y2++;
            return (float) (((double) (((this.integralImage[y2, x2] + this.integralImage[y1, x1]) - this.integralImage[y2, x1]) - this.integralImage[y1, x2])) / ((double) ((x2 - x1) * (y2 - y1))));
        }

        public uint GetRectangleSum(int x, int y, int radius)
        {
            return this.GetRectangleSum(x - radius, y - radius, x + radius, y + radius);
        }

        public uint GetRectangleSum(int x1, int y1, int x2, int y2)
        {
            if (((x2 < 0) || (y2 < 0)) || ((x1 >= this.width) || (y1 >= this.height)))
            {
                return 0;
            }
            if (x1 < 0)
            {
                x1 = 0;
            }
            if (y1 < 0)
            {
                y1 = 0;
            }
            x2++;
            y2++;
            if (x2 > this.width)
            {
                x2 = this.width;
            }
            if (y2 > this.height)
            {
                y2 = this.height;
            }
            return (((this.integralImage[y2, x2] + this.integralImage[y1, x1]) - this.integralImage[y2, x1]) - this.integralImage[y1, x2]);
        }

        public uint GetRectangleSumUnsafe(int x, int y, int radius)
        {
            return this.GetRectangleSumUnsafe(x - radius, y - radius, x + radius, y + radius);
        }

        public uint GetRectangleSumUnsafe(int x1, int y1, int x2, int y2)
        {
            x2++;
            y2++;
            return (((this.integralImage[y2, x2] + this.integralImage[y1, x1]) - this.integralImage[y2, x1]) - this.integralImage[y1, x2]);
        }

        public uint[,] InternalData
        {
            get
            {
                return this.integralImage;
            }
        }
    }
}

