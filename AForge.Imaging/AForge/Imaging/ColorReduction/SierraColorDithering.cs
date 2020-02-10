namespace AForge.Imaging.ColorReduction
{
    using System;

    public sealed class SierraColorDithering : ColorErrorDiffusionToAdjacentNeighbors
    {
        public SierraColorDithering() : base(new int[][] { new int[] { 5, 3 }, new int[] { 2, 4, 5, 4, 2 }, new int[] { 2, 3, 2 } })
        {
        }
    }
}

