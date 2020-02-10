namespace AForge.Imaging
{
    using System;

    public class HoughLine : IComparable
    {
        public readonly short Intensity;
        public readonly short Radius;
        public readonly double RelativeIntensity;
        public readonly double Theta;

        public HoughLine(double theta, short radius, short intensity, double relativeIntensity)
        {
            this.Theta = theta;
            this.Radius = radius;
            this.Intensity = intensity;
            this.RelativeIntensity = relativeIntensity;
        }

        public int CompareTo(object value)
        {
            return -this.Intensity.CompareTo(((HoughLine) value).Intensity);
        }
    }
}

