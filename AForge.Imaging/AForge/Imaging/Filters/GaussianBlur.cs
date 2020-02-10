namespace AForge.Imaging.Filters
{
    using AForge.Math;
    using System;

    public sealed class GaussianBlur : Convolution
    {
        private double sigma;
        private int size;

        public GaussianBlur()
        {
            this.sigma = 1.4;
            this.size = 5;
            this.CreateFilter();
        }

        public GaussianBlur(double sigma)
        {
            this.sigma = 1.4;
            this.size = 5;
            this.Sigma = sigma;
        }

        public GaussianBlur(double sigma, int size)
        {
            this.sigma = 1.4;
            this.size = 5;
            this.Sigma = sigma;
            this.Size = size;
        }

        private void CreateFilter()
        {
            double[,] numArray = new Gaussian(this.sigma).Kernel2D(this.size);
            double num = numArray[0, 0];
            int[,] numArray2 = new int[this.size, this.size];
            int num2 = 0;
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    double num5 = numArray[i, j] / num;
                    if (num5 > 65535.0)
                    {
                        num5 = 65535.0;
                    }
                    numArray2[i, j] = (int) num5;
                    num2 += numArray2[i, j];
                }
            }
            base.Kernel = numArray2;
            base.Divisor = num2;
        }

        public double Sigma
        {
            get
            {
                return this.sigma;
            }
            set
            {
                this.sigma = Math.Max(0.5, Math.Min(5.0, value));
                this.CreateFilter();
            }
        }

        public int Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = Math.Max(3, Math.Min(0x15, value | 1));
                this.CreateFilter();
            }
        }
    }
}

