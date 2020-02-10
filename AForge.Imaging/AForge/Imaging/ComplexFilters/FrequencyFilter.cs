namespace AForge.Imaging.ComplexFilters
{
    using AForge;
    using AForge.Imaging;
    using AForge.Math;
    using System;

    public class FrequencyFilter : IComplexFilter
    {
        private IntRange frequencyRange;

        public FrequencyFilter()
        {
            this.frequencyRange = new IntRange(0, 0x400);
        }

        public FrequencyFilter(IntRange frequencyRange)
        {
            this.frequencyRange = new IntRange(0, 0x400);
            this.frequencyRange = frequencyRange;
        }

        public void Apply(ComplexImage complexImage)
        {
            if (!complexImage.FourierTransformed)
            {
                throw new ArgumentException("The source complex image should be Fourier transformed.");
            }
            int width = complexImage.Width;
            int height = complexImage.Height;
            int num3 = width >> 1;
            int num4 = height >> 1;
            int min = this.frequencyRange.Min;
            int max = this.frequencyRange.Max;
            Complex[,] data = complexImage.Data;
            for (int i = 0; i < height; i++)
            {
                int num8 = i - num4;
                for (int j = 0; j < width; j++)
                {
                    int num10 = j - num3;
                    int num11 = (int) Math.Sqrt((double) ((num10 * num10) + (num8 * num8)));
                    if ((num11 > max) || (num11 < min))
                    {
                        data[i, j].Re = 0.0;
                        data[i, j].Im = 0.0;
                    }
                }
            }
        }

        public IntRange FrequencyRange
        {
            get
            {
                return this.frequencyRange;
            }
            set
            {
                this.frequencyRange = value;
            }
        }
    }
}

