namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public abstract class BaseInPlacePartialFilter : IFilter, IInPlaceFilter, IInPlacePartialFilter, IFilterInformation
    {
        protected BaseInPlacePartialFilter()
        {
        }

        public UnmanagedImage Apply(UnmanagedImage image)
        {
            this.CheckSourceFormat(image.PixelFormat);
            UnmanagedImage destinationImage = UnmanagedImage.Create(image.Width, image.Height, image.PixelFormat);
            this.Apply(image, destinationImage);
            return destinationImage;
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
            PixelFormat pixelFormat = imageData.PixelFormat;
            this.CheckSourceFormat(pixelFormat);
            int width = imageData.Width;
            int height = imageData.Height;
            Bitmap bitmap = (pixelFormat == PixelFormat.Format8bppIndexed) ? Image.CreateGrayscaleImage(width, height) : new Bitmap(width, height, pixelFormat);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, pixelFormat);
            SystemTools.CopyUnmanagedMemory(bitmapData.Scan0, imageData.Scan0, imageData.Stride * height);
            try
            {
                this.ProcessFilter(new UnmanagedImage(bitmapData), new Rectangle(0, 0, width, height));
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
            return bitmap;
        }

        public unsafe void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage)
        {
            this.CheckSourceFormat(sourceImage.PixelFormat);
            if (destinationImage.PixelFormat != sourceImage.PixelFormat)
            {
                throw new InvalidImagePropertiesException("Destination pixel format must be the same as pixel format of source image.");
            }
            if ((destinationImage.Width != sourceImage.Width) || (destinationImage.Height != sourceImage.Height))
            {
                throw new InvalidImagePropertiesException("Destination image must have the same width and height as source image.");
            }
            int stride = destinationImage.Stride;
            int num2 = sourceImage.Stride;
            int count = Math.Min(num2, stride);
            byte* dst = (byte*) destinationImage.ImageData.ToPointer();
            byte* src = (byte*) sourceImage.ImageData.ToPointer();
            int num4 = 0;
            int height = sourceImage.Height;
            while (num4 < height)
            {
                SystemTools.CopyUnmanagedMemory(dst, src, count);
                dst += stride;
                src += num2;
                num4++;
            }
            this.ProcessFilter(destinationImage, new Rectangle(0, 0, destinationImage.Width, destinationImage.Height));
        }

        public void ApplyInPlace(UnmanagedImage image)
        {
            this.CheckSourceFormat(image.PixelFormat);
            this.ProcessFilter(image, new Rectangle(0, 0, image.Width, image.Height));
        }

        public void ApplyInPlace(Bitmap image)
        {
            this.ApplyInPlace(image, new Rectangle(0, 0, image.Width, image.Height));
        }

        public void ApplyInPlace(BitmapData imageData)
        {
            this.CheckSourceFormat(imageData.PixelFormat);
            this.ProcessFilter(new UnmanagedImage(imageData), new Rectangle(0, 0, imageData.Width, imageData.Height));
        }

        public void ApplyInPlace(UnmanagedImage image, Rectangle rect)
        {
            this.CheckSourceFormat(image.PixelFormat);
            rect.Intersect(new Rectangle(0, 0, image.Width, image.Height));
            if ((rect.Width | rect.Height) != 0)
            {
                this.ProcessFilter(image, rect);
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

        protected abstract void ProcessFilter(UnmanagedImage image, Rectangle rect);

        public abstract Dictionary<PixelFormat, PixelFormat> FormatTranslations { get; }
    }
}

