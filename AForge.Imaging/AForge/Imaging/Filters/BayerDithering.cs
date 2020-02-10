namespace AForge.Imaging.Filters
{
    using System;

    public sealed class BayerDithering : OrderedDithering
    {
        public BayerDithering() : base(new byte[,] { { 0, 0xc0, 0x30, 240 }, { 0x80, 0x40, 0xb0, 0x70 }, { 0x20, 0xe0, 0x10, 0xd0 }, { 160, 0x60, 0x90, 80 } })
        {
        }
    }
}

