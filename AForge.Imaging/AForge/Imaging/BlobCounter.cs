namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class BlobCounter : BlobCounterBase
    {
        private byte backgroundThresholdB;
        private byte backgroundThresholdG;
        private byte backgroundThresholdR;

        public BlobCounter()
        {
        }

        public BlobCounter(UnmanagedImage image) : base(image)
        {
        }

        public BlobCounter(Bitmap image) : base(image)
        {
        }

        public BlobCounter(BitmapData imageData) : base(imageData)
        {
        }

        protected override unsafe void BuildObjectsMap(UnmanagedImage image)
        {
            int stride = image.Stride;
            if (((image.PixelFormat != PixelFormat.Format8bppIndexed) && (image.PixelFormat != PixelFormat.Format24bppRgb)) && ((image.PixelFormat != PixelFormat.Format32bppArgb) && (image.PixelFormat != PixelFormat.Format32bppPArgb)))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
            if (base.imageWidth == 1)
            {
                throw new InvalidImagePropertiesException("Too small image.");
            }
            base.objectLabels = new int[base.imageWidth * base.imageHeight];
            int num2 = 0;
            int num3 = (((base.imageWidth / 2) + 1) * ((base.imageHeight / 2) + 1)) + 1;
            int[] numArray = new int[num3];
            for (int i = 0; i < num3; i++)
            {
                numArray[i] = i;
            }
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            int index = 0;
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                int num6 = stride - base.imageWidth;
                if (numPtr[0] > this.backgroundThresholdG)
                {
                    base.objectLabels[index] = ++num2;
                }
                numPtr++;
                index++;
                int num7 = 1;
                while (num7 < base.imageWidth)
                {
                    if (numPtr[0] > this.backgroundThresholdG)
                    {
                        if (numPtr[-1] > this.backgroundThresholdG)
                        {
                            base.objectLabels[index] = base.objectLabels[index - 1];
                        }
                        else
                        {
                            base.objectLabels[index] = ++num2;
                        }
                    }
                    num7++;
                    numPtr++;
                    index++;
                }
                numPtr += num6;
                for (int m = 1; m < base.imageHeight; m++)
                {
                    if (numPtr[0] > this.backgroundThresholdG)
                    {
                        if (numPtr[-stride] > this.backgroundThresholdG)
                        {
                            base.objectLabels[index] = base.objectLabels[index - base.imageWidth];
                        }
                        else if (numPtr[1 - stride] > this.backgroundThresholdG)
                        {
                            base.objectLabels[index] = base.objectLabels[(index + 1) - base.imageWidth];
                        }
                        else
                        {
                            base.objectLabels[index] = ++num2;
                        }
                    }
                    numPtr++;
                    index++;
                    int num9 = 1;
                    while (num9 < (base.imageWidth - 1))
                    {
                        if (numPtr[0] > this.backgroundThresholdG)
                        {
                            if (numPtr[-1] > this.backgroundThresholdG)
                            {
                                base.objectLabels[index] = base.objectLabels[index - 1];
                            }
                            else if (numPtr[-1 - stride] > this.backgroundThresholdG)
                            {
                                base.objectLabels[index] = base.objectLabels[(index - 1) - base.imageWidth];
                            }
                            else if (numPtr[-stride] > this.backgroundThresholdG)
                            {
                                base.objectLabels[index] = base.objectLabels[index - base.imageWidth];
                            }
                            if (numPtr[1 - stride] > this.backgroundThresholdG)
                            {
                                if (base.objectLabels[index] == 0)
                                {
                                    base.objectLabels[index] = base.objectLabels[(index + 1) - base.imageWidth];
                                }
                                else
                                {
                                    int num10 = base.objectLabels[index];
                                    int num11 = base.objectLabels[(index + 1) - base.imageWidth];
                                    if ((num10 != num11) && (numArray[num10] != numArray[num11]))
                                    {
                                        if (numArray[num10] == num10)
                                        {
                                            numArray[num10] = numArray[num11];
                                        }
                                        else if (numArray[num11] == num11)
                                        {
                                            numArray[num11] = numArray[num10];
                                        }
                                        else
                                        {
                                            numArray[numArray[num10]] = numArray[num11];
                                            numArray[num10] = numArray[num11];
                                        }
                                        for (int n = 1; n <= num2; n++)
                                        {
                                            if (numArray[n] != n)
                                            {
                                                int num13 = numArray[n];
                                                while (num13 != numArray[num13])
                                                {
                                                    num13 = numArray[num13];
                                                }
                                                numArray[n] = num13;
                                            }
                                        }
                                    }
                                }
                            }
                            if (base.objectLabels[index] == 0)
                            {
                                base.objectLabels[index] = ++num2;
                            }
                        }
                        num9++;
                        numPtr++;
                        index++;
                    }
                    if (numPtr[0] > this.backgroundThresholdG)
                    {
                        if (numPtr[-1] > this.backgroundThresholdG)
                        {
                            base.objectLabels[index] = base.objectLabels[index - 1];
                        }
                        else if (numPtr[-1 - stride] > this.backgroundThresholdG)
                        {
                            base.objectLabels[index] = base.objectLabels[(index - 1) - base.imageWidth];
                        }
                        else if (numPtr[-stride] > this.backgroundThresholdG)
                        {
                            base.objectLabels[index] = base.objectLabels[index - base.imageWidth];
                        }
                        else
                        {
                            base.objectLabels[index] = ++num2;
                        }
                    }
                    numPtr++;
                    index++;
                    numPtr += num6;
                }
            }
            else
            {
                int num14 = Image.GetPixelFormatSize(image.PixelFormat) / 8;
                int num15 = stride - (base.imageWidth * num14);
                int num16 = stride - num14;
                int num17 = stride + num14;
                if (((numPtr[2] | numPtr[1]) | numPtr[0]) != 0)
                {
                    base.objectLabels[index] = ++num2;
                }
                numPtr += num14;
                index++;
                int num18 = 1;
                while (num18 < base.imageWidth)
                {
                    if (((numPtr[2] > this.backgroundThresholdR) || (numPtr[1] > this.backgroundThresholdG)) || (numPtr[0] > this.backgroundThresholdB))
                    {
                        if (((numPtr[2 - num14] > this.backgroundThresholdR) || (numPtr[1 - num14] > this.backgroundThresholdG)) || (numPtr[-num14] > this.backgroundThresholdB))
                        {
                            base.objectLabels[index] = base.objectLabels[index - 1];
                        }
                        else
                        {
                            base.objectLabels[index] = ++num2;
                        }
                    }
                    num18++;
                    numPtr += num14;
                    index++;
                }
                numPtr += num15;
                for (int num19 = 1; num19 < base.imageHeight; num19++)
                {
                    if (((numPtr[2] > this.backgroundThresholdR) || (numPtr[1] > this.backgroundThresholdG)) || (numPtr[0] > this.backgroundThresholdB))
                    {
                        if (((numPtr[2 - stride] > this.backgroundThresholdR) || (numPtr[1 - stride] > this.backgroundThresholdG)) || (numPtr[-stride] > this.backgroundThresholdB))
                        {
                            base.objectLabels[index] = base.objectLabels[index - base.imageWidth];
                        }
                        else if (((numPtr[2 - num16] > this.backgroundThresholdR) || (numPtr[1 - num16] > this.backgroundThresholdG)) || (numPtr[-num16] > this.backgroundThresholdB))
                        {
                            base.objectLabels[index] = base.objectLabels[(index + 1) - base.imageWidth];
                        }
                        else
                        {
                            base.objectLabels[index] = ++num2;
                        }
                    }
                    numPtr += num14;
                    index++;
                    int num20 = 1;
                    while (num20 < (base.imageWidth - 1))
                    {
                        if (((numPtr[2] > this.backgroundThresholdR) || (numPtr[1] > this.backgroundThresholdG)) || (numPtr[0] > this.backgroundThresholdB))
                        {
                            if (((numPtr[2 - num14] > this.backgroundThresholdR) || (numPtr[1 - num14] > this.backgroundThresholdG)) || (numPtr[-num14] > this.backgroundThresholdB))
                            {
                                base.objectLabels[index] = base.objectLabels[index - 1];
                            }
                            else if (((numPtr[2 - num17] > this.backgroundThresholdR) || (numPtr[1 - num17] > this.backgroundThresholdG)) || (numPtr[-num17] > this.backgroundThresholdB))
                            {
                                base.objectLabels[index] = base.objectLabels[(index - 1) - base.imageWidth];
                            }
                            else if (((numPtr[2 - stride] > this.backgroundThresholdR) || (numPtr[1 - stride] > this.backgroundThresholdG)) || (numPtr[-stride] > this.backgroundThresholdB))
                            {
                                base.objectLabels[index] = base.objectLabels[index - base.imageWidth];
                            }
                            if (((numPtr[2 - num16] > this.backgroundThresholdR) || (numPtr[1 - num16] > this.backgroundThresholdG)) || (numPtr[-num16] > this.backgroundThresholdB))
                            {
                                if (base.objectLabels[index] == 0)
                                {
                                    base.objectLabels[index] = base.objectLabels[(index + 1) - base.imageWidth];
                                }
                                else
                                {
                                    int num21 = base.objectLabels[index];
                                    int num22 = base.objectLabels[(index + 1) - base.imageWidth];
                                    if ((num21 != num22) && (numArray[num21] != numArray[num22]))
                                    {
                                        if (numArray[num21] == num21)
                                        {
                                            numArray[num21] = numArray[num22];
                                        }
                                        else if (numArray[num22] == num22)
                                        {
                                            numArray[num22] = numArray[num21];
                                        }
                                        else
                                        {
                                            numArray[numArray[num21]] = numArray[num22];
                                            numArray[num21] = numArray[num22];
                                        }
                                        for (int num23 = 1; num23 <= num2; num23++)
                                        {
                                            if (numArray[num23] != num23)
                                            {
                                                int num24 = numArray[num23];
                                                while (num24 != numArray[num24])
                                                {
                                                    num24 = numArray[num24];
                                                }
                                                numArray[num23] = num24;
                                            }
                                        }
                                    }
                                }
                            }
                            if (base.objectLabels[index] == 0)
                            {
                                base.objectLabels[index] = ++num2;
                            }
                        }
                        num20++;
                        numPtr += num14;
                        index++;
                    }
                    if (((numPtr[2] > this.backgroundThresholdR) || (numPtr[1] > this.backgroundThresholdG)) || (numPtr[0] > this.backgroundThresholdB))
                    {
                        if (((numPtr[2 - num14] > this.backgroundThresholdR) || (numPtr[1 - num14] > this.backgroundThresholdG)) || (numPtr[-num14] > this.backgroundThresholdB))
                        {
                            base.objectLabels[index] = base.objectLabels[index - 1];
                        }
                        else if (((numPtr[2 - num17] > this.backgroundThresholdR) || (numPtr[1 - num17] > this.backgroundThresholdG)) || (numPtr[-num17] > this.backgroundThresholdB))
                        {
                            base.objectLabels[index] = base.objectLabels[(index - 1) - base.imageWidth];
                        }
                        else if (((numPtr[2 - stride] > this.backgroundThresholdR) || (numPtr[1 - stride] > this.backgroundThresholdG)) || (numPtr[-stride] > this.backgroundThresholdB))
                        {
                            base.objectLabels[index] = base.objectLabels[index - base.imageWidth];
                        }
                        else
                        {
                            base.objectLabels[index] = ++num2;
                        }
                    }
                    numPtr += num14;
                    index++;
                    numPtr += num15;
                }
            }
            int[] numArray2 = new int[numArray.Length];
            base.objectsCount = 0;
            for (int j = 1; j <= num2; j++)
            {
                if (numArray[j] == j)
                {
                    numArray2[j] = ++base.objectsCount;
                }
            }
            for (int k = 1; k <= num2; k++)
            {
                if (numArray[k] != k)
                {
                    numArray2[k] = numArray2[numArray[k]];
                }
            }
            int num27 = 0;
            int length = base.objectLabels.Length;
            while (num27 < length)
            {
                base.objectLabels[num27] = numArray2[base.objectLabels[num27]];
                num27++;
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

