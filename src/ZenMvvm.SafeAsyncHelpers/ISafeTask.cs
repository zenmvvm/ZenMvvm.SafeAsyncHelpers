using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ZenMvvm.Helpers
{
    /// <summary>
    /// For unit testing and mocking of <see cref="SafeTaskExtensions"/>
    /// </summary>
    internal interface ISafeTask
    {
        /// <summary>
        /// For unit testing and mocking of <see cref="SafeTaskExtensions"/>
        /// </summary>
        Task InternalHandleExceptionLogic<TException>(
            Task task, Action<TException> onException,
            TaskScheduler scheduler = null)
            where TException : Exception;
    }
}