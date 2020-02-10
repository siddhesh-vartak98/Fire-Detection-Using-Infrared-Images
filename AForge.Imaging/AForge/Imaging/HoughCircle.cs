namespace AForge.Imaging
{
    using System;

    public class HoughCircle : IComparable
    {
        public readonly short Intensity;
        public readonly int Radius;
        public readonly double RelativeIntensity;
        public readonly int X;
        public readonly int Y;

        public HoughCircle(int x, int y, int radius, short intensity, double relativeIntensity)
        {
            this.X = x;
            this.Y = y;
            this.Radius = radius;
            this.Intensity = intensity;
            this.RelativeIntensity = relativeIntensity;
        }

        public int CompareTo(object value)
        {
            return -this.Intensity.CompareTo(((HoughCircle) value).Intensity);
        }
    }
}

