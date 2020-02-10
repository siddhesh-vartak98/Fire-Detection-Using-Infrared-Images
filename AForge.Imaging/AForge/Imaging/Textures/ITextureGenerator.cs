namespace AForge.Imaging.Textures
{
    using System;

    public interface ITextureGenerator
    {
        float[,] Generate(int width, int height);
        void Reset();
    }
}

