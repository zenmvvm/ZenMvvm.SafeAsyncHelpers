using System;
using System.Threading;
using System.Threading.Tasks;

//todo refactor HandleException()
// https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/chaining-tasks-by-using-continuation-tasks

namespace ZenMvvm.Helpers
{
    /// <summary>
    /// For unit testing and mocking of <see cref="SafeTaskExtensions"/>
    /// </summary>
    internal class SafeTask : ISafeTask
    {
        /// <summary>
        /// For unit testing and mocking of <see cref="SafeTaskExtensions"/>
        /// </summary>
        public Task InternalHandleExceptionLogic<TException>(Task task, Action<TException> onException, TaskScheduler scheduler = null) where TException : Exception
        {
            task.ContinueWith(
                    t => SafeExecutionHelpers
                        .HandleException<TException>(t.Exception.InnerException, onException)
                    , CancellationToken.None
                    , TaskContinuationOptions.OnlyOnFaulted
                    , scheduler ?? TaskScheduler.Default);

            return task;
        }
    }
}
