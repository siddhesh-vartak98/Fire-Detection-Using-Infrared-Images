namespace AForge.Imaging
{
    using System;

    public class HSL
    {
        public int Hue;
        public double Luminance;
        public double Saturation;

        public HSL()
        {
        }

        public HSL(int hue, double saturation, double luminance)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Luminance = luminance;
        }

        public static HSL FromRGB(RGB rgb)
        {
            HSL hsl = new HSL();
            FromRGB(rgb, hsl);
            return hsl;
        }

        public static void FromRGB(RGB rgb, HSL hsl)
        {
            double num = ((double) rgb.Red) / 255.0;
            double num2 = ((double) rgb.Green) / 255.0;
            double num3 = ((double) rgb.Blue) / 255.0;
            double num4 = Math.Min(Math.Min(num, num2), num3);
            double num5 = Math.Max(Math.Max(num, num2), num3);
            double num6 = num5 - num4;
            hsl.Luminance = (num5 + num4) / 2.0;
            if (num6 == 0.0)
            {
                hsl.Hue = 0;
                hsl.Saturation = 0.0;
            }
            else
            {
                double num7;
                hsl.Saturation = (hsl.Luminance <= 0.5) ? (num6 / (num5 + num4)) : (num6 / ((2.0 - num5) - num4));
                if (num == num5)
                {
                    num7 = ((num2 - num3) / 6.0) / num6;
                }
                else if (num2 == num5)
                {
                    num7 = 0.33333333333333331 + (((num3 - num) / 6.0) / num6);
                }
                else
                {
                    num7 = 0.66666666666666663 + (((num - num2) / 6.0) / num6);
                }
                if (num7 < 0.0)
                {
                    num7++;
                }
                if (num7 > 1.0)
                {
                    num7--;
                }
                hsl.Hue = (int) (num7 * 360.0);
            }
        }

        private static double Hue_2_RGB(double v1, double v2, double vH)
        {
            if (vH < 0.0)
            {
                vH++;
            }
            if (vH > 1.0)
            {
                vH--;
            }
            if ((6.0 * vH) < 1.0)
            {
                return (v1 + (((v2 - v1) * 6.0) * vH));
            }
            if ((2.0 * vH) < 1.0)
            {
                return v2;
            }
            if ((3.0 * vH) < 2.0)
            {
                return (v1 + (((v2 - v1) * (0.66666666666666663 - vH)) * 6.0));
            }
            return v1;
        }

        public RGB ToRGB()
        {
            RGB rgb = new RGB();
            ToRGB(this, rgb);
            return rgb;
        }

        public static void ToRGB(HSL hsl, RGB rgb)
        {
            if (hsl.Saturation == 0.0)
            {
                rgb.Red = rgb.Green = rgb.Blue = (byte) (hsl.Luminance * 255.0);
            }
            else
            {
                double vH = ((double) hsl.Hue) / 360.0;
                double num2 = (hsl.Luminance < 0.5) ? (hsl.Luminance * (1.0 + hsl.Saturation)) : ((hsl.Luminance + hsl.Saturation) - (hsl.Luminance * hsl.Saturation));
                double num = (2.0 * hsl.Luminance) - num2;
                rgb.Red = (byte) (255.0 * Hue_2_RGB(num, num2, vH + 0.33333333333333331));
                rgb.Green = (byte) (255.0 * Hue_2_RGB(num, num2, vH));
                rgb.Blue = (byte) (255.0 * Hue_2_RGB(num, num2, vH - 0.33333333333333331));
            }
        }
    }
}

