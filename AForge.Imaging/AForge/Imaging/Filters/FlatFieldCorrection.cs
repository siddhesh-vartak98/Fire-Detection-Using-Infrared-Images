namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class FlatFieldCorrection : BaseInPlaceFilter
    {
        private Bitmap backgroundImage;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private UnmanagedImage unmanagedBackgroundImage;

        public FlatFieldCorrection()
        {
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
        }

        public FlatFieldCorrection(Bitmap backgroundImage) : this()
        {
            this.backgroundImage = backgroundImage;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image)
        {
            UnmanagedImage unmanagedBackgroundImage = null;
            BitmapData bitmapData = null;
            int width = image.Width;
            int height = image.Height;
            int num3 = image.Stride - ((image.PixelFormat == PixelFormat.Format8bppIndexed) ? width : (width * 3));
            if ((this.backgroundImage == null) && (this.unmanagedBackgroundImage == null))
            {
                ResizeBicubic bicubic = new ResizeBicubic(width / 3, height / 3);
                UnmanagedImage image3 = bicubic.Apply(image);
                GaussianBlur blur = new GaussianBlur(5.0, 0x15);
                blur.ApplyInPlace(image3);
                blur.ApplyInPlace(image3);
                blur.ApplyInPlace(image3);
                blur.ApplyInPlace(image3);
                blur.ApplyInPlace(image3);
                bicubic.NewWidth = width;
                bicubic.NewHeight = height;
                unmanagedBackgroundImage = bicubic.Apply(image3);
                image3.Dispose();
            }
            else if (this.backgroundImage != null)
            {
                if (((width != this.backgroundImage.Width) || (height != this.backgroundImage.Height)) || (image.PixelFormat != this.backgroundImage.PixelFormat))
                {
                    throw new InvalidImagePropertiesException("Source image and background images must have the same size and pixel format");
                }
                bitmapData = this.backgroundImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, this.backgroundImage.PixelFormat);
                unmanagedBackgroundImage = new UnmanagedImage(bitmapData);
            }
            else
            {
                unmanagedBackgroundImage = this.unmanagedBackgroundImage;
            }
            ImageStatistics statistics = new ImageStatistics(unmanagedBackgroundImage);
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            byte* numPtr2 = (byte*) unmanagedBackgroundImage.ImageData.ToPointer();
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                double num4 = statistics.Gray.get_Mean();
                for (int i = 0; i < height; i++)
                {
                    int num6 = 0;
                    while (num6 < width)
                    {
                        if (numPtr2[0] != 0)
                        {
                            numPtr[0] = (byte) Math.Min((double) ((num4 * numPtr[0]) / ((double) numPtr2[0])), (double) 255.0);
                        }
                        num6++;
                        numPtr++;
                        numPtr2++;
                    }
                    numPtr += num3;
                    numPtr2 += num3;
                }
            }
            else
            {
                double num7 = statistics.Red.get_Mean();
                double num8 = statistics.Green.get_Mean();
                double num9 = statistics.Blue.get_Mean();
                for (int j = 0; j < height; j++)
                {
                    int num11 = 0;
                    while (num11 < width)
                    {
                        if (numPtr2[2] != 0)
                        {
                            numPtr[2] = (byte) Math.Min((double) ((num7 * numPtr[2]) / ((double) numPtr2[2])), (double) 255.0);
                        }
                        if (numPtr2[1] != 0)
                        {
                            numPtr[1] = (byte) Math.Min((double) ((num8 * numPtr[1]) / ((double) numPtr2[1])), (double) 255.0);
                        }
                        if (numPtr2[0] != 0)
                        {
                            numPtr[0] = (byte) Math.Min((double) ((num9 * numPtr[0]) / ((double) numPtr2[0])), (double) 255.0);
                        }
                        num11++;
                        numPtr += 3;
                        numPtr2 += 3;
                    }
                    numPtr += num3;
                    numPtr2 += num3;
                }
            }
            if (this.backgroundImage != null)
            {
                this.backgroundImage.UnlockBits(bitmapData);
            }
            if ((this.backgroundImage == null) && (this.unmanagedBackgroundImage == null))
            {
                unmanagedBackgroundImage.Dispose();
            }
        }

        public Bitmap BackgoundImage
        {
            get
            {
                return this.backgroundImage;
            }
            set
            {
                this.backgroundImage = value;
                if (value != null)
                {
                    this.unmanagedBackgroundImage = null;
                }
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public UnmanagedImage UnmanagedBackgoundImage
        {
            get
            {
                return this.unmanagedBackgroundImage;
            }
            set
            {
                this.unmanagedBackgroundImage = value;
                if (value != null)
                {
                    this.backgroundImage = null;
                }
            }
        }
    }
}

