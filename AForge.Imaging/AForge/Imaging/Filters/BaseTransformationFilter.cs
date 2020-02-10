namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public abstract class BaseTransformationFilter : IFilter, IFilterInformation
    {
        protected BaseTransformationFilter()
        {
        }

        public UnmanagedImage Apply(UnmanagedImage image)
        {
            this.CheckSourceFormat(image.PixelFormat);
            Size size = this.CalculateNewImageSize(image);
            UnmanagedImage destinationData = UnmanagedImage.Create(size.Width, size.Height, this.FormatTranslations[image.PixelFormat]);
            this.ProcessFilter(image, destinationData);
            return destinationData;
        }

        public Bitmap Apply(Bitmap image)
        {
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            Bitmap bitmap = null;
            try
            {
                bitmap = this.Apply(imageData);
            }
            finally
            {
                image.UnlockBits(imageData);
            }
            return bitmap;
        }

        public Bitmap Apply(BitmapData imageData)
        {
            this.CheckSourceFormat(imageData.PixelFormat);
            PixelFormat format = this.FormatTranslations[imageData.PixelFormat];
            Size size = this.CalculateNewImageSize(new UnmanagedImage(imageData));
            Bitmap bitmap = (format == PixelFormat.Format8bppIndexed) ? Image.CreateGrayscaleImage(size.Width, size.Height) : new Bitmap(size.Width, size.Height, format);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, size.Width, size.Height), ImageLockMode.ReadWrite, format);
            try
            {
                this.ProcessFilter(new UnmanagedImage(imageData), new UnmanagedImage(bitmapData));
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
            return bitmap;
        }

        public void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage)
        {
            this.CheckSourceFormat(sourceImage.PixelFormat);
            if (destinationImage.PixelFormat != ((PixelFormat) this.FormatTranslations[sourceImage.PixelFormat]))
            {
                throw new InvalidImagePropertiesException("Destination pixel format is specified incorrectly.");
            }
            Size size = this.CalculateNewImageSize(sourceImage);
            if ((destinationImage.Width != size.Width) || (destinationImage.Height != size.Height))
            {
                throw new InvalidImagePropertiesException("Destination image must have the size expected by the filter.");
            }
            this.ProcessFilter(sourceImage, destinationImage);
        }

        protected abstract Size CalculateNewImageSize(UnmanagedImage sourceData);
        private void CheckSourceFormat(PixelFormat pixelFormat)
        {
            if (!this.FormatTranslations.ContainsKey(pixelFormat))
            {
                throw new UnsupportedImageFormatException("Source pixel format is not supported by the filter.");
            }
        }

        protected abstract void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData);

        public abstract Dictionary<PixelFormat, PixelFormat> FormatTranslations { get; }
    }
}

