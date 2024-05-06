using System;
using System.Threading;

namespace nthLink.Wpf.Model
{
    internal class SimpleTimer : IDisposable
    {
        public int Interval
        {
            get { return this.interval; }
            set
            {
                if (this.interval != value)
                {
                    this.interval = value;
                }
            }
        }
        public bool IsRuning { get; private set; }

        private bool isDisposed = false;

        private int interval = 1000;

        private Action? stopCallBack;

        public event Action? Ticks;

        private readonly Timer timer;

        public SimpleTimer()
        {
            this.timer = new Timer(OnTicks);
        }
        private void OnTicks(object? state)
        {
            if (IsRuning)
            {
                if (Ticks != null)
                {
                    Ticks.Invoke();
                }

                this.timer.Change(Interval, Timeout.Infinite);
            }
            else
            {
                if (this.stopCallBack != null)
                {
                    this.stopCallBack.Invoke();
                    this.stopCallBack = null;
                }
            }
        }
        void IDisposable.Dispose()
        {
            lock (this.timer)
            {
                if (!this.isDisposed)
                {
                    Stop();
                    this.timer.Dispose();
                    Ticks = null;
                    this.isDisposed = true;
                    if (this.stopCallBack != null)
                    {
                        this.stopCallBack = null;
                    }
                }
            }
        }
        public void Start()
        {
            lock (this.timer)
            {
                if (this.isDisposed)
                {
                    throw new Exception("timer already dispose.");
                }

                if (IsRuning)
                {
                    return;
                }

                IsRuning = true;

                this.timer.Change(Interval, Timeout.Infinite);
            }
        }

        public void StartWithTick()
        {
            lock (this.timer)
            {
                if (this.isDisposed)
                {
                    throw new Exception("timer already dispose.");
                }

                if (IsRuning)
                {
                    return;
                }

                IsRuning = true;

                this.timer.Change(0, Timeout.Infinite);
            }
        }

        public void Stop()
        {
            lock (this.timer)
            {
                if (!IsRuning)
                {
                    return;
                }

                IsRuning = false;
            }
        }

        public void StopWithContinue(Action action)
        {
            this.stopCallBack = action;
            Stop();
        }
    }
}
