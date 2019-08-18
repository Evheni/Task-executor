using System;

namespace TaskExecutor
{
    public interface ITaskExecutor
    {
        void AddForExecution(Action action);
        void StopExecution();
    }
}