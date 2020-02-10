namespace AForge.Imaging.ColorReduction
{
    using System;

    public class ColorErrorDiffusionToAdjacentNeighbors : ErrorDiffusionColorDithering
    {
        private int[][] coefficients;
        private int coefficientsSum;

        public ColorErrorDiffusionToAdjacentNeighbors(int[][] coefficients)
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

        protected override unsafe void Diffuse(int rError, int gError, int bError, byte* ptr)
        {
            int num;
            int num2;
            int num3;
            int[] numArray = this.coefficients[0];
            int num4 = 1;
            int pixelSize = base.pixelSize;
            int index = 0;
            int length = numArray.Length;
            while (index < length)
            {
                if ((base.x + num4) >= base.width)
                {
                    break;
                }
                num = ptr[pixelSize + 2] + ((rError * numArray[index]) / this.coefficientsSum);
                num = (num < 0) ? 0 : ((num > 0xff) ? 0xff : num);
                ptr[pixelSize + 2] = (byte) num;
                num2 = ptr[pixelSize + 1] + ((gError * numArray[index]) / this.coefficientsSum);
                num2 = (num2 < 0) ? 0 : ((num2 > 0xff) ? 0xff : num2);
                ptr[pixelSize + 1] = (byte) num2;
                num3 = ptr[pixelSize] + ((bError * numArray[index]) / this.coefficientsSum);
                num3 = (num3 < 0) ? 0 : ((num3 > 0xff) ? 0xff : num3);
                ptr[pixelSize] = (byte) num3;
                num4++;
                index++;
                pixelSize += base.pixelSize;
            }
            int num8 = 1;
            int num9 = this.coefficients.Length;
            while (num8 < num9)
            {
                if ((base.y + num8) >= base.height)
                {
                    return;
                }
                ptr += base.stride;
                numArray = this.coefficients[num8];
                int num10 = 0;
                int num11 = numArray.Length;
                int num12 = -(num11 >> 1);
                for (int i = -(num11 >> 1) * base.pixelSize; num10 < num11; i += base.pixelSize)
                {
                    if ((base.x + num12) >= base.width)
                    {
                        break;
                    }
                    if ((base.x + num12) >= 0)
                    {
                        num = ptr[i + 2] + ((rError * numArray[num10]) / this.coefficientsSum);
                        num = (num < 0) ? 0 : ((num > 0xff) ? 0xff : num);
                        ptr[i + 2] = (byte) num;
                        num2 = ptr[i + 1] + ((gError * numArray[num10]) / this.coefficientsSum);
                        num2 = (num2 < 0) ? 0 : ((num2 > 0xff) ? 0xff : num2);
                        ptr[i + 1] = (byte) num2;
                        num3 = ptr[i] + ((bError * numArray[num10]) / this.coefficientsSum);
                        num3 = (num3 < 0) ? 0 : ((num3 > 0xff) ? 0xff : num3);
                        ptr[i] = (byte) num3;
                    }
                    num12++;
                    num10++;
                }
                num8++;
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

