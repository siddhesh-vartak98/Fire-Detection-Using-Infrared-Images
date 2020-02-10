namespace AForge.Imaging.Filters
{
    using System.Collections.Generic;

    public interface IFilterInformation
    {
        Dictionary<PixelFormat, PixelFormat> FormatTranslations { get; }
    }
}

