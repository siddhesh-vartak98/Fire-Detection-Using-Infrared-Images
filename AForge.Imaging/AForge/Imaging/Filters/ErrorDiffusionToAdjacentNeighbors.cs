namespace AForge.Imaging.Filters
{
    using System;

    public class ErrorDiffusionToAdjacentNeighbors : ErrorDiffusionDithering
    {
        private int[][] coefficients;
        private int coefficientsSum;

        public ErrorDiffusionToAdjacentNeighbors(int[][] coefficients)
        {
            this.coefficients = coefficients;
            this.CalculateCoefficientsSum();
        }

        private void CalculateCoefficientsSum()
        {
            this.coefficientsSum = 0;
            int index = 0;
            int length = this.coefficients.Length;
            while (index < length)
            {
                int[] numArray = this.coefficients[index];
                int num3 = 0;
                int num4 = numArray.Length;
                while (num3 < num4)
                {
                    this.coefficientsSum += numArray[num3];
                    num3++;
                }
                index++;
            }
        }

        protected override unsafe void Diffuse(int error, byte* ptr)
        {
            int num;
            int[] numArray = this.coefficients[0];
            int index = 1;
            int num3 = 0;
            int length = numArray.Length;
            while (num3 < length)
            {
                if ((base.x + index) >= base.stopX)
                {
                    break;
                }
                num = ptr[index] + ((error * numArray[num3]) / this.coefficientsSum);
                num = (num < 0) ? 0 : ((num > 0xff) ? 0xff : num);
                ptr[index] = (byte) num;
                index++;
                num3++;
            }
            int num5 = 1;
            int num6 = this.coefficients.Length;
            while (num5 < num6)
            {
                if ((base.y + num5) >= base.stopY)
                {
                    return;
                }
                ptr += base.stride;
                numArray = this.coefficients[num5];
                int num7 = 0;
                int num8 = numArray.Length;
                int num9 = -(num8 >> 1);
                while (num7 < num8)
                {
                    if ((base.x + num9) >= base.stopX)
                    {
                        break;
                    }
                    if ((base.x + num9) >= base.startX)
                    {
                        num = ptr[num9] + ((error * numArray[num7]) / this.coefficientsSum);
                        num = (num < 0) ? 0 : ((num > 0xff) ? 0xff : num);
                        ptr[num9] = (byte) num;
                    }
                    num9++;
                    num7++;
                }
                num5++;
            }
        }

        public int[][] Coefficients
        {
            get
            {
                return this.coefficients;
            }
            set
            {
                this.coefficients = value;
                this.CalculateCoefficientsSum();
            }
        }
    }
}

