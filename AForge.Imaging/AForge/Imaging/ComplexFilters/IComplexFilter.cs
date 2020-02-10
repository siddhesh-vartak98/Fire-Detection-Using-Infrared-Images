namespace AForge.Imaging.ComplexFilters
{
    using AForge.Imaging;
    using System;

    public interface IComplexFilter
    {
        void Apply(ComplexImage complexImage);
    }
}

