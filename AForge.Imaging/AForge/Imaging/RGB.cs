namespace AForge.Imaging
{
    using System;
    using System.Drawing;

    public class RGB
    {
        public const short A = 3;
        public const short B = 0;
        public byte Blue;
        public const short G = 1;
        public byte Green;
        public const short R = 2;
        public byte Red;

        public RGB()
        {
        }

        public RGB(System.Drawing.Color color)
        {
            this.Red = color.R;
            this.Green = color.G;
            this.Blue = color.B;
        }

        public RGB(byte red, byte green, byte blue)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }

        public System.Drawing.Color Color
        {
            get
            {
                return System.Drawing.Color.FromArgb(this.Red, this.Green, this.Blue);
            }
            set
            {
                this.Red = value.R;
                this.Green = value.G;
                this.Blue = value.B;
            }
        }
    }
}

