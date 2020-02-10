namespace AForge
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct DoublePoint
    {
        public double X;
        public double Y;
        public DoublePoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double DistanceTo(DoublePoint anotherPoint)
        {
            double num = this.X - anotherPoint.X;
            double num2 = this.Y - anotherPoint.Y;
            return Math.Sqrt((num * num) + (num2 * num2));
        }

        public static DoublePoint operator +(DoublePoint p1, DoublePoint p2)
        {
            return new DoublePoint(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static DoublePoint operator -(DoublePoint p1, DoublePoint p2)
        {
            return new DoublePoint(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static DoublePoint operator +(DoublePoint p, double valueToAdd)
        {
            return new DoublePoint(p.X + valueToAdd, p.Y + valueToAdd);
        }

        public static DoublePoint operator -(DoublePoint p, double valueToSubtract)
        {
            return new DoublePoint(p.X - valueToSubtract, p.Y - valueToSubtract);
        }

        public static DoublePoint operator *(DoublePoint p, double factor)
        {
            return new DoublePoint(p.X * factor, p.Y * factor);
        }

        public static DoublePoint operator /(DoublePoint p, double factor)
        {
            return new DoublePoint(p.X / factor, p.Y / factor);
        }

        public static bool operator ==(DoublePoint p1, DoublePoint p2)
        {
            return ((p1.X == p2.X) && (p1.Y == p2.Y));
        }

        public static bool operator !=(DoublePoint p1, DoublePoint p2)
        {
            if (p1.X == p2.X)
            {
                return (p1.Y != p2.Y);
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return ((obj is DoublePoint) && (this == ((DoublePoint) obj)));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static explicit operator IntPoint(DoublePoint p)
        {
            return new IntPoint((int) p.X, (int) p.Y);
        }

        public IntPoint Round()
        {
            return new IntPoint((int) Math.Round(this.X), (int) Math.Round(this.Y));
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", this.X, this.Y);
        }

        public double EuclideanNorm()
        {
            return Math.Sqrt((this.X * this.X) + (this.Y * this.Y));
        }
    }
}

