namespace AForge.Imaging.ColorReduction
{
    using System;

    public sealed class JarvisJudiceNinkeColorDithering : ColorErrorDiffusionToAdjacentNeighbors
    {
        public JarvisJudiceNinkeColorDithering() : base(new int[][] { new int[] { 7, 5 }, new int[] { 3, 5, 7, 5, 3 }, new int[] { 1, 3, 5, 3, 1 } })
        {
        }
    }
}

