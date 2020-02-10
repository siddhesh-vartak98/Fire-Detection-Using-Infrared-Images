namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class CanvasMove : BaseInPlaceFilter
    {
        private byte fillBlue;
        private byte fillGray;
        private byte fillGreen;
        private byte fillRed;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private IntPoint movePoint;

        private CanvasMove()
        {
            this.fillRed = 0xff;
            this.fillGreen = 0xff;
            this.fillBlue = 0xff;
            this.fillGray = 0xff;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
        }

        public CanvasMove(IntPoint movePoint) : this()
        {
            this.movePoint = movePoint;
        }

        public CanvasMove(IntPoint movePoint, byte fillColorGray) : this()
        {
            this.movePoint = movePoint;
            this.fillGray = fillColorGray;
        }

        public CanvasMove(IntPoint movePoint, Color fillColorRGB) : this()
        {
            this.movePoint = movePoint;
            this.fillRed = fillColorRGB.R;
            this.fillGreen = fillColorRGB.G;
            this.fillBlue = fillColorRGB.B;
        }

        public CanvasMove(IntPoint movePoint, Color fillColorRGB, byte fillColorGray) : this()
        {
            this.movePoint = movePoint;
            this.fillRed = fillColorRGB.R;
            this.fillGreen = fillColorRGB.G;
            this.fillBlue = fillColorRGB.B;
            this.fillGray = fillColorGray;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image)
        {
            byte* numPtr2;
            byte* numPtr3;
            int num = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int x = this.movePoint.X;
            int y = this.movePoint.Y;
            Rectangle rectangle = Rectangle.Intersect(new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height));
            int num7 = 0;
            int num8 = height;
            int num9 = 1;
            int num10 = 0;
            int num11 = width;
            int num12 = 1;
            if (y > 0)
            {
                num7 = height - 1;
                num8 = -1;
                num9 = -1;
            }
            if (x > 0)
            {
                num10 = width - 1;
                num11 = -1;
                num12 = -1;
            }
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                for (int i = num7; i != num8; i += num9)
                {
                    for (int j = num10; j != num11; j += num12)
                    {
                        numPtr2 = (numPtr + (i * stride)) + j;
                        if (rectangle.Contains(j, i))
                        {
                            numPtr3 = (numPtr + ((i - y) * stride)) + (j - x);
                            numPtr2[0] = numPtr3[0];
                        }
                        else
                        {
                            numPtr2[0] = this.fillGray;
                        }
                    }
                }
            }
            else
            {
                for (int k = num7; k != num8; k += num9)
                {
                    for (int m = num10; m != num11; m += num12)
                    {
                        numPtr2 = (numPtr + (k * stride)) + (m * num);
                        if (rectangle.Contains(m, k))
                        {
                            numPtr3 = (numPtr + ((k - y) * stride)) + ((m - x) * num);
                            numPtr2[2] = numPtr3[2];
                            numPtr2[1] = numPtr3[1];
                            numPtr2[0] = numPtr3[0];
                        }
                        else
                        {
                            numPtr2[2] = this.fillRed;
                            numPtr2[1] = this.fillGreen;
                            numPtr2[0] = this.fillBlue;
                        }
                    }
                }
            }
        }

        public byte FillColorGray
        {
            get
            {
                return this.fillGray;
            }
            set
            {
                this.fillGray = value;
            }
        }

        public Color FillColorRGB
        {
            get
            {
                return Color.FromArgb(this.fillRed, this.fillGreen, this.fillBlue);
            }
            set
            {
                this.fillRed = value.R;
                this.fillGreen = value.G;
                this.fillBlue = value.B;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public IntPoint MovePoint
        {
            get
            {
                return this.movePoint;
            }
            set
            {
                this.movePoint = value;
            }
        }
    }
}

