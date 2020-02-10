namespace camera_connection
{
    using System;

    public class ConnectionLostException : Exception
    {
        public ConnectionLostException(string message) : base(message)
        {
        }
    }
}

