namespace AForge.Imaging.Filters
{
    using System;

    [Obsolete("Use Grayscale.CommonAlgorithms.BT709 object instead")]
    public sealed class GrayscaleBT709 : Grayscale
    {
        public GrayscaleBT709() : base(0.2125, 0.7154, 0.0721)
        {
        }
    }
}

