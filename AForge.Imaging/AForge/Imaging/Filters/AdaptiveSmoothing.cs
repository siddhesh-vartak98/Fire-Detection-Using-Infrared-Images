namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class AdaptiveSmoothing : BaseUsingCopyPartialFilter
    {
        private double factor;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;

        public AdaptiveSmoothing()
        {
            this.factor = 3.0;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
        }

        public AdaptiveSmoothing(double factor) : this()
        {
            this.factor = factor;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
        {
            int index = Image.GetPixelFormatSize(source.PixelFormat) / 8;
            int num2 = index * 2;
            int left = rect.Left;
            int top = rect.Top;
            int num5 = left + rect.Width;
            int num6 = top + rect.Height;
            int num7 = left + 2;
            int num8 = top + 2;
            int num9 = num5 - 2;
            int num10 = num6 - 2;
            int stride = source.Stride;
            int num12 = destination.Stride;
            int num13 = stride - (rect.Width * index);
            int num14 = num12 - (rect.Width * index);
            double num20 = (-8.0 * this.factor) * this.factor;
            byte* numPtr = (byte*) (source.ImageData.ToPointer() + (stride * 2));
            byte* numPtr2 = (byte*) (destination.ImageData.ToPointer() + (num12 * 2));
            numPtr += (top * stride) + (left * index);
            numPtr2 += (top * num12) + (left * index);
            for (int i = num8; i < num10; i++)
            {
                numPtr += num2;
                numPtr2 += num2;
                for (int j = num7; j < num9; j++)
                {
                    int num23 = 0;
                    while (num23 < index)
                    {
                        double num18 = 0.0;
                        double num19 = 0.0;
                        double num15 = numPtr[-stride] - numPtr[-num2 - stride];
                        double num16 = numPtr[-index] - numPtr[-index - (2 * stride)];
                        double num17 = Math.Exp(((num15 * num15) + (num16 * num16)) / num20);
                        num19 += num17 * numPtr[-index - stride];
                        num18 += num17;
                        num15 = numPtr[index - stride] - numPtr[-index - stride];
                        num16 = numPtr[0] - numPtr[-2 * stride];
                        num17 = Math.Exp(((num15 * num15) + (num16 * num16)) / num20);
                        num19 += num17 * numPtr[-stride];
                        num18 += num17;
                        num15 = numPtr[num2 - stride] - numPtr[-stride];
                        num16 = numPtr[index] - numPtr[index - (2 * stride)];
                        num17 = Math.Exp(((num15 * num15) + (num16 * num16)) / num20);
                        num19 += num17 * numPtr[index - stride];
                        num18 += num17;
                        num15 = numPtr[0] - numPtr[-num2];
                        num16 = numPtr[-index + stride] - numPtr[-index - stride];
                        num17 = Math.Exp(((num15 * num15) + (num16 * num16)) / num20);
                        num19 += num17 * numPtr[-index];
                        num18 += num17;
                        num15 = numPtr[index] - numPtr[-index];
                        num16 = numPtr[stride] - numPtr[-stride];
                        num17 = Math.Exp(((num15 * num15) + (num16 * num16)) / num20);
                        num19 += num17 * numPtr[0];
                        num18 += num17;
                        num15 = numPtr[num2] - numPtr[0];
                        num16 = numPtr[index + stride] - numPtr[index - stride];
                        num17 = Math.Exp(((num15 * num15) + (num16 * num16)) / num20);
                        num19 += num17 * numPtr[index];
                        num18 += num17;
                        num15 = numPtr[stride] - numPtr[-num2 + stride];
                        num16 = numPtr[-index + (2 * stride)] - numPtr[-index];
                        num17 = Math.Exp(((num15 * num15) + (num16 * num16)) / num20);
                        num19 += num17 * numPtr[-index + stride];
                        num18 += num17;
                        num15 = numPtr[index + stride] - numPtr[-index + stride];
                        num16 = numPtr[2 * stride] - numPtr[0];
                        num17 = Math.Exp(((num15 * num15) + (num16 * num16)) / num20);
                        num19 += num17 * numPtr[stride];
                        num18 += num17;
                        num15 = numPtr[num2 + stride] - numPtr[stride];
                        num16 = numPtr[index + (2 * stride)] - numPtr[index];
                        num17 = Math.Exp(((num15 * num15) + (num16 * num16)) / num20);
                        num19 += num17 * numPtr[index + stride];
                        num18 += num17;
                        numPtr2[0] = (num18 == 0.0) ? numPtr[0] : ((byte) Math.Min((double) (num19 / num18), (double) 255.0));
                        num23++;
                        numPtr++;
                        numPtr2++;
                    }
                }
                numPtr += num13 + num2;
                numPtr2 += num14 + num2;
            }
        }

        public double Factor
        {
            get
            {
                return this.factor;
            }
            set
            {
                this.factor = value;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }
    }
}

