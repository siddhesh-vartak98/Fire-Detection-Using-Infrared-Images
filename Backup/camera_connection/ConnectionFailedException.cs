namespace camera_connection
{
    using System;

    public class ConnectionFailedException : Exception
    {
        public ConnectionFailedException(string message) : base(message)
        {
        }
    }
}

