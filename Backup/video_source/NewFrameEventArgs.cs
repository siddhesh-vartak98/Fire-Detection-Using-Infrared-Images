namespace AForge.Video
{
    using System;
    using System.Drawing;

    public class NewFrameEventArgs : EventArgs
    {
        private Bitmap frame;

        public NewFrameEventArgs(Bitmap frame)
        {
            this.frame = frame;
        }

        public Bitmap Frame
        {
            get
            {
                return this.frame;
            }
        }
    }
}

