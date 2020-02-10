namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class IterativeThreshold : Threshold
    {
        private int minError;

        public IterativeThreshold()
        {
        }

        public IterativeThreshold(int minError)
        {
            this.minError = minError;
        }

        public IterativeThreshold(int minError, int threshold)
        {
            this.minError = minError;
            base.threshold = threshold;
        }

        public unsafe int CalculateThreshold(UnmanagedImage image, Rectangle rect)
        {
            if ((image.PixelFormat != PixelFormat.Format8bppIndexed) && (image.PixelFormat != PixelFormat.Format16bppGrayScale))
            {
                throw new UnsupportedImageFormatException("Source pixel format is not supported by the routine.");
            }
            int threshold = base.threshold;
            int left = rect.Left;
            int top = rect.Top;
            int num4 = left + rect.Width;
            int num5 = top + rect.Height;
            int[] numArray = null;
            int num6 = 0;
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                numArray = new int[0x100];
                num6 = 0x100;
                byte* numPtr = (byte*) image.ImageData.ToPointer();
                int num7 = image.Stride - rect.Width;
                numPtr += (top * image.Stride) + left;
                for (int i = top; i < num5; i++)
                {
                    int num9 = left;
                    while (num9 < num4)
                    {
                        numArray[numPtr[0]]++;
                        num9++;
                        numPtr++;
                    }
                    numPtr += num7;
                }
            }
            else
            {
                numArray = new int[0x10000];
                num6 = 0x10000;
                byte* numPtr2 = (byte*) (image.ImageData.ToPointer() + (left * 2));
                int stride = image.Stride;
                for (int j = top; j < num5; j++)
                {
                    ushort* numPtr3 = (ushort*) (numPtr2 + (j * stride));
                    int num12 = left;
                    while (num12 < num4)
                    {
                        numArray[numPtr3[0]]++;
                        num12++;
                        numPtr3++;
                    }
                }
            }
            int num13 = 0;
            do
            {
                num13 = threshold;
                double num14 = 0.0;
                int num15 = 0;
                double num16 = 0.0;
                int num17 = 0;
                for (int k = 0; k < threshold; k++)
                {
                    num16 += k * numArray[k];
                    num17 += numArray[k];
                }
                for (int m = threshold; m < num6; m++)
                {
                    num14 += m * numArray[m];
                    num15 += numArray[m];
                }
                num16 /= (double) num17;
                num14 /= (double) num15;
                if (num17 == 0)
                {
                    threshold = (int) num14;
                }
                else if (num15 == 0)
                {
                    threshold = (int) num16;
                }
                else
                {
                    threshold = (int) ((num16 + num14) / 2.0);
                }
            }
            while (Math.Abs((int) (num13 - threshold)) > this.minError);
            return threshold;
        }

        public int CalculateThreshold(Bitmap image, Rectangle rect)
        {
            int num = 0;
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            try
            {
                num = this.CalculateThreshold(data, rect);
            }
            finally
            {
                image.UnlockBits(data);
            }
            return num;
        }

        public int CalculateThreshold(BitmapData image, Rectangle rect)
        {
            return this.CalculateThreshold(new UnmanagedImage(image), rect);
        }

        protected override void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            base.threshold = this.CalculateThreshold(image, rect);
            base.ProcessFilter(image, rect);
        }

        public int MinimumError
        {
            get
            {
                return this.minError;
            }
            set
            {
                this.minError = value;
            }
        }
    }
}

