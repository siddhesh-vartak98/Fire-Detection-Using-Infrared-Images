namespace AForge.Imaging.Filters
{
    using AForge.Math;
    using System;

    public class GaussianSharpen : Convolution
    {
        private double sigma;
        private int size;

        public GaussianSharpen()
        {
            this.sigma = 1.4;
            this.size = 5;
            this.CreateFilter();
        }

        public GaussianSharpen(double sigma)
        {
            this.sigma = 1.4;
            this.size = 5;
            this.Sigma = sigma;
        }

        public GaussianSharpen(double sigma, int size)
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
            int num3 = 0;
            for (int i = 0; i < this.size; i++)
            {
                for (int k = 0; k < this.size; k++)
                {
                    double num6 = numArray[i, k] / num;
                    if (num6 > 65535.0)
                    {
                        num6 = 65535.0;
                    }
                    numArray2[i, k] = (int) num6;
                    num2 += numArray2[i, k];
                }
            }
            int num7 = this.size >> 1;
            for (int j = 0; j < this.size; j++)
            {
                for (int m = 0; m < this.size; m++)
                {
                    if ((j == num7) && (m == num7))
                    {
                        numArray2[j, m] = (2 * num2) - numArray2[j, m];
                    }
                    else
                    {
                        numArray2[j, m] = -numArray2[j, m];
                    }
                    num3 += numArray2[j, m];
                }
            }
            base.Kernel = numArray2;
            base.Divisor = num3;
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

