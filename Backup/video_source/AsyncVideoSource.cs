namespace AForge.Video
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class AsyncVideoSource : IVideoSource
    {
        private int framesProcessed;
        private Thread imageProcessingThread;
        private AutoResetEvent isNewFrameAvailable;
        private ManualResetEvent isProcessingThreadAvailable;
        private Bitmap lastVideoFrame;
        private readonly IVideoSource nestedVideoSource;
        private bool skipFramesIfBusy;

        public event NewFrameEventHandler NewFrame;

        public event PlayingFinishedEventHandler PlayingFinished
        {
            add
            {
                this.nestedVideoSource.PlayingFinished += value;
            }
            remove
            {
                this.nestedVideoSource.PlayingFinished -= value;
            }
        }

        public event VideoSourceErrorEventHandler VideoSourceError
        {
            add
            {
                this.nestedVideoSource.VideoSourceError += value;
            }
            remove
            {
                this.nestedVideoSource.VideoSourceError -= value;
            }
        }

        public AsyncVideoSource(IVideoSource nestedVideoSource)
        {
            this.nestedVideoSource = nestedVideoSource;
        }

        public AsyncVideoSource(IVideoSource nestedVideoSource, bool skipFramesIfBusy)
        {
            this.nestedVideoSource = nestedVideoSource;
            this.skipFramesIfBusy = skipFramesIfBusy;
        }

        private void Free()
        {
            if (this.imageProcessingThread != null)
            {
                this.nestedVideoSource.NewFrame -= new NewFrameEventHandler(this.nestedVideoSource_NewFrame);
                this.lastVideoFrame = null;
                this.isNewFrameAvailable.Set();
                this.imageProcessingThread.Join();
                this.imageProcessingThread = null;
                this.isNewFrameAvailable.Close();
                this.isNewFrameAvailable = null;
                this.isProcessingThreadAvailable.Close();
                this.isProcessingThreadAvailable = null;
            }
        }

        private void imageProcessingThread_Worker()
        {
            while (true)
            {
                this.isNewFrameAvailable.WaitOne();
                if (this.lastVideoFrame == null)
                {
                    return;
                }
                this.isProcessingThreadAvailable.Reset();
                if (this.NewFrame != null)
                {
                    this.NewFrame(this, new NewFrameEventArgs(this.lastVideoFrame));
                }
                this.lastVideoFrame.Dispose();
                this.framesProcessed++;
                this.isProcessingThreadAvailable.Set();
            }
        }

        private void nestedVideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (this.NewFrame != null)
            {
                if (this.skipFramesIfBusy)
                {
                    if (!this.isProcessingThreadAvailable.WaitOne(0, false))
                    {
                        return;
                    }
                }
                else
                {
                    this.isProcessingThreadAvailable.WaitOne();
                }
                this.lastVideoFrame = (Bitmap) eventArgs.Frame.Clone();
                this.isNewFrameAvailable.Set();
            }
        }

        public void SignalToStop()
        {
            this.nestedVideoSource.SignalToStop();
        }

        public void Start()
        {
            if (!this.IsRunning)
            {
                this.framesProcessed = 0;
                this.isNewFrameAvailable = new AutoResetEvent(false);
                this.isProcessingThreadAvailable = new ManualResetEvent(true);
                this.imageProcessingThread = new Thread(new ThreadStart(this.imageProcessingThread_Worker));
                this.imageProcessingThread.Start();
                this.nestedVideoSource.NewFrame += new NewFrameEventHandler(this.nestedVideoSource_NewFrame);
                this.nestedVideoSource.Start();
            }
        }

        public void Stop()
        {
            this.nestedVideoSource.Stop();
            this.Free();
        }

        public void WaitForStop()
        {
            this.nestedVideoSource.WaitForStop();
            this.Free();
        }

        public int BytesReceived
        {
            get
            {
                return this.nestedVideoSource.BytesReceived;
            }
        }

        public int FramesProcessed
        {
            get
            {
                int framesProcessed = this.framesProcessed;
                this.framesProcessed = 0;
                return framesProcessed;
            }
        }

        public int FramesReceived
        {
            get
            {
                return this.nestedVideoSource.FramesReceived;
            }
        }

        public bool IsRunning
        {
            get
            {
                bool isRunning = this.nestedVideoSource.IsRunning;
                if (!isRunning)
                {
                    this.Free();
                }
                return isRunning;
            }
        }

        public IVideoSource NestedVideoSource
        {
            get
            {
                return this.nestedVideoSource;
            }
        }

        public bool SkipFramesIfBusy
        {
            get
            {
                return this.skipFramesIfBusy;
            }
            set
            {
                this.skipFramesIfBusy = value;
            }
        }

        public string Source
        {
            get
            {
                return this.nestedVideoSource.Source;
            }
        }
    }
}

