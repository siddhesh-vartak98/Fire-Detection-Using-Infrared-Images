namespace AForge.Video
{
    using System;
    using System.Runtime.CompilerServices;

    public interface IVideoSource
    {
        event NewFrameEventHandler NewFrame;

        event PlayingFinishedEventHandler PlayingFinished;

        event VideoSourceErrorEventHandler VideoSourceError;

        void SignalToStop();
        void Start();
        void Stop();
        void WaitForStop();

        int BytesReceived { get; }

        int FramesReceived { get; }

        bool IsRunning { get; }

        string Source { get; }
    }
}

