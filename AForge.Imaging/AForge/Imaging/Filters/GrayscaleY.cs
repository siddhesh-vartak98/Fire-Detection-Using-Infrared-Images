namespace AForge.Imaging.Filters
{
    using System;

    [Obsolete("Use Grayscale.CommonAlgorithms.Y object instead")]
    public sealed class GrayscaleY : Grayscale
    {
        public GrayscaleY() : base(0.299, 0.587, 0.114)
        {
        }
    }
}

