namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class PointedColorFloodFill : BaseInPlacePartialFilter
    {
        private bool[,] checkedPixels;
        private byte fillB;
        private byte fillG;
        private byte fillR;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private byte maxB;
        private byte maxG;
        private byte maxR;
        private byte minB;
        private byte minG;
        private byte minR;
        private int scan0;
        private IntPoint startingPoint;
        private int startX;
        private int startY;
        private int stopX;
        private int stopY;
        private int stride;
        private Color tolerance;

        public PointedColorFloodFill()
        {
            this.startingPoint = new IntPoint(0, 0);
            this.tolerance = Color.FromArgb(0, 0, 0);
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
        }

        public PointedColorFloodFill(Color fillColor) : this()
        {
            this.FillColor = fillColor;
        }

        private bool CheckGrayPixel(byte pixel)
        {
            return ((pixel >= this.minG) && (pixel <= this.maxG));
        }

        private unsafe bool CheckRGBPixel(byte* pixel)
        {
            return (((((pixel[2] >= this.minR) && (pixel[2] <= this.maxR)) && ((pixel[1] >= this.minG) && (pixel[1] <= this.maxG))) && (pixel[0] >= this.minB)) && (pixel[0] <= this.maxB));
        }

        private int CoordsToPointerGray(int x, int y)
        {
            return ((this.scan0 + (this.stride * y)) + x);
        }

        private int CoordsToPointerRGB(int x, int y)
        {
            return ((this.scan0 + (this.stride * y)) + (x * 3));
        }

        private unsafe void LinearFloodFill4Gray(IntPoint startingPoint)
        {
            Queue<IntPoint> queue = new Queue<IntPoint>();
            queue.Enqueue(startingPoint);
            while (queue.Count > 0)
            {
                IntPoint point = queue.Dequeue();
                int x = point.X;
                int y = point.Y;
                byte* numPtr = (byte*) this.CoordsToPointerGray(x, y);
                int num3 = x;
                byte* numPtr2 = numPtr;
                do
                {
                    numPtr2[0] = this.fillG;
                    this.checkedPixels[y, num3] = true;
                    num3--;
                    numPtr2--;
                }
                while (((num3 >= this.startX) && !this.checkedPixels[y, num3]) && this.CheckGrayPixel(numPtr2[0]));
                num3++;
                int num4 = x;
                numPtr2 = numPtr;
                do
                {
                    numPtr2[0] = this.fillG;
                    this.checkedPixels[y, num4] = true;
                    num4++;
                    numPtr2++;
                }
                while (((num4 <= this.stopX) && !this.checkedPixels[y, num4]) && this.CheckGrayPixel(numPtr2[0]));
                num4--;
                numPtr2 = (byte*) this.CoordsToPointerGray(num3, y);
                bool flag = false;
                bool flag2 = false;
                int num5 = y - 1;
                int num6 = y + 1;
                int num7 = num3;
                while (num7 <= num4)
                {
                    if (((y > this.startY) && !this.checkedPixels[y - 1, num7]) && this.CheckGrayPixel(*(numPtr2 - this.stride)))
                    {
                        if (!flag)
                        {
                            queue.Enqueue(new IntPoint(num7, num5));
                            flag = true;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                    if (((y < this.stopY) && !this.checkedPixels[y + 1, num7]) && this.CheckGrayPixel(numPtr2[this.stride]))
                    {
                        if (!flag2)
                        {
                            queue.Enqueue(new IntPoint(num7, num6));
                            flag2 = true;
                        }
                    }
                    else
                    {
                        flag2 = false;
                    }
                    num7++;
                    numPtr2++;
                }
            }
        }

        private unsafe void LinearFloodFill4RGB(IntPoint startPoint)
        {
            Queue<IntPoint> queue = new Queue<IntPoint>();
            queue.Enqueue(this.startingPoint);
            while (queue.Count > 0)
            {
                IntPoint point = queue.Dequeue();
                int x = point.X;
                int y = point.Y;
                byte* numPtr = (byte*) this.CoordsToPointerRGB(x, y);
                int num3 = x;
                byte* pixel = numPtr;
                do
                {
                    pixel[2] = this.fillR;
                    pixel[1] = this.fillG;
                    pixel[0] = this.fillB;
                    this.checkedPixels[y, num3] = true;
                    num3--;
                    pixel -= 3;
                }
                while (((num3 >= this.startX) && !this.checkedPixels[y, num3]) && this.CheckRGBPixel(pixel));
                num3++;
                int num4 = x;
                pixel = numPtr;
                do
                {
                    pixel[2] = this.fillR;
                    pixel[1] = this.fillG;
                    pixel[0] = this.fillB;
                    this.checkedPixels[y, num4] = true;
                    num4++;
                    pixel += 3;
                }
                while (((num4 <= this.stopX) && !this.checkedPixels[y, num4]) && this.CheckRGBPixel(pixel));
                num4--;
                pixel = (byte*) this.CoordsToPointerRGB(num3, y);
                bool flag = false;
                bool flag2 = false;
                int num5 = y - 1;
                int num6 = y + 1;
                int num7 = num3;
                while (num7 <= num4)
                {
                    if (((y > this.startY) && !this.checkedPixels[num5, num7]) && this.CheckRGBPixel(pixel - this.stride))
                    {
                        if (!flag)
                        {
                            queue.Enqueue(new IntPoint(num7, num5));
                            flag = true;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                    if (((y < this.stopY) && !this.checkedPixels[num6, num7]) && this.CheckRGBPixel(pixel + this.stride))
                    {
                        if (!flag2)
                        {
                            queue.Enqueue(new IntPoint(num7, num6));
                            flag2 = true;
                        }
                    }
                    else
                    {
                        flag2 = false;
                    }
                    num7++;
                    pixel += 3;
                }
            }
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            if (rect.Contains(this.startingPoint.X, this.startingPoint.Y))
            {
                this.startX = rect.Left;
                this.startY = rect.Top;
                this.stopX = rect.Right - 1;
                this.stopY = rect.Bottom - 1;
                this.scan0 = image.ImageData.ToInt32();
                this.stride = image.Stride;
                this.checkedPixels = new bool[image.Height, image.Width];
                if (image.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    byte num = *((byte*) this.CoordsToPointerGray(this.startingPoint.X, this.startingPoint.Y));
                    this.minG = (byte) Math.Max(0, num - this.tolerance.G);
                    this.maxG = (byte) Math.Min(0xff, num + this.tolerance.G);
                    this.LinearFloodFill4Gray(this.startingPoint);
                }
                else
                {
                    byte* numPtr = (byte*) this.CoordsToPointerRGB(this.startingPoint.X, this.startingPoint.Y);
                    this.minR = (byte) Math.Max(0, numPtr[2] - this.tolerance.R);
                    this.maxR = (byte) Math.Min(0xff, numPtr[2] + this.tolerance.R);
                    this.minG = (byte) Math.Max(0, numPtr[1] - this.tolerance.G);
                    this.maxG = (byte) Math.Min(0xff, numPtr[1] + this.tolerance.G);
                    this.minB = (byte) Math.Max(0, numPtr[0] - this.tolerance.B);
                    this.maxB = (byte) Math.Min(0xff, numPtr[0] + this.tolerance.B);
                    this.LinearFloodFill4RGB(this.startingPoint);
                }
            }
        }

        public Color FillColor
        {
            get
            {
                return Color.FromArgb(this.fillR, this.fillG, this.fillB);
            }
            set
            {
                this.fillR = value.R;
                this.fillG = value.G;
                this.fillB = value.B;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public IntPoint StartingPoint
        {
            get
            {
                return this.startingPoint;
            }
            set
            {
                this.startingPoint = value;
            }
        }

        public Color Tolerance
        {
            get
            {
                return this.tolerance;
            }
            set
            {
                this.tolerance = value;
            }
        }
    }
}

