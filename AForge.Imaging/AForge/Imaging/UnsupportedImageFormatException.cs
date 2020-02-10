namespace AForge.Imaging
{
    using System;

    public class UnsupportedImageFormatException : ArgumentException
    {
        public UnsupportedImageFormatException()
        {
        }

        public UnsupportedImageFormatException(string message) : base(message)
        {
        }

        public UnsupportedImageFormatException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

