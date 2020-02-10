namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class HSLFiltering : BaseInPlacePartialFilter
    {
        private int fillH;
        private double fillL;
        private bool fillOutsideRange;
        private double fillS;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private IntRange hue;
        private DoubleRange luminance;
        private DoubleRange saturation;
        private bool updateH;
        private bool updateL;
        private bool updateS;

        public HSLFiltering()
        {
            this.hue = new IntRange(0, 0x167);
            this.saturation = new DoubleRange(0.0, 1.0);
            this.luminance = new DoubleRange(0.0, 1.0);
            this.fillOutsideRange = true;
            this.updateH = true;
            this.updateS = true;
            this.updateL = true;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        public HSLFiltering(IntRange hue, DoubleRange saturation, DoubleRange luminance) : this()
        {
            this.hue = hue;
            this.saturation = saturation;
            this.luminance = luminance;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            int num = (image.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
            int left = rect.Left;
            int top = rect.Top;
            int num4 = left + rect.Width;
            int num5 = top + rect.Height;
            int num6 = image.Stride - (rect.Width * num);
            RGB rgb = new RGB();
            HSL hsl = new HSL();
            byte* numPtr = (byte*) (image.ImageData.ToPointer() + ((top * image.Stride) + (left * num)));
            for (int i = top; i < num5; i++)
            {
                int num8 = left;
                while (num8 < num4)
                {
                    bool flag = false;
                    rgb.Red = numPtr[2];
                    rgb.Green = numPtr[1];
                    rgb.Blue = numPtr[0];
                    HSL.FromRGB(rgb, hsl);
                    if ((((hsl.Saturation >= this.saturation.Min) && (hsl.Saturation <= this.saturation.Max)) && ((hsl.Luminance >= this.luminance.Min) && (hsl.Luminance <= this.luminance.Max))) && ((((this.hue.Min < this.hue.Max) && (hsl.Hue >= this.hue.Min)) && (hsl.Hue <= this.hue.Max)) || ((this.hue.Min > this.hue.Max) && ((hsl.Hue >= this.hue.Min) || (hsl.Hue <= this.hue.Max)))))
                    {
                        if (!this.fillOutsideRange)
                        {
                            if (this.updateH)
                            {
                                hsl.Hue = this.fillH;
                            }
                            if (this.updateS)
                            {
                                hsl.Saturation = this.fillS;
                            }
                            if (this.updateL)
                            {
                                hsl.Luminance = this.fillL;
                            }
                            flag = true;
                        }
                    }
                    else if (this.fillOutsideRange)
                    {
                        if (this.updateH)
                        {
                            hsl.Hue = this.fillH;
                        }
                        if (this.updateS)
                        {
                            hsl.Saturation = this.fillS;
                        }
                        if (this.updateL)
                        {
                            hsl.Luminance = this.fillL;
                        }
                        flag = true;
                    }
                    if (flag)
                    {
                        HSL.ToRGB(hsl, rgb);
                        numPtr[2] = rgb.Red;
                        numPtr[1] = rgb.Green;
                        numPtr[0] = rgb.Blue;
                    }
                    num8++;
                    numPtr += num;
                }
                numPtr += num6;
            }
        }

        public HSL FillColor
        {
            get
            {
                return new HSL(this.fillH, this.fillS, this.fillL);
            }
            set
            {
                this.fillH = value.Hue;
                this.fillS = value.Saturation;
                this.fillL = value.Luminance;
            }
        }

        public bool FillOutsideRange
        {
            get
            {
                return this.fillOutsideRange;
            }
            set
            {
                this.fillOutsideRange = value;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public IntRange Hue
        {
            get
            {
                return this.hue;
            }
            set
            {
                this.hue = value;
            }
        }

        public DoubleRange Luminance
        {
            get
            {
                return this.luminance;
            }
            set
            {
                this.luminance = value;
            }
        }

        public DoubleRange Saturation
        {
            get
            {
                return this.saturation;
            }
            set
            {
                this.saturation = value;
            }
        }

        public bool UpdateHue
        {
            get
            {
                return this.updateH;
            }
            set
            {
                this.updateH = value;
            }
        }

        public bool UpdateLuminance
        {
            get
            {
                return this.updateL;
            }
            set
            {
                this.updateL = value;
            }
        }

        public bool UpdateSaturation
        {
            get
            {
                return this.updateS;
            }
            set
            {
                this.updateS = value;
            }
        }
    }
}

