namespace AForge.Imaging.Filters
{
    using System;

    public sealed class FloydSteinbergDithering : ErrorDiffusionToAdjacentNeighbors
    {
        public FloydSteinbergDithering() : base(new int[][] { new int[] { 7 }, new int[] { 3, 5, 1 } })
        {
        }
    }
}

