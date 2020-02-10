namespace AForge
{
    using System;

    public class DoubleRange
    {
        private double max;
        private double min;

        public DoubleRange(double min, double max)
        {
            this.min = min;
            this.max = max;
        }

        public bool IsInside(DoubleRange range)
        {
            return (this.IsInside(range.min) && this.IsInside(range.max));
        }

        public bool IsInside(double x)
        {
            return ((x >= this.min) && (x <= this.max));
        }

        public bool IsOverlapping(DoubleRange range)
        {
            if ((!this.IsInside(range.min) && !this.IsInside(range.max)) && !range.IsInside(this.min))
            {
                return range.IsInside(this.max);
            }
            return true;
        }

        public double Length
        {
            get
            {
                return (this.max - this.min);
            }
        }

        public double Max
        {
            get
            {
                return this.max;
            }
            set
            {
                this.max = value;
            }
        }

        public double Min
        {
            get
            {
                return this.min;
            }
            set
            {
                this.min = value;
            }
        }
    }
}

