namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Drawing;

    public abstract class BaseRotateFilter : BaseTransformationFilter
    {
        protected double angle;
        protected Color fillColor;
        protected bool keepSize;

        public BaseRotateFilter(double angle)
        {
            this.fillColor = Color.FromArgb(0, 0, 0);
            this.angle = angle;
        }

        public BaseRotateFilter(double angle, bool keepSize)
        {
            this.fillColor = Color.FromArgb(0, 0, 0);
            this.angle = angle;
            this.keepSize = keepSize;
        }

        protected override Size CalculateNewImageSize(UnmanagedImage sourceData)
        {
            if (this.keepSize)
            {
                return new Size(sourceData.Width, sourceData.Height);
            }
            double d = (-this.angle * 3.1415926535897931) / 180.0;
            double num2 = Math.Cos(d);
            double num3 = Math.Sin(d);
            double num4 = ((double) sourceData.Width) / 2.0;
            double num5 = ((double) sourceData.Height) / 2.0;
            double num6 = num4 * num2;
            double num7 = num4 * num3;
            double num8 = (num4 * num2) - (num5 * num3);
            double num9 = (num4 * num3) + (num5 * num2);
            double num10 = -num5 * num3;
            double num11 = num5 * num2;
            double num12 = 0.0;
            double num13 = 0.0;
            num4 = Math.Max(Math.Max(num6, num8), Math.Max(num10, num12)) - Math.Min(Math.Min(num6, num8), Math.Min(num10, num12));
            num5 = Math.Max(Math.Max(num7, num9), Math.Max(num11, num13)) - Math.Min(Math.Min(num7, num9), Math.Min(num11, num13));
            return new Size((int) ((num4 * 2.0) + 0.5), (int) ((num5 * 2.0) + 0.5));
        }

        public double Angle
        {
            get
            {
                return this.angle;
            }
            set
            {
                this.angle = value % 360.0;
            }
        }

        public Color FillColor
        {
            get
            {
                return this.fillColor;
            }
            set
            {
                this.fillColor = value;
            }
        }

        public bool KeepSize
        {
            get
            {
                return this.keepSize;
            }
            set
            {
                this.keepSize = value;
            }
        }
    }
}

