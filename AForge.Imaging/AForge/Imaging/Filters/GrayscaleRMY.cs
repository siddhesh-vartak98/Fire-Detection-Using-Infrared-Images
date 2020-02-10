namespace AForge.Imaging.Filters
{
    using System;

    [Obsolete("Use Grayscale.CommonAlgorithms.RMY object instead")]
    public sealed class GrayscaleRMY : Grayscale
    {
        public GrayscaleRMY() : base(0.5, 0.419, 0.081)
        {
        }
    }
}

