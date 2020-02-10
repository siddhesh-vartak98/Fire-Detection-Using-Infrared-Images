namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class BinaryDilatation3x3 : BaseUsingCopyPartialFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        public BinaryDilatation3x3()
        {
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData, Rectangle rect)
        {
            if ((rect.Width < 3) || (rect.Height < 3))
            {
                throw new InvalidImagePropertiesException("Processing rectangle mast be at least 3x3 in size.");
            }
            int num = rect.Left + 1;
            int num2 = rect.Top + 1;
            int num3 = rect.Right - 1;
            int num4 = rect.Bottom - 1;
            int stride = destinationData.Stride;
            int index = sourceData.Stride;
            int num7 = (stride - rect.Width) + 1;
            int num8 = (index - rect.Width) + 1;
            byte* numPtr = (byte*) sourceData.ImageData.ToPointer();
            byte* numPtr2 = (byte*) destinationData.ImageData.ToPointer();
            numPtr += (num - 1) + ((num2 - 1) * index);
            numPtr2 += (num - 1) + ((num2 - 1) * stride);
            numPtr2[0] = (byte) (((numPtr[0] | numPtr[1]) | numPtr[index]) | numPtr[index + 1]);
            numPtr++;
            numPtr2++;
            int num9 = num;
            while (num9 < num3)
            {
                numPtr2[0] = (byte) (((((numPtr[0] | numPtr[-1]) | numPtr[1]) | numPtr[index]) | numPtr[index - 1]) | numPtr[index + 1]);
                num9++;
                numPtr++;
                numPtr2++;
            }
            numPtr2[0] = (byte) (((numPtr[0] | numPtr[-1]) | numPtr[index]) | numPtr[index - 1]);
            numPtr += num8;
            numPtr2 += num7;
            for (int i = num2; i < num4; i++)
            {
                numPtr2[0] = (byte) (((((numPtr[0] | numPtr[1]) | numPtr[-index]) | numPtr[-index + 1]) | numPtr[index]) | numPtr[index + 1]);
                numPtr++;
                numPtr2++;
                int num11 = num;
                while (num11 < num3)
                {
                    numPtr2[0] = (byte) ((((((((numPtr[0] | numPtr[-1]) | numPtr[1]) | numPtr[-index]) | numPtr[-index - 1]) | numPtr[-index + 1]) | numPtr[index]) | numPtr[index - 1]) | numPtr[index + 1]);
                    num11++;
                    numPtr++;
                    numPtr2++;
                }
                numPtr2[0] = (byte) (((((numPtr[0] | numPtr[-1]) | numPtr[-index]) | numPtr[-index - 1]) | numPtr[index]) | numPtr[index - 1]);
                numPtr += num8;
                numPtr2 += num7;
            }
            numPtr2[0] = (byte) (((numPtr[0] | numPtr[1]) | numPtr[-index]) | numPtr[-index + 1]);
            numPtr++;
            numPtr2++;
            int num12 = num;
            while (num12 < num3)
            {
                numPtr2[0] = (byte) (((((numPtr[0] | numPtr[-1]) | numPtr[1]) | numPtr[-index]) | numPtr[-index - 1]) | numPtr[-index + 1]);
                num12++;
                numPtr++;
                numPtr2++;
            }
            numPtr2[0] = (byte) (((numPtr[0] | numPtr[-1]) | numPtr[-index]) | numPtr[-index - 1]);
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

