﻿using System;
using System.Threading;
using NLog;

namespace Codestellation.DarkFlow.Misc
{
    public abstract class Disposable : IDisposable
    {
        protected volatile bool _disposed;
        private int _disposeCount;
        protected readonly Logger Logger;

        public Disposable()
        {
            Logger = LogManager.GetLogger(GetType().FullName);
        }

        public bool Disposed
        {
            get { return _disposed; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Disposable()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            
            _disposed = true;
            if (Interlocked.Increment(ref _disposeCount) > 1) return;


            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Dispose started.");
            }

            if (disposing)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Dispose managed resources started.");
                }

                DisposeManaged();

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Dispose managed resources finished.");
                }
            }

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Dispose unmanaged resources started.");
            }

            ReleaseUnmanaged();

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Dispose unmanaged resources finished.");
            }
        }

        protected abstract void DisposeManaged();

        protected virtual void ReleaseUnmanaged()
        {
            
        }

        protected void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().AssemblyQualifiedName);
            }
        }
    }
}