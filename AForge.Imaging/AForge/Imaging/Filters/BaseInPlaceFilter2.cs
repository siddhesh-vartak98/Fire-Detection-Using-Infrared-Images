namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public abstract class BaseInPlaceFilter2 : BaseInPlaceFilter
    {
        private Bitmap overlayImage;
        private UnmanagedImage unmanagedOverlayImage;

        protected BaseInPlaceFilter2()
        {
        }

        protected BaseInPlaceFilter2(UnmanagedImage unmanagedOverlayImage)
        {
            this.unmanagedOverlayImage = unmanagedOverlayImage;
        }

        protected BaseInPlaceFilter2(Bitmap overlayImage)
        {
            this.overlayImage = overlayImage;
        }

        protected override void ProcessFilter(UnmanagedImage image)
        {
            PixelFormat pixelFormat = image.PixelFormat;
            int width = image.Width;
            int height = image.Height;
            if (this.overlayImage != null)
            {
                if (pixelFormat != this.overlayImage.PixelFormat)
                {
                    throw new InvalidImagePropertiesException("Source and overlay images must have same pixel format.");
                }
                if ((width != this.overlayImage.Width) || (height != this.overlayImage.Height))
                {
                    throw new InvalidImagePropertiesException("Overlay image size must be equal to source image size.");
                }
                BitmapData bitmapData = this.overlayImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, pixelFormat);
                try
                {
                    this.ProcessFilter(image, new UnmanagedImage(bitmapData));
                    return;
                }
                finally
                {
                    this.overlayImage.UnlockBits(bitmapData);
                }
            }
            if (this.unmanagedOverlayImage == null)
            {
                throw new NullReferenceException("Overlay image is not set.");
            }
            if (pixelFormat != this.unmanagedOverlayImage.PixelFormat)
            {
                throw new InvalidImagePropertiesException("Source and overlay images must have same pixel format.");
            }
            if ((width != this.unmanagedOverlayImage.Width) || (height != this.unmanagedOverlayImage.Height))
            {
                throw new InvalidImagePropertiesException("Overlay image size must be equal to source image size.");
            }
            this.ProcessFilter(image, this.unmanagedOverlayImage);
        }

        protected abstract void ProcessFilter(UnmanagedImage image, UnmanagedImage overlay);

        public Bitmap OverlayImage
        {
            get
            {
                return this.overlayImage;
            }
            set
            {
                this.overlayImage = value;
                if (value != null)
                {
                    this.unmanagedOverlayImage = null;
                }
            }
        }

        public UnmanagedImage UnmanagedOverlayImage
        {
            get
            {
                return this.unmanagedOverlayImage;
            }
            set
            {
                this.unmanagedOverlayImage = value;
                if (value != null)
                {
                    this.overlayImage = null;
                }
            }
        }
    }
}

