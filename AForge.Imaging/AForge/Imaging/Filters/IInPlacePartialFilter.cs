namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public interface IInPlacePartialFilter
    {
        void ApplyInPlace(UnmanagedImage image, Rectangle rect);
        void ApplyInPlace(Bitmap image, Rectangle rect);
        void ApplyInPlace(BitmapData imageData, Rectangle rect);
    }
}

