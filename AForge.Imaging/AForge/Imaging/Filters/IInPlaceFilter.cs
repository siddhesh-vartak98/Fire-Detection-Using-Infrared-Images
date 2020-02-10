namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public interface IInPlaceFilter
    {
        void ApplyInPlace(UnmanagedImage image);
        void ApplyInPlace(Bitmap image);
        void ApplyInPlace(BitmapData imageData);
    }
}

