namespace AForge.Imaging
{
    using System.Drawing;
    using System.Drawing.Imaging;

    public interface ITemplateMatching
    {
        TemplateMatch[] ProcessImage(UnmanagedImage image, UnmanagedImage template, Rectangle searchZone);
        TemplateMatch[] ProcessImage(Bitmap image, Bitmap template, Rectangle searchZone);
        TemplateMatch[] ProcessImage(BitmapData imageData, BitmapData templateData, Rectangle searchZone);
    }
}

