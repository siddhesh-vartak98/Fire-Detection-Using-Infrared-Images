namespace AForge.Imaging.Filters
{
    using System;

    public sealed class BurkesDithering : ErrorDiffusionToAdjacentNeighbors
    {
        public BurkesDithering() : base(new int[][] { new int[] { 8, 4 }, new int[] { 2, 4, 8, 4, 2 } })
        {
        }
    }
}

