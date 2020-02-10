namespace AForge.Imaging.Textures
{
    using AForge.Math;
    using System;

    public class LabyrinthTexture : ITextureGenerator
    {
        private PerlinNoise noise = new PerlinNoise(1, 0.65, 0.0625, 1.0);
        private int r;
        private Random rand = new Random();

        public LabyrinthTexture()
        {
            this.Reset();
        }

        public float[,] Generate(int width, int height)
        {
            float[,] numArray = new float[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    numArray[i, j] = Math.Min(1f, (float) Math.Abs(this.noise.Function2D((double) (j + this.r), (double) (i + this.r))));
                }
            }
            return numArray;
        }

        public void Reset()
        {
            this.r = this.rand.Next(0x1388);
        }
    }
}

