namespace AForge.Imaging.ColorReduction
{
    using System;

    public sealed class StuckiColorDithering : ColorErrorDiffusionToAdjacentNeighbors
    {
        public StuckiColorDithering() : base(new int[][] { new int[] { 8, 4 }, new int[] { 2, 4, 8, 4, 2 }, new int[] { 1, 2, 4, 2, 1 } })
        {
        }
    }
}

