using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace TaskExecutor
{
    public class TaskExecutorSlim : ITaskExecutor, IDisposable
    {
        #region Private Fields

        private readonly AutoResetEvent _resetEvent = new AutoResetEvent(true);
        private readonly ConcurrentQueue<Action> _store = new ConcurrentQueue<Action>();
        private readonly CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        #endregion

        #region Constructors

        public TaskExecutorSlim()
        {
            Task.Run(() =>
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    if (_store.TryDequeue(out var action))
                    {
                        action();
                    }
                    else
                    {
                        _resetEvent.WaitOne();
                    }
                }
            });
        }

        #endregion

        #region Public Methods

        public void AddForExecution(Action action)
        {
            _store.Enqueue(action);
            _resetEvent.Set();
        }

        public void StopExecution()
        {
            _cancellationToken.Cancel();
        }

        #endregion

        #region Dispose

        ~TaskExecutorSlim()
        {
            Dispose(false);
        }

        private void ReleaseUnmanagedResources()
        {
            StopExecution();
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                _resetEvent?.Dispose();
                _cancellationToken?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}