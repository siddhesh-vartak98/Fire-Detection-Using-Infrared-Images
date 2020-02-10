namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public abstract class BaseUsingCopyPartialFilter : IFilter, IInPlaceFilter, IInPlacePartialFilter, IFilterInformation
    {
        protected BaseUsingCopyPartialFilter()
        {
        }

        public UnmanagedImage Apply(UnmanagedImage image)
        {
            this.CheckSourceFormat(image.PixelFormat);
            UnmanagedImage destinationData = UnmanagedImage.Create(image.Width, image.Height, this.FormatTranslations[image.PixelFormat]);
            this.ProcessFilter(image, destinationData, new Rectangle(0, 0, image.Width, image.Height));
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
                this.ProcessFilter(new UnmanagedImage(imageData), new UnmanagedImage(bitmapData), new Rectangle(0, 0, width, height));
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
            this.ProcessFilter(sourceImage, destinationImage, new Rectangle(0, 0, sourceImage.Width, sourceImage.Height));
        }

        public void ApplyInPlace(UnmanagedImage image)
        {
            this.ApplyInPlace(image, new Rectangle(0, 0, image.Width, image.Height));
        }

        public void ApplyInPlace(Bitmap image)
        {
            this.ApplyInPlace(image, new Rectangle(0, 0, image.Width, image.Height));
        }

        public void ApplyInPlace(BitmapData imageData)
        {
            this.ApplyInPlace(new UnmanagedImage(imageData), new Rectangle(0, 0, imageData.Width, imageData.Height));
        }

        public void ApplyInPlace(UnmanagedImage image, Rectangle rect)
        {
            this.CheckSourceFormat(image.PixelFormat);
            rect.Intersect(new Rectangle(0, 0, image.Width, image.Height));
            if ((rect.Width | rect.Height) != 0)
            {
                int size = image.Stride * image.Height;
                IntPtr dst = MemoryManager.Alloc(size);
                SystemTools.CopyUnmanagedMemory(dst, image.ImageData, size);
                this.ProcessFilter(new UnmanagedImage(dst, image.Width, image.Height, image.Stride, image.PixelFormat), image, rect);
                MemoryManager.Free(dst);
            }
        }

        public void ApplyInPlace(Bitmap image, Rectangle rect)
        {
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);
            try
            {
                this.ApplyInPlace(new UnmanagedImage(bitmapData), rect);
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
        }

        public void ApplyInPlace(BitmapData imageData, Rectangle rect)
        {
            this.ApplyInPlace(new UnmanagedImage(imageData), rect);
        }

        private void CheckSourceFormat(PixelFormat pixelFormat)
        {
            if (!this.FormatTranslations.ContainsKey(pixelFormat))
            {
                throw new UnsupportedImageFormatException("Source pixel format is not supported by the filter.");
            }
        }

        protected abstract void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData, Rectangle rect);

        public abstract Dictionary<PixelFormat, PixelFormat> FormatTranslations { get; }
    }
}

