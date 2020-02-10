namespace AForge
{
    using System;

    public class IntRange
    {
        private int max;
        private int min;

        public IntRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public bool IsInside(IntRange range)
        {
            return (this.IsInside(range.min) && this.IsInside(range.max));
        }

        public bool IsInside(int x)
        {
            return ((x >= this.min) && (x <= this.max));
        }

        public bool IsOverlapping(IntRange range)
        {
            if ((!this.IsInside(range.min) && !this.IsInside(range.max)) && !range.IsInside(this.min))
            {
                return range.IsInside(this.max);
            }
            return true;
        }

        public int Length
        {
            get
            {
                return (this.max - this.min);
            }
        }

        public int Max
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

        public int Min
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

