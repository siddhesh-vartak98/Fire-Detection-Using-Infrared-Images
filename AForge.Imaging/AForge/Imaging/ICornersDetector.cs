namespace AForge.Imaging
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public interface ICornersDetector
    {
        List<IntPoint> ProcessImage(UnmanagedImage image);
        List<IntPoint> ProcessImage(Bitmap image);
        List<IntPoint> ProcessImage(BitmapData imageData);
    }
}

