namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class Closing : IFilter, IInPlaceFilter, IInPlacePartialFilter, IFilterInformation
    {
        private Dilatation dilatation;
        private Erosion errosion;

        public Closing()
        {
            this.errosion = new Erosion();
            this.dilatation = new Dilatation();
        }

        public Closing(short[,] se)
        {
            this.errosion = new Erosion();
            this.dilatation = new Dilatation();
            this.errosion = new Erosion(se);
            this.dilatation = new Dilatation(se);
        }

        public UnmanagedImage Apply(UnmanagedImage image)
        {
            UnmanagedImage image2 = this.dilatation.Apply(image);
            this.errosion.ApplyInPlace(image2);
            return image2;
        }

        public Bitmap Apply(Bitmap image)
        {
            Bitmap bitmap = this.dilatation.Apply(image);
            Bitmap bitmap2 = this.errosion.Apply(bitmap);
            bitmap.Dispose();
            return bitmap2;
        }

        public Bitmap Apply(BitmapData imageData)
        {
            Bitmap image = this.dilatation.Apply(imageData);
            Bitmap bitmap2 = this.errosion.Apply(image);
            image.Dispose();
            return bitmap2;
        }

        public void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage)
        {
            this.dilatation.Apply(sourceImage, destinationImage);
            this.errosion.ApplyInPlace(destinationImage);
        }

        public void ApplyInPlace(UnmanagedImage image)
        {
            this.dilatation.ApplyInPlace(image);
            this.errosion.ApplyInPlace(image);
        }

        public void ApplyInPlace(Bitmap image)
        {
            this.dilatation.ApplyInPlace(image);
            this.errosion.ApplyInPlace(image);
        }

        public void ApplyInPlace(BitmapData imageData)
        {
            this.dilatation.ApplyInPlace(imageData);
            this.errosion.ApplyInPlace(imageData);
        }

        public void ApplyInPlace(UnmanagedImage image, Rectangle rect)
        {
            this.dilatation.ApplyInPlace(image, rect);
            this.errosion.ApplyInPlace(image, rect);
        }

        public void ApplyInPlace(Bitmap image, Rectangle rect)
        {
            this.dilatation.ApplyInPlace(image, rect);
            this.errosion.ApplyInPlace(image, rect);
        }

        public void ApplyInPlace(BitmapData imageData, Rectangle rect)
        {
            this.dilatation.ApplyInPlace(imageData, rect);
            this.errosion.ApplyInPlace(imageData, rect);
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

