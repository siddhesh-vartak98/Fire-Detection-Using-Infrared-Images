namespace AForge
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct IntPoint
    {
        public int X;
        public int Y;
        public IntPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public double DistanceTo(IntPoint anotherPoint)
        {
            int num = this.X - anotherPoint.X;
            int num2 = this.Y - anotherPoint.Y;
            return Math.Sqrt((double) ((num * num) + (num2 * num2)));
        }

        public static IntPoint operator +(IntPoint p1, IntPoint p2)
        {
            return new IntPoint(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static IntPoint operator -(IntPoint p1, IntPoint p2)
        {
            return new IntPoint(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static IntPoint operator +(IntPoint p, int valueToAdd)
        {
            return new IntPoint(p.X + valueToAdd, p.Y + valueToAdd);
        }

        public static IntPoint operator -(IntPoint p, int valueToSubtract)
        {
            return new IntPoint(p.X - valueToSubtract, p.Y - valueToSubtract);
        }

        public static IntPoint operator *(IntPoint p, int factor)
        {
            return new IntPoint(p.X * factor, p.Y * factor);
        }

        public static IntPoint operator /(IntPoint p, int factor)
        {
            return new IntPoint(p.X / factor, p.Y / factor);
        }

        public static bool operator ==(IntPoint p1, IntPoint p2)
        {
            return ((p1.X == p2.X) && (p1.Y == p2.Y));
        }

        public static bool operator !=(IntPoint p1, IntPoint p2)
        {
            if (p1.X == p2.X)
            {
                return (p1.Y != p2.Y);
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return ((obj is IntPoint) && (this == ((IntPoint) obj)));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator DoublePoint(IntPoint p)
        {
            return new DoublePoint((double) p.X, (double) p.Y);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", this.X, this.Y);
        }

        public double EuclideanNorm()
        {
            return Math.Sqrt((double) ((this.X * this.X) + (this.Y * this.Y)));
        }
    }
}

