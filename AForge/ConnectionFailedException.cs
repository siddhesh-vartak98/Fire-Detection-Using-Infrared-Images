namespace AForge
{
    using System;

    public class ConnectionFailedException : Exception
    {
        public ConnectionFailedException(string message) : base(message)
        {
        }
    }
}

