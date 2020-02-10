namespace AForge.Imaging.Filters
{
    using System;

    public sealed class JarvisJudiceNinkeDithering : ErrorDiffusionToAdjacentNeighbors
    {
        public JarvisJudiceNinkeDithering() : base(new int[][] { new int[] { 7, 5 }, new int[] { 3, 5, 7, 5, 3 }, new int[] { 1, 3, 5, 3, 1 } })
        {
        }
    }
}

