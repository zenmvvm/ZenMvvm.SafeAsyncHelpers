using System;
using System.Threading.Tasks;

// Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/

namespace ZenMvvm.Helpers
{
    /// <summary>
    /// Extension methods for <see cref="Task"/> enabling the safe execution and handling of errors 
    /// </summary> 
    public static class SafeTaskExtensions
    {
        /// <summary>
        /// Handles exceptions for the given Task with <see cref="SafeExecutionHelpers.DefaultExceptionHandler"/>
        /// </summary>
        /// <param name="task">The Task</param>
        /// <typeparam name="TException">If an exception is thrown of a different type, it will not be handled</typeparam>
        /// <param name="onException">In addition to the <see cref="SafeExecutionHelpers.DefaultExceptionHandler"/>
        /// , <paramref name="onException"/> will execute if an Exception is thrown.</param>
        public static Task HandleException<TException>(this Task task, Action<TException> onException)
            where TException : Exception
            => Implementation.InternalHandleExceptionLogic(task, onException);

        /// <summary>
        /// Handles exceptions for the given Task with <see cref="SafeExecutionHelpers.DefaultExceptionHandler"/>
        /// </summary>
        /// <param name="task">The Task</param>
        /// <param name="onException">In addition to the <see cref="SafeExecutionHelpers.DefaultExceptionHandler"/>
        /// , <paramref name="onException"/> will execute if an Exception is thrown.</param>
        public static Task HandleException(this Task task, Action<Exception> onException)
            => Implementation.InternalHandleExceptionLogic(task, onException);

        /// <summary>
        /// Handles exceptions for the given Task with <see cref="SafeExecutionHelpers.DefaultExceptionHandler"/>
        /// </summary>
        /// <param name="task">The Task</param>
        /// <param name="onException">In addition to the <see cref="SafeExecutionHelpers.DefaultExceptionHandler"/>
        /// , <paramref name="onException"/> will execute if an Exception is thrown.</param>
        public static void FireForgetAndHandleException(this Task task, Action<Exception> onException)
            => Implementation.InternalHandleExceptionLogic(task, onException);

        /// <summary>
        /// Handles exceptions for the given Task with <see cref="SafeExecutionHelpers.DefaultExceptionHandler"/>
        /// </summary>
        /// <param name="task">The Task</param>
        /// <typeparam name="TException">If an exception is thrown of a different type, it will not be handled</typeparam>
        /// <param name="onException">In addition to the <see cref="SafeExecutionHelpers.DefaultExceptionHandler"/>
        /// , <paramref name="onException"/> will execute if an Exception is thrown.</param>
        public static void FireForgetAndHandleException<TException>(this Task task, Action<TException> onException)
            where TException : Exception => Implementation.InternalHandleExceptionLogic(task, onException);


        #region For Unit Testing

        private static readonly ISafeTask defaultImplementation = new SafeTask();

        /// <summary>
        /// For unit testing / mocking
        /// </summary>
        internal static ISafeTask Implementation { private get; set; } = defaultImplementation;

        /// <summary>
        /// For unit testing / mocking
        /// </summary>
        internal static void RevertToDefaultImplementation() => Implementation = defaultImplementation;

        /// <summary>
        /// For unit testing / mocking
        /// </summary>
        internal static Task InternalHandleException(this Task task, Action<Exception> onException, TaskScheduler scheduler = null)
            => Implementation.InternalHandleExceptionLogic(task, onException, scheduler);


        #endregion
    }
}