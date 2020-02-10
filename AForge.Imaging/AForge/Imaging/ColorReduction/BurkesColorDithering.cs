namespace AForge.Imaging.ColorReduction
{
    using System;

    public sealed class BurkesColorDithering : ColorErrorDiffusionToAdjacentNeighbors
    {
        public BurkesColorDithering() : base(new int[][] { new int[] { 8, 4 }, new int[] { 2, 4, 8, 4, 2 } })
        {
        }
    }
}

