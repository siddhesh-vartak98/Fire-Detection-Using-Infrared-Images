namespace AForge.Imaging.Textures
{
    using AForge.Math;
    using System;

    public class WoodTexture : ITextureGenerator
    {
        private PerlinNoise noise;
        private int r;
        private Random rand;
        private double rings;

        public WoodTexture()
        {
            this.noise = new PerlinNoise(8, 0.5, 0.03125, 0.05);
            this.rand = new Random();
            this.rings = 12.0;
            this.Reset();
        }

        public WoodTexture(double rings)
        {
            this.noise = new PerlinNoise(8, 0.5, 0.03125, 0.05);
            this.rand = new Random();
            this.rings = 12.0;
            this.rings = rings;
            this.Reset();
        }

        public float[,] Generate(int width, int height)
        {
            float[,] numArray = new float[height, width];
            int num = width / 2;
            int num2 = height / 2;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double num5 = ((double) (j - num)) / ((double) width);
                    double num6 = ((double) (i - num2)) / ((double) height);
                    numArray[i, j] = Math.Min(1f, (float) Math.Abs(Math.Sin((((Math.Sqrt((num5 * num5) + (num6 * num6)) + this.noise.Function2D((double) (j + this.r), (double) (i + this.r))) * 3.1415926535897931) * 2.0) * this.rings)));
                }
            }
            return numArray;
        }

        public void Reset()
        {
            this.r = this.rand.Next(0x1388);
        }

        public double Rings
        {
            get
            {
                return this.rings;
            }
            set
            {
                this.rings = Math.Max(3.0, value);
            }
        }
    }
}

