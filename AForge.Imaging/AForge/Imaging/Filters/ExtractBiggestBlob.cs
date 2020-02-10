namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ExtractBiggestBlob : IFilter, IFilterInformation
    {
        private IntPoint blobPosition;
        private Bitmap originalImage;

        public UnmanagedImage Apply(UnmanagedImage image)
        {
            throw new NotImplementedException("The method is not implemented for the filter.");
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
            if (!this.FormatTranslations.ContainsKey(imageData.PixelFormat))
            {
                throw new UnsupportedImageFormatException("Source pixel format is not supported by the filter.");
            }
            BlobCounter counter = new BlobCounter(imageData);
            Blob[] objectsInformation = counter.GetObjectsInformation();
            int num = 0;
            Blob blob = null;
            int index = 0;
            int length = objectsInformation.Length;
            while (index < length)
            {
                int num4 = objectsInformation[index].Rectangle.Width * objectsInformation[index].Rectangle.Height;
                if (num4 > num)
                {
                    num = num4;
                    blob = objectsInformation[index];
                }
                index++;
            }
            if (blob == null)
            {
                throw new ArgumentException("The source image does not contain any blobs.");
            }
            this.blobPosition = new IntPoint(blob.Rectangle.Left, blob.Rectangle.Top);
            if (this.originalImage == null)
            {
                counter.ExtractBlobsImage(new UnmanagedImage(imageData), blob, false);
            }
            else
            {
                if ((((this.originalImage.PixelFormat != PixelFormat.Format24bppRgb) && (this.originalImage.PixelFormat != PixelFormat.Format32bppArgb)) && ((this.originalImage.PixelFormat != PixelFormat.Format32bppRgb) && (this.originalImage.PixelFormat != PixelFormat.Format32bppPArgb))) && (this.originalImage.PixelFormat != PixelFormat.Format8bppIndexed))
                {
                    throw new UnsupportedImageFormatException("Original image may be grayscale (8bpp indexed) or color (24/32bpp) image only.");
                }
                if ((this.originalImage.Width != imageData.Width) || (this.originalImage.Height != imageData.Height))
                {
                    throw new InvalidImagePropertiesException("Original image must have the same size as passed source image.");
                }
                counter.ExtractBlobsImage(this.originalImage, blob, false);
            }
            Bitmap bitmap = blob.Image.ToManagedImage();
            blob.Image.Dispose();
            return bitmap;
        }

        public void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage)
        {
            throw new NotImplementedException("The method is not implemented filter.");
        }

        public IntPoint BlobPosition
        {
            get
            {
                return this.blobPosition;
            }
        }

        public Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                Dictionary<PixelFormat, PixelFormat> dictionary = new Dictionary<PixelFormat, PixelFormat>();
                if (this.originalImage == null)
                {
                    dictionary[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
                    dictionary[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
                    dictionary[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
                    dictionary[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
                    dictionary[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
                    return dictionary;
                }
                dictionary[PixelFormat.Format8bppIndexed] = this.originalImage.PixelFormat;
                dictionary[PixelFormat.Format24bppRgb] = this.originalImage.PixelFormat;
                dictionary[PixelFormat.Format32bppArgb] = this.originalImage.PixelFormat;
                dictionary[PixelFormat.Format32bppRgb] = this.originalImage.PixelFormat;
                dictionary[PixelFormat.Format32bppPArgb] = this.originalImage.PixelFormat;
                return dictionary;
            }
        }

        public Bitmap OriginalImage
        {
            get
            {
                return this.originalImage;
            }
            set
            {
                this.originalImage = value;
            }
        }
    }
}

