namespace AForge.Imaging.Filters
{
    using System;

    public sealed class Mean : Convolution
    {
        public Mean() : base(new int[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } })
        {
        }
    }
}

