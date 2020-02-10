namespace AForge.Imaging.ColorReduction
{
    using System;

    public sealed class FloydSteinbergColorDithering : ColorErrorDiffusionToAdjacentNeighbors
    {
        public FloydSteinbergColorDithering() : base(new int[][] { new int[] { 7 }, new int[] { 3, 5, 1 } })
        {
        }
    }
}

