namespace AForge.Video
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    public class MJPEGStream : IVideoSource
    {
        private const int bufSize = 0x80000;
        private int bytesReceived;
        private int framesReceived;
        private string login;
        private string password;
        private const int readSize = 0x400;
        private ManualResetEvent reloadEvent;
        private int requestTimeout;
        private string source;
        private ManualResetEvent stopEvent;
        private Thread thread;
        private string userAgent;
        private bool useSeparateConnectionGroup;

        public event NewFrameEventHandler NewFrame;

        public event PlayingFinishedEventHandler PlayingFinished;

        public event VideoSourceErrorEventHandler VideoSourceError;

        public MJPEGStream()
        {
            this.useSeparateConnectionGroup = true;
            this.requestTimeout = 0x2710;
            this.userAgent = "Mozilla/5.0";
        }

        public MJPEGStream(string source)
        {
            this.useSeparateConnectionGroup = true;
            this.requestTimeout = 0x2710;
            this.userAgent = "Mozilla/5.0";
            this.source = source;
        }

        private void Free()
        {
            this.thread = null;
            this.stopEvent.Close();
            this.stopEvent = null;
            this.reloadEvent.Close();
            this.reloadEvent = null;
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
                this.reloadEvent = new ManualResetEvent(false);
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
            byte[] needle = new byte[] { 0xff, 0xd8, 0xff };
            int num = 3;
            do
            {
                this.reloadEvent.Reset();
                HttpWebRequest request = null;
                WebResponse response = null;
                Stream responseStream = null;
                byte[] bytes = null;
                int sourceLength = 0;
                int offset = 0;
                int startIndex = 0;
                int num7 = 1;
                int index = 0;
                int num9 = 0;
                try
                {
                    request = (HttpWebRequest) WebRequest.Create(this.source);
                    if (this.userAgent != null)
                    {
                        request.UserAgent = this.userAgent;
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
                    string contentType = response.ContentType;
                    if ((contentType.Split(new char[] { '/' })[0] != "multipart") || !contentType.Contains("mixed"))
                    {
                        if (this.VideoSourceError != null)
                        {
                            this.VideoSourceError(this, new VideoSourceErrorEventArgs("Invalid content type"));
                        }
                        throw new ApplicationException();
                    }
                    int num10 = contentType.IndexOf("boundary", 0);
                    if (num10 != -1)
                    {
                        num10 = contentType.IndexOf("=", (int) (num10 + 8));
                    }
                    if (num10 == -1)
                    {
                        if (this.VideoSourceError != null)
                        {
                            this.VideoSourceError(this, new VideoSourceErrorEventArgs("Invalid content type"));
                        }
                        throw new ApplicationException();
                    }
                    string s = contentType.Substring(num10 + 1).Trim(new char[] { ' ', '"' });
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    bytes = encoding.GetBytes(s);
                    int length = bytes.Length;
                    bool flag = false;
                    responseStream = response.GetResponseStream();
                    while (!this.stopEvent.WaitOne(0, true) && !this.reloadEvent.WaitOne(0, true))
                    {
                        if (offset > 0x7fc00)
                        {
                            offset = startIndex = sourceLength = 0;
                        }
                        int num3 = responseStream.Read(buffer, offset, 0x400);
                        if (num3 == 0)
                        {
                            throw new ApplicationException();
                        }
                        offset += num3;
                        sourceLength += num3;
                        this.bytesReceived += num3;
                        if (flag)
                        {
                            goto Label_028A;
                        }
                        startIndex = ByteArrayUtils.Find(buffer, bytes, 0, sourceLength);
                        if (startIndex == -1)
                        {
                            continue;
                        }
                        for (int i = startIndex - 1; i >= 0; i--)
                        {
                            byte num12 = buffer[i];
                            switch (num12)
                            {
                                case 10:
                                case 13:
                                    goto Label_0276;
                            }
                            s = ((char) num12) + s;
                        }
                    Label_0276:
                        bytes = encoding.GetBytes(s);
                        length = bytes.Length;
                        flag = true;
                    Label_028A:
                        if (num7 == 1)
                        {
                            index = ByteArrayUtils.Find(buffer, needle, startIndex, sourceLength);
                            if (index != -1)
                            {
                                startIndex = index;
                                sourceLength = offset - startIndex;
                                num7 = 2;
                            }
                            else
                            {
                                sourceLength = num - 1;
                                startIndex = offset - sourceLength;
                            }
                        }
                        while ((num7 == 2) && (sourceLength >= length))
                        {
                            num9 = ByteArrayUtils.Find(buffer, bytes, startIndex, sourceLength);
                            if (num9 != -1)
                            {
                                startIndex = num9;
                                sourceLength = offset - startIndex;
                                this.framesReceived++;
                                if ((this.NewFrame != null) && !this.stopEvent.WaitOne(0, true))
                                {
                                    Bitmap frame = (Bitmap) Image.FromStream(new MemoryStream(buffer, index, num9 - index));
                                    this.NewFrame(this, new NewFrameEventArgs(frame));
                                    frame.Dispose();
                                    frame = null;
                                }
                                startIndex = num9 + length;
                                sourceLength = offset - startIndex;
                                Array.Copy(buffer, startIndex, buffer, 0, sourceLength);
                                offset = sourceLength;
                                startIndex = 0;
                                num7 = 1;
                            }
                            else
                            {
                                sourceLength = length - 1;
                                startIndex = offset - sourceLength;
                            }
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
                catch (ApplicationException)
                {
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

        public int FramesReceived
        {
            get
            {
                int framesReceived = this.framesReceived;
                this.framesReceived = 0;
                return framesReceived;
            }
        }

        public string HttpUserAgent
        {
            get
            {
                return this.userAgent;
            }
            set
            {
                this.userAgent = value;
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

        public string Source
        {
            get
            {
                return this.source;
            }
            set
            {
                this.source = value;
                if (this.thread != null)
                {
                    this.reloadEvent.Set();
                }
            }
        }
    }
}

