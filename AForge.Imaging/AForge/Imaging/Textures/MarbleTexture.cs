namespace AForge.Imaging.Textures
{
    using AForge.Math;
    using System;

    public class MarbleTexture : ITextureGenerator
    {
        private PerlinNoise noise;
        private int r;
        private Random rand;
        private double xPeriod;
        private double yPeriod;

        public MarbleTexture()
        {
            this.noise = new PerlinNoise(2, 0.65, 0.03125, 1.0);
            this.rand = new Random();
            this.xPeriod = 5.0;
            this.yPeriod = 10.0;
            this.Reset();
        }

        public MarbleTexture(double xPeriod, double yPeriod)
        {
            this.noise = new PerlinNoise(2, 0.65, 0.03125, 1.0);
            this.rand = new Random();
            this.xPeriod = 5.0;
            this.yPeriod = 10.0;
            this.xPeriod = xPeriod;
            this.yPeriod = yPeriod;
            this.Reset();
        }

        public float[,] Generate(int width, int height)
        {
            float[,] numArray = new float[height, width];
            double num = this.xPeriod / ((double) width);
            double num2 = this.yPeriod / ((double) height);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    numArray[i, j] = Math.Min(1f, (float) Math.Abs(Math.Sin((((j * num) + (i * num2)) + this.noise.Function2D((double) (j + this.r), (double) (i + this.r))) * 3.1415926535897931)));
                }
            }
            return numArray;
        }

        public void Reset()
        {
            this.r = this.rand.Next(0x1388);
        }

        public double XPeriod
        {
            get
            {
                return this.xPeriod;
            }
            set
            {
                this.xPeriod = Math.Max(2.0, value);
            }
        }

        public double YPeriod
        {
            get
            {
                return this.yPeriod;
            }
            set
            {
                this.yPeriod = Math.Max(2.0, value);
            }
        }
    }
}

