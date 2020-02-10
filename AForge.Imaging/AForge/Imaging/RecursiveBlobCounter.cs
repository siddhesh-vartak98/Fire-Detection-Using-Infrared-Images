namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class RecursiveBlobCounter : BlobCounterBase
    {
        private byte backgroundThresholdB;
        private byte backgroundThresholdG;
        private byte backgroundThresholdR;
        private int pixelSize;
        private int stride;
        private int[] tempLabels;

        public RecursiveBlobCounter()
        {
        }

        public RecursiveBlobCounter(UnmanagedImage image) : base(image)
        {
        }

        public RecursiveBlobCounter(Bitmap image) : base(image)
        {
        }

        public RecursiveBlobCounter(BitmapData imageData) : base(imageData)
        {
        }

        protected override unsafe void BuildObjectsMap(UnmanagedImage image)
        {
            this.stride = image.Stride;
            if (((image.PixelFormat != PixelFormat.Format8bppIndexed) && (image.PixelFormat != PixelFormat.Format24bppRgb)) && ((image.PixelFormat != PixelFormat.Format32bppArgb) && (image.PixelFormat != PixelFormat.Format32bppPArgb)))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
            this.tempLabels = new int[(base.imageWidth + 2) * (base.imageHeight + 2)];
            int index = 0;
            int num2 = base.imageWidth + 2;
            while (index < num2)
            {
                this.tempLabels[index] = -1;
                this.tempLabels[index + ((base.imageHeight + 1) * (base.imageWidth + 2))] = -1;
                index++;
            }
            int num3 = 0;
            int num4 = base.imageHeight + 2;
            while (num3 < num4)
            {
                this.tempLabels[num3 * (base.imageWidth + 2)] = -1;
                this.tempLabels[((num3 * (base.imageWidth + 2)) + base.imageWidth) + 1] = -1;
                num3++;
            }
            base.objectsCount = 0;
            byte* pixel = (byte*) image.ImageData.ToPointer();
            int num5 = (base.imageWidth + 2) + 1;
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                int num6 = this.stride - base.imageWidth;
                for (int j = 0; j < base.imageHeight; j++)
                {
                    int num8 = 0;
                    while (num8 < base.imageWidth)
                    {
                        if ((pixel[0] > this.backgroundThresholdG) && (this.tempLabels[num5] == 0))
                        {
                            base.objectsCount++;
                            this.LabelPixel(pixel, num5);
                        }
                        num8++;
                        pixel++;
                        num5++;
                    }
                    pixel += num6;
                    num5 += 2;
                }
            }
            else
            {
                this.pixelSize = Image.GetPixelFormatSize(image.PixelFormat) / 8;
                int num9 = this.stride - (base.imageWidth * this.pixelSize);
                for (int k = 0; k < base.imageHeight; k++)
                {
                    int num11 = 0;
                    while (num11 < base.imageWidth)
                    {
                        if ((((pixel[2] > this.backgroundThresholdR) || (pixel[1] > this.backgroundThresholdG)) || (pixel[0] > this.backgroundThresholdB)) && (this.tempLabels[num5] == 0))
                        {
                            base.objectsCount++;
                            this.LabelColorPixel(pixel, num5);
                        }
                        num11++;
                        pixel += this.pixelSize;
                        num5++;
                    }
                    pixel += num9;
                    num5 += 2;
                }
            }
            base.objectLabels = new int[base.imageWidth * base.imageHeight];
            for (int i = 0; i < base.imageHeight; i++)
            {
                Array.Copy(this.tempLabels, ((i + 1) * (base.imageWidth + 2)) + 1, base.objectLabels, i * base.imageWidth, base.imageWidth);
            }
        }

        private unsafe void LabelColorPixel(byte* pixel, int labelPointer)
        {
            if ((this.tempLabels[labelPointer] == 0) && (((pixel[2] > this.backgroundThresholdR) || (pixel[1] > this.backgroundThresholdG)) || (pixel[0] > this.backgroundThresholdB)))
            {
                this.tempLabels[labelPointer] = base.objectsCount;
                this.LabelColorPixel(pixel + this.pixelSize, labelPointer + 1);
                this.LabelColorPixel((pixel + this.pixelSize) + this.stride, ((labelPointer + 1) + 2) + base.imageWidth);
                this.LabelColorPixel(pixel + this.stride, (labelPointer + 2) + base.imageWidth);
                this.LabelColorPixel((pixel - this.pixelSize) + this.stride, ((labelPointer - 1) + 2) + base.imageWidth);
                this.LabelColorPixel(pixel - this.pixelSize, labelPointer - 1);
                this.LabelColorPixel((pixel - this.pixelSize) - this.stride, ((labelPointer - 1) - 2) - base.imageWidth);
                this.LabelColorPixel(pixel - this.stride, (labelPointer - 2) - base.imageWidth);
                this.LabelColorPixel((pixel + this.pixelSize) - this.stride, ((labelPointer + 1) - 2) - base.imageWidth);
            }
        }

        private unsafe void LabelPixel(byte* pixel, int labelPointer)
        {
            if ((this.tempLabels[labelPointer] == 0) && (pixel[0] > this.backgroundThresholdG))
            {
                this.tempLabels[labelPointer] = base.objectsCount;
                this.LabelPixel(pixel + 1, labelPointer + 1);
                this.LabelPixel((pixel + 1) + this.stride, ((labelPointer + 1) + 2) + base.imageWidth);
                this.LabelPixel(pixel + this.stride, (labelPointer + 2) + base.imageWidth);
                this.LabelPixel((pixel - 1) + this.stride, ((labelPointer - 1) + 2) + base.imageWidth);
                this.LabelPixel(pixel - 1, labelPointer - 1);
                this.LabelPixel((pixel - 1) - this.stride, ((labelPointer - 1) - 2) - base.imageWidth);
                this.LabelPixel(pixel - this.stride, (labelPointer - 2) - base.imageWidth);
                this.LabelPixel((pixel + 1) - this.stride, ((labelPointer + 1) - 2) - base.imageWidth);
            }
        }

        public Color BackgroundThreshold
        {
            get
            {
                return Color.FromArgb(this.backgroundThresholdR, this.backgroundThresholdG, this.backgroundThresholdB);
            }
            set
            {
                this.backgroundThresholdR = value.R;
                this.backgroundThresholdG = value.G;
                this.backgroundThresholdB = value.B;
            }
        }
    }
}

