using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TaskExecutor
{
    public class TaskExecutor : ITaskExecutor, IDisposable
    {
        #region Private Fields

        private readonly BlockingCollection<Action> _store = new BlockingCollection<Action>();

        #endregion

        #region Constructors

        public TaskExecutor()
        {
            Task.Run(() =>
            {
                foreach (var action in _store.GetConsumingEnumerable())
                {
                    action();
                }
            });
        }

        #endregion

        #region Public Methods

        public void AddForExecution(Action action)
        {
            _store.Add(action);
        }

        public void StopExecution()
        {
            _store.CompleteAdding();
        }

        #endregion

        #region Dispose

        ~TaskExecutor()
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
                _store?.Dispose();
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