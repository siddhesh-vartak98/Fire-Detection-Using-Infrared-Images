namespace AForge
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public sealed class Parallel
    {
        private int currentIndex;
        private static volatile Parallel instance = null;
        private AutoResetEvent[] jobAvailable;
        private ForLoopBody loopBody;
        private int stopIndex;
        private static object sync = new object();
        private ManualResetEvent[] threadIdle;
        private Thread[] threads;
        private static int threadsCount = Environment.ProcessorCount;

        private Parallel()
        {
        }

        public static void For(int start, int stop, ForLoopBody loopBody)
        {
            lock (sync)
            {
                Parallel instance = Instance;
                instance.currentIndex = start - 1;
                instance.stopIndex = stop;
                instance.loopBody = loopBody;
                for (int i = 0; i < threadsCount; i++)
                {
                    instance.threadIdle[i].Reset();
                    instance.jobAvailable[i].Set();
                }
                for (int j = 0; j < threadsCount; j++)
                {
                    instance.threadIdle[j].WaitOne();
                }
            }
        }

        private void Initialize()
        {
            this.jobAvailable = new AutoResetEvent[threadsCount];
            this.threadIdle = new ManualResetEvent[threadsCount];
            this.threads = new Thread[threadsCount];
            for (int i = 0; i < threadsCount; i++)
            {
                this.jobAvailable[i] = new AutoResetEvent(false);
                this.threadIdle[i] = new ManualResetEvent(true);
                this.threads[i] = new Thread(new ParameterizedThreadStart(this.WorkerThread));
                this.threads[i].Name = "AForge.Parallel";
                this.threads[i].IsBackground = true;
                this.threads[i].Start(i);
            }
        }

        private void Terminate()
        {
            this.loopBody = null;
            int index = 0;
            int length = this.threads.Length;
            while (index < length)
            {
                this.jobAvailable[index].Set();
                this.threads[index].Join();
                this.jobAvailable[index].Close();
                this.threadIdle[index].Close();
                index++;
            }
            this.jobAvailable = null;
            this.threadIdle = null;
            this.threads = null;
        }

        private void WorkerThread(object index)
        {
            int num = (int) index;
            int num2 = 0;
        Label_0009:
            this.jobAvailable[num].WaitOne();
            if (this.loopBody == null)
            {
                return;
            }
        Label_0020:
            num2 = Interlocked.Increment(ref this.currentIndex);
            if (num2 < this.stopIndex)
            {
                this.loopBody(num2);
                goto Label_0020;
            }
            this.threadIdle[num].Set();
            goto Label_0009;
        }

        private static Parallel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Parallel();
                    instance.Initialize();
                }
                else if (instance.threads.Length != threadsCount)
                {
                    instance.Terminate();
                    instance.Initialize();
                }
                return instance;
            }
        }

        public static int ThreadsCount
        {
            get
            {
                return threadsCount;
            }
            set
            {
                lock (sync)
                {
                    threadsCount = Math.Max(1, value);
                }
            }
        }

        public delegate void ForLoopBody(int index);
    }
}

