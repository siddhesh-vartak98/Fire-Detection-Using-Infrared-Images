namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public abstract class BaseFilter : IFilter, IFilterInformation
    {
        protected BaseFilter()
        {
        }

        public UnmanagedImage Apply(UnmanagedImage image)
        {
            this.CheckSourceFormat(image.PixelFormat);
            UnmanagedImage destinationData = UnmanagedImage.Create(image.Width, image.Height, this.FormatTranslations[image.PixelFormat]);
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
            int width = imageData.Width;
            int height = imageData.Height;
            PixelFormat format = this.FormatTranslations[imageData.PixelFormat];
            Bitmap bitmap = (format == PixelFormat.Format8bppIndexed) ? Image.CreateGrayscaleImage(width, height) : new Bitmap(width, height, format);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, format);
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
            if ((destinationImage.Width != sourceImage.Width) || (destinationImage.Height != sourceImage.Height))
            {
                throw new InvalidImagePropertiesException("Destination image must have the same width and height as source image.");
            }
            this.ProcessFilter(sourceImage, destinationImage);
        }

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

