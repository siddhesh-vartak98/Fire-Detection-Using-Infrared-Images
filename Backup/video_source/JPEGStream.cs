namespace AForge.Video
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class JPEGStream : IVideoSource
    {
        private const int bufferSize = 0x80000;
        private int bytesReceived;
        private int frameInterval;
        private int framesReceived;
        private string login;
        private string password;
        private bool preventCaching;
        private const int readSize = 0x400;
        private int requestTimeout;
        private string source;
        private ManualResetEvent stopEvent;
        private Thread thread;
        private bool useSeparateConnectionGroup;

        public event NewFrameEventHandler NewFrame;

        public event PlayingFinishedEventHandler PlayingFinished;

        public event VideoSourceErrorEventHandler VideoSourceError;

        public JPEGStream()
        {
            this.preventCaching = true;
            this.requestTimeout = 0x2710;
        }

        public JPEGStream(string source)
        {
            this.preventCaching = true;
            this.requestTimeout = 0x2710;
            this.source = source;
        }

        private void Free()
        {
            this.thread = null;
            this.stopEvent.Close();
            this.stopEvent = null;
        }

        public void SignalToStop()
        {
            if (this.thread != null)
            {
                this.stopEvent.Set();
            }
        }

        public void Start()
        {
            if (!this.IsRunning)
            {
                if ((this.source == null) || (this.source == string.Empty))
                {
                    throw new ArgumentException("Video source is not specified.");
                }
                this.framesReceived = 0;
                this.bytesReceived = 0;
                this.stopEvent = new ManualResetEvent(false);
                this.thread = new Thread(new ThreadStart(this.WorkerThread));
                this.thread.Name = this.source;
                this.thread.Start();
            }
        }

        public void Stop()
        {
            if (this.IsRunning)
            {
                this.stopEvent.Set();
                this.thread.Abort();
                this.WaitForStop();
            }
        }

        public void WaitForStop()
        {
            if (this.thread != null)
            {
                this.thread.Join();
                this.Free();
            }
        }

        private void WorkerThread()
        {
            byte[] buffer = new byte[0x80000];
            HttpWebRequest request = null;
            WebResponse response = null;
            Stream responseStream = null;
            Random random = new Random((int) DateTime.Now.Ticks);
            do
            {
                int offset = 0;
                try
                {
                    DateTime now = DateTime.Now;
                    if (!this.preventCaching)
                    {
                        request = (HttpWebRequest) WebRequest.Create(this.source);
                    }
                    else
                    {
                        request = (HttpWebRequest) WebRequest.Create(string.Concat(new object[] { this.source, (this.source.IndexOf('?') == -1) ? '?' : '&', "fake=", random.Next().ToString() }));
                    }
                    request.Timeout = this.requestTimeout;
                    if (((this.login != null) && (this.password != null)) && (this.login != string.Empty))
                    {
                        request.Credentials = new NetworkCredential(this.login, this.password);
                    }
                    if (this.useSeparateConnectionGroup)
                    {
                        request.ConnectionGroupName = this.GetHashCode().ToString();
                    }
                    response = request.GetResponse();
                    responseStream = response.GetResponseStream();
                    while (!this.stopEvent.WaitOne(0, true))
                    {
                        if (offset > 0x7fc00)
                        {
                            offset = 0;
                        }
                        int num = responseStream.Read(buffer, offset, 0x400);
                        if (num == 0)
                        {
                            break;
                        }
                        offset += num;
                        this.bytesReceived += num;
                    }
                    if (!this.stopEvent.WaitOne(0, true))
                    {
                        this.framesReceived++;
                        if (this.NewFrame != null)
                        {
                            Bitmap frame = (Bitmap) Image.FromStream(new MemoryStream(buffer, 0, offset));
                            this.NewFrame(this, new NewFrameEventArgs(frame));
                            frame.Dispose();
                            frame = null;
                        }
                    }
                    if (this.frameInterval > 0)
                    {
                        TimeSpan span = DateTime.Now.Subtract(now);
                        for (int i = this.frameInterval - ((int) span.TotalMilliseconds); (i > 0) && !this.stopEvent.WaitOne(0, true); i -= 100)
                        {
                            Thread.Sleep((i < 100) ? i : 100);
                        }
                    }
                }
                catch (WebException exception)
                {
                    if (this.VideoSourceError != null)
                    {
                        this.VideoSourceError(this, new VideoSourceErrorEventArgs(exception.Message));
                    }
                    Thread.Sleep(250);
                }
                catch (Exception)
                {
                }
                finally
                {
                    if (request != null)
                    {
                        request.Abort();
                        request = null;
                    }
                    if (responseStream != null)
                    {
                        responseStream.Close();
                        responseStream = null;
                    }
                    if (response != null)
                    {
                        response.Close();
                        response = null;
                    }
                }
            }
            while (!this.stopEvent.WaitOne(0, true));
            if (this.PlayingFinished != null)
            {
                this.PlayingFinished(this, ReasonToFinishPlaying.StoppedByUser);
            }
        }

        public int BytesReceived
        {
            get
            {
                int bytesReceived = this.bytesReceived;
                this.bytesReceived = 0;
                return bytesReceived;
            }
        }

        public int FrameInterval
        {
            get
            {
                return this.frameInterval;
            }
            set
            {
                this.frameInterval = value;
            }
        }

        public int FramesReceived
        {
            get
            {
                int framesReceived = this.framesReceived;
                this.framesReceived = 0;
                return framesReceived;
            }
        }

        public bool IsRunning
        {
            get
            {
                if (this.thread != null)
                {
                    if (!this.thread.Join(0))
                    {
                        return true;
                    }
                    this.Free();
                }
                return false;
            }
        }

        public string Login
        {
            get
            {
                return this.login;
            }
            set
            {
                this.login = value;
            }
        }

        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
            }
        }

        public bool PreventCaching
        {
            get
            {
                return this.preventCaching;
            }
            set
            {
                this.preventCaching = value;
            }
        }

        public int RequestTimeout
        {
            get
            {
                return this.requestTimeout;
            }
            set
            {
                this.requestTimeout = value;
            }
        }

        public bool SeparateConnectionGroup
        {
            get
            {
                return this.useSeparateConnectionGroup;
            }
            set
            {
                this.useSeparateConnectionGroup = value;
            }
        }

        public virtual string Source
        {
            get
            {
                return this.source;
            }
            set
            {
                this.source = value;
            }
        }
    }
}

