namespace AForge.Video
{
    using System;

    public class VideoSourceErrorEventArgs : EventArgs
    {
        private string description;

        public VideoSourceErrorEventArgs(string description)
        {
            this.description = description;
        }

        public string Description
        {
            get
            {
                return this.description;
            }
        }
    }
}

