namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class CannyEdgeDetector : BaseUsingCopyPartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private GaussianBlur gaussianFilter;
        private byte highThreshold;
        private byte lowThreshold;

        public CannyEdgeDetector()
        {
            this.gaussianFilter = new GaussianBlur();
            this.lowThreshold = 20;
            this.highThreshold = 100;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        public CannyEdgeDetector(byte lowThreshold, byte highThreshold) : this()
        {
            this.lowThreshold = lowThreshold;
            this.highThreshold = highThreshold;
        }

        public CannyEdgeDetector(byte lowThreshold, byte highThreshold, double sigma) : this()
        {
            this.lowThreshold = lowThreshold;
            this.highThreshold = highThreshold;
            this.gaussianFilter.Sigma = sigma;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
        {
            int num = rect.Left + 1;
            int num2 = rect.Top + 1;
            int num3 = (num + rect.Width) - 2;
            int num4 = (num2 + rect.Height) - 2;
            int num5 = rect.Width - 2;
            int num6 = rect.Height - 2;
            int stride = destination.Stride;
            int index = source.Stride;
            int num9 = (stride - rect.Width) + 2;
            int num10 = (index - rect.Width) + 2;
            double num14 = 57.295779513082323;
            float num15 = 0f;
            float num16 = 0f;
            UnmanagedImage image = this.gaussianFilter.Apply(source);
            byte[] buffer = new byte[num5 * num6];
            float[,] numArray = new float[source.Width, source.Height];
            float negativeInfinity = float.NegativeInfinity;
            byte* numPtr = (byte*) (image.ImageData.ToPointer() + ((index * num2) + num));
            int num18 = 0;
            for (int i = num2; i < num4; i++)
            {
                int num20 = num;
                while (num20 < num3)
                {
                    double num13;
                    int num11 = (((numPtr[-index + 1] + numPtr[index + 1]) - numPtr[-index - 1]) - numPtr[index - 1]) + (2 * (numPtr[1] - numPtr[-1]));
                    int num12 = (((numPtr[-index - 1] + numPtr[-index + 1]) - numPtr[index - 1]) - numPtr[index + 1]) + (2 * (numPtr[-index] - numPtr[index]));
                    numArray[num20, i] = (float) Math.Sqrt((double) ((num11 * num11) + (num12 * num12)));
                    if (numArray[num20, i] > negativeInfinity)
                    {
                        negativeInfinity = numArray[num20, i];
                    }
                    if (num11 == 0)
                    {
                        num13 = (num12 == 0) ? ((double) 0) : ((double) 90);
                    }
                    else
                    {
                        double d = ((double) num12) / ((double) num11);
                        if (d < 0.0)
                        {
                            num13 = 180.0 - (Math.Atan(-d) * num14);
                        }
                        else
                        {
                            num13 = Math.Atan(d) * num14;
                        }
                        if (num13 < 22.5)
                        {
                            num13 = 0.0;
                        }
                        else if (num13 < 67.5)
                        {
                            num13 = 45.0;
                        }
                        else if (num13 < 112.5)
                        {
                            num13 = 90.0;
                        }
                        else if (num13 < 157.5)
                        {
                            num13 = 135.0;
                        }
                        else
                        {
                            num13 = 0.0;
                        }
                    }
                    buffer[num18] = (byte) num13;
                    num20++;
                    numPtr++;
                    num18++;
                }
                numPtr += num10;
            }
            byte* numPtr2 = (byte*) (destination.ImageData.ToPointer() + ((stride * num2) + num));
            num18 = 0;
            for (int j = num2; j < num4; j++)
            {
                int num23 = num;
                while (num23 < num3)
                {
                    switch (buffer[num18])
                    {
                        case 90:
                            num15 = numArray[num23, j + 1];
                            num16 = numArray[num23, j - 1];
                            break;

                        case 0x87:
                            num15 = numArray[num23 + 1, j + 1];
                            num16 = numArray[num23 - 1, j - 1];
                            break;

                        case 0:
                            num15 = numArray[num23 - 1, j];
                            num16 = numArray[num23 + 1, j];
                            break;

                        case 0x2d:
                            num15 = numArray[num23 - 1, j + 1];
                            num16 = numArray[num23 + 1, j - 1];
                            break;
                    }
                    if ((numArray[num23, j] < num15) || (numArray[num23, j] < num16))
                    {
                        numPtr2[0] = 0;
                    }
                    else
                    {
                        numPtr2[0] = (byte) ((numArray[num23, j] / negativeInfinity) * 255f);
                    }
                    num23++;
                    numPtr2++;
                    num18++;
                }
                numPtr2 += num9;
            }
            numPtr2 = (byte*) (destination.ImageData.ToPointer() + ((stride * num2) + num));
            for (int k = num2; k < num4; k++)
            {
                int num25 = num;
                while (num25 < num3)
                {
                    if (numPtr2[0] < this.highThreshold)
                    {
                        if (numPtr2[0] < this.lowThreshold)
                        {
                            numPtr2[0] = 0;
                        }
                        else if ((((numPtr2[-1] < this.highThreshold) && (numPtr2[1] < this.highThreshold)) && ((numPtr2[-stride - 1] < this.highThreshold) && (numPtr2[-stride] < this.highThreshold))) && (((numPtr2[-stride + 1] < this.highThreshold) && (numPtr2[stride - 1] < this.highThreshold)) && ((numPtr2[stride] < this.highThreshold) && (numPtr2[stride + 1] < this.highThreshold))))
                        {
                            numPtr2[0] = 0;
                        }
                    }
                    num25++;
                    numPtr2++;
                }
                numPtr2 += num9;
            }
            Drawing.Rectangle(destination, rect, Color.Black);
            image.Dispose();
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public double GaussianSigma
        {
            get
            {
                return this.gaussianFilter.Sigma;
            }
            set
            {
                this.gaussianFilter.Sigma = value;
            }
        }

        public int GaussianSize
        {
            get
            {
                return this.gaussianFilter.Size;
            }
            set
            {
                this.gaussianFilter.Size = value;
            }
        }

        public byte HighThreshold
        {
            get
            {
                return this.highThreshold;
            }
            set
            {
                this.highThreshold = value;
            }
        }

        public byte LowThreshold
        {
            get
            {
                return this.lowThreshold;
            }
            set
            {
                this.lowThreshold = value;
            }
        }
    }
}

