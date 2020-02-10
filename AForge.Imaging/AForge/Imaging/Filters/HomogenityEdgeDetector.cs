namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class HomogenityEdgeDetector : BaseUsingCopyPartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        public HomogenityEdgeDetector()
        {
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
        {
            int num = rect.Left + 1;
            int num2 = rect.Top + 1;
            int num3 = (num + rect.Width) - 2;
            int num4 = (num2 + rect.Height) - 2;
            int stride = destination.Stride;
            int index = source.Stride;
            int num7 = (stride - rect.Width) + 2;
            int num8 = (index - rect.Width) + 2;
            byte* numPtr = (byte*) source.ImageData.ToPointer();
            byte* numPtr2 = (byte*) destination.ImageData.ToPointer();
            numPtr += (index * num2) + num;
            numPtr2 += (stride * num2) + num;
            for (int i = num2; i < num4; i++)
            {
                int num13 = num;
                while (num13 < num3)
                {
                    int num10 = 0;
                    int num11 = numPtr[0];
                    int num9 = num11 - numPtr[-index - 1];
                    if (num9 < 0)
                    {
                        num9 = -num9;
                    }
                    if (num9 > num10)
                    {
                        num10 = num9;
                    }
                    num9 = num11 - numPtr[-index];
                    if (num9 < 0)
                    {
                        num9 = -num9;
                    }
                    if (num9 > num10)
                    {
                        num10 = num9;
                    }
                    num9 = num11 - numPtr[-index + 1];
                    if (num9 < 0)
                    {
                        num9 = -num9;
                    }
                    if (num9 > num10)
                    {
                        num10 = num9;
                    }
                    num9 = num11 - numPtr[-1];
                    if (num9 < 0)
                    {
                        num9 = -num9;
                    }
                    if (num9 > num10)
                    {
                        num10 = num9;
                    }
                    num9 = num11 - numPtr[1];
                    if (num9 < 0)
                    {
                        num9 = -num9;
                    }
                    if (num9 > num10)
                    {
                        num10 = num9;
                    }
                    num9 = num11 - numPtr[index - 1];
                    if (num9 < 0)
                    {
                        num9 = -num9;
                    }
                    if (num9 > num10)
                    {
                        num10 = num9;
                    }
                    num9 = num11 - numPtr[index];
                    if (num9 < 0)
                    {
                        num9 = -num9;
                    }
                    if (num9 > num10)
                    {
                        num10 = num9;
                    }
                    num9 = num11 - numPtr[index + 1];
                    if (num9 < 0)
                    {
                        num9 = -num9;
                    }
                    if (num9 > num10)
                    {
                        num10 = num9;
                    }
                    numPtr2[0] = (byte) num10;
                    num13++;
                    numPtr++;
                    numPtr2++;
                }
                numPtr += num8;
                numPtr2 += num7;
            }
            Drawing.Rectangle(destination, rect, Color.Black);
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

