namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class FilterIterator : IFilter, IFilterInformation
    {
        private IFilter baseFilter;
        private int iterations;

        public FilterIterator(IFilter baseFilter)
        {
            this.iterations = 1;
            this.baseFilter = baseFilter;
        }

        public FilterIterator(IFilter baseFilter, int iterations)
        {
            this.iterations = 1;
            this.baseFilter = baseFilter;
            this.iterations = iterations;
        }

        public UnmanagedImage Apply(UnmanagedImage image)
        {
            UnmanagedImage image2 = this.baseFilter.Apply(image);
            for (int i = 1; i < this.iterations; i++)
            {
                UnmanagedImage image3 = image2;
                image2 = this.baseFilter.Apply(image3);
                image3.Dispose();
            }
            return image2;
        }

        public Bitmap Apply(Bitmap image)
        {
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            Bitmap bitmap = this.Apply(imageData);
            image.UnlockBits(imageData);
            return bitmap;
        }

        public Bitmap Apply(BitmapData imageData)
        {
            Bitmap bitmap = this.baseFilter.Apply(imageData);
            for (int i = 1; i < this.iterations; i++)
            {
                Bitmap image = bitmap;
                bitmap = this.baseFilter.Apply(image);
                image.Dispose();
            }
            return bitmap;
        }

        public void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage)
        {
            if (this.iterations == 1)
            {
                this.baseFilter.Apply(sourceImage, destinationImage);
            }
            else
            {
                UnmanagedImage image = this.baseFilter.Apply(sourceImage);
                this.iterations--;
                for (int i = 1; i < this.iterations; i++)
                {
                    UnmanagedImage image2 = image;
                    image = this.baseFilter.Apply(image2);
                    image2.Dispose();
                }
                this.baseFilter.Apply(image, destinationImage);
            }
        }

        public IFilter BaseFilter
        {
            get
            {
                return this.baseFilter;
            }
            set
            {
                this.baseFilter = value;
            }
        }

        public Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return ((IFilterInformation) this.baseFilter).FormatTranslations;
            }
        }

        public int Iterations
        {
            get
            {
                return this.iterations;
            }
            set
            {
                this.iterations = Math.Max(1, Math.Min(0xff, value));
            }
        }
    }
}

