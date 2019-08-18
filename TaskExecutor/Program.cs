using System;
using System.Threading.Tasks;

namespace TaskExecutor
{
    class Program
    {
        static void Main(string[] args)
        {
            var taskExecutor = new TaskExecutor();
            var taskExecutorSlim = new TaskExecutorSlim();

            ExecuteCounter(taskExecutor);

            Console.ReadLine();

            ExecuteCounter(taskExecutorSlim);

            Console.ReadLine();
        }

        private static void ExecuteCounter(ITaskExecutor taskExecutor, int iterationsCount = 100)
        {
            var executionCounter = 0;

            Parallel.For(0, iterationsCount, i =>
                {
                    taskExecutor.AddForExecution(() =>
                    {
                        executionCounter++;
                        Console.WriteLine(executionCounter);
                    });
                }
            );
        }
    }
}
