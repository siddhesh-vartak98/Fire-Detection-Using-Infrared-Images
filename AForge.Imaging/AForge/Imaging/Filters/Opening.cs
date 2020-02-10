namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class Opening : IFilter, IInPlaceFilter, IInPlacePartialFilter, IFilterInformation
    {
        private Dilatation dilatation;
        private Erosion errosion;

        public Opening()
        {
            this.errosion = new Erosion();
            this.dilatation = new Dilatation();
        }

        public Opening(short[,] se)
        {
            this.errosion = new Erosion();
            this.dilatation = new Dilatation();
            this.errosion = new Erosion(se);
            this.dilatation = new Dilatation(se);
        }

        public UnmanagedImage Apply(UnmanagedImage image)
        {
            UnmanagedImage image2 = this.errosion.Apply(image);
            this.dilatation.ApplyInPlace(image2);
            return image2;
        }

        public Bitmap Apply(Bitmap image)
        {
            Bitmap bitmap = this.errosion.Apply(image);
            Bitmap bitmap2 = this.dilatation.Apply(bitmap);
            bitmap.Dispose();
            return bitmap2;
        }

        public Bitmap Apply(BitmapData imageData)
        {
            Bitmap image = this.errosion.Apply(imageData);
            Bitmap bitmap2 = this.dilatation.Apply(image);
            image.Dispose();
            return bitmap2;
        }

        public void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage)
        {
            this.errosion.Apply(sourceImage, destinationImage);
            this.dilatation.ApplyInPlace(destinationImage);
        }

        public void ApplyInPlace(UnmanagedImage image)
        {
            this.errosion.ApplyInPlace(image);
            this.dilatation.ApplyInPlace(image);
        }

        public void ApplyInPlace(Bitmap image)
        {
            this.errosion.ApplyInPlace(image);
            this.dilatation.ApplyInPlace(image);
        }

        public void ApplyInPlace(BitmapData imageData)
        {
            this.errosion.ApplyInPlace(imageData);
            this.dilatation.ApplyInPlace(imageData);
        }

        public void ApplyInPlace(UnmanagedImage image, Rectangle rect)
        {
            this.errosion.ApplyInPlace(image, rect);
            this.dilatation.ApplyInPlace(image, rect);
        }

        public void ApplyInPlace(Bitmap image, Rectangle rect)
        {
            this.errosion.ApplyInPlace(image, rect);
            this.dilatation.ApplyInPlace(image, rect);
        }

        public void ApplyInPlace(BitmapData imageData, Rectangle rect)
        {
            this.errosion.ApplyInPlace(imageData, rect);
            this.dilatation.ApplyInPlace(imageData, rect);
        }

        public Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.errosion.FormatTranslations;
            }
        }
    }
}

