namespace AForge.Imaging.ColorReduction
{
    using System;
    using System.Drawing;

    public interface IColorQuantizer
    {
        void AddColor(Color color);
        void Clear();
        Color[] GetPalette(int colorCount);
    }
}

