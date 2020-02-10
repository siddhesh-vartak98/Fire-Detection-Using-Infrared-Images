namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Reflection;

    public class FiltersSequence : CollectionBase, IFilter
    {
        public FiltersSequence()
        {
        }

        public FiltersSequence(params IFilter[] filters)
        {
            base.InnerList.AddRange(filters);
        }

        public void Add(IFilter filter)
        {
            base.InnerList.Add(filter);
        }

        public UnmanagedImage Apply(UnmanagedImage image)
        {
            int count = base.InnerList.Count;
            if (count == 0)
            {
                throw new ApplicationException("No filters in the sequence.");
            }
            UnmanagedImage image2 = null;
            UnmanagedImage image3 = null;
            image2 = ((IFilter) base.InnerList[0]).Apply(image);
            for (int i = 1; i < count; i++)
            {
                image3 = image2;
                image2 = ((IFilter) base.InnerList[i]).Apply(image3);
                image3.Dispose();
            }
            return image2;
        }

        public Bitmap Apply(Bitmap image)
        {
            Bitmap bitmap = null;
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
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
            UnmanagedImage image = this.Apply(new UnmanagedImage(imageData));
            Bitmap bitmap = image.ToManagedImage();
            image.Dispose();
            return bitmap;
        }

        public void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage)
        {
            int count = base.InnerList.Count;
            switch (count)
            {
                case 0:
                    throw new ApplicationException("No filters in the sequence.");

                case 1:
                    ((IFilter) base.InnerList[0]).Apply(sourceImage, destinationImage);
                    return;
            }
            UnmanagedImage image = null;
            UnmanagedImage image2 = null;
            image = ((IFilter) base.InnerList[0]).Apply(sourceImage);
            count--;
            for (int i = 1; i < count; i++)
            {
                image2 = image;
                image = ((IFilter) base.InnerList[i]).Apply(image2);
                image2.Dispose();
            }
            ((IFilter) base.InnerList[count]).Apply(image, destinationImage);
        }

        public IFilter this[int index]
        {
            get
            {
                return (IFilter) base.InnerList[index];
            }
        }
    }
}

