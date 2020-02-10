namespace AForge.Imaging.Filters
{
    using System;

    public sealed class Edges : Convolution
    {
        public Edges() : base(new int[,] { { 0, -1, 0 }, { -1, 4, -1 }, { 0, -1, 0 } })
        {
        }
    }
}

