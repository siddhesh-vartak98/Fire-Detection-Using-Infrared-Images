namespace AForge.Imaging
{
    using System;

    public class YCbCr
    {
        public double Cb;
        public const short CbIndex = 1;
        public double Cr;
        public const short CrIndex = 2;
        public double Y;
        public const short YIndex = 0;

        public YCbCr()
        {
        }

        public YCbCr(double y, double cb, double cr)
        {
            this.Y = Math.Max(0.0, Math.Min(1.0, y));
            this.Cb = Math.Max(-0.5, Math.Min(0.5, cb));
            this.Cr = Math.Max(-0.5, Math.Min(0.5, cr));
        }

        public static YCbCr FromRGB(RGB rgb)
        {
            YCbCr ycbcr = new YCbCr();
            FromRGB(rgb, ycbcr);
            return ycbcr;
        }

        public static void FromRGB(RGB rgb, YCbCr ycbcr)
        {
            double num = ((double) rgb.Red) / 255.0;
            double num2 = ((double) rgb.Green) / 255.0;
            double num3 = ((double) rgb.Blue) / 255.0;
            ycbcr.Y = ((0.2989 * num) + (0.5866 * num2)) + (0.1145 * num3); //image transform equation to get y component
            ycbcr.Cb = ((-0.1687 * num) - (0.3313 * num2)) + (0.5 * num3); //image transform equation to get Cb component
            ycbcr.Cr = ((0.5 * num) - (0.4184 * num2)) - (0.0816 * num3); //image transform equation to get Cr component
        }

        public RGB ToRGB()
        {
            RGB rgb = new RGB();
            ToRGB(this, rgb);
            return rgb;
        }

        public static void ToRGB(YCbCr ycbcr, RGB rgb)
        {
            double num = Math.Max(0.0, Math.Min((double) 1.0, (double) ((ycbcr.Y + (0.0 * ycbcr.Cb)) + (1.4022 * ycbcr.Cr))));
            double num2 = Math.Max(0.0, Math.Min((double) 1.0, (double) ((ycbcr.Y - (0.3456 * ycbcr.Cb)) - (0.7145 * ycbcr.Cr))));
            double num3 = Math.Max(0.0, Math.Min((double) 1.0, (double) ((ycbcr.Y + (1.771 * ycbcr.Cb)) + (0.0 * ycbcr.Cr))));
            rgb.Red = (byte) (num * 255.0);
            rgb.Green = (byte) (num2 * 255.0);
            rgb.Blue = (byte) (num3 * 255.0);
        }
    }
}

