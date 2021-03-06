﻿namespace AForge.Imaging.Textures
{
    using AForge.Math;
    using System;

    public class CloudsTexture : ITextureGenerator
    {
        private PerlinNoise noise = new PerlinNoise(8, 0.5, 0.03125, 1.0);
        private int r;
        private Random rand = new Random();

        public CloudsTexture()
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
                    numArray[i, j] = Math.Max(0f, Math.Min((float) 1f, (float) ((((float) this.noise.Function2D((double) (j + this.r), (double) (i + this.r))) * 0.5f) + 0.5f)));
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

