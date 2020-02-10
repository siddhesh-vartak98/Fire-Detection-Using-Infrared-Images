namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public interface IFilter
    {
        UnmanagedImage Apply(UnmanagedImage image);
        Bitmap Apply(Bitmap image);
        Bitmap Apply(BitmapData imageData);
        void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage);
    }
}

