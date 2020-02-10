namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public abstract class BaseInPlaceFilter : IFilter, IInPlaceFilter, IFilterInformation
    {
        protected BaseInPlaceFilter()
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
                this.ProcessFilter(new UnmanagedImage(bitmapData));
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
            this.ProcessFilter(destinationImage);
        }

        public void ApplyInPlace(UnmanagedImage image)
        {
            this.CheckSourceFormat(image.PixelFormat);
            this.ProcessFilter(image);
        }

        public void ApplyInPlace(Bitmap image)
        {
            this.CheckSourceFormat(image.PixelFormat);
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);
            try
            {
                this.ProcessFilter(new UnmanagedImage(bitmapData));
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
        }

        public void ApplyInPlace(BitmapData imageData)
        {
            this.CheckSourceFormat(imageData.PixelFormat);
            this.ProcessFilter(new UnmanagedImage(imageData));
        }

        private void CheckSourceFormat(PixelFormat pixelFormat)
        {
            if (!this.FormatTranslations.ContainsKey(pixelFormat))
            {
                throw new UnsupportedImageFormatException("Source pixel format is not supported by the filter.");
            }
        }

        protected abstract void ProcessFilter(UnmanagedImage image);

        public abstract Dictionary<PixelFormat, PixelFormat> FormatTranslations { get; }
    }
}

