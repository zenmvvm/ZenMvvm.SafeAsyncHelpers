using System;
using System.ComponentModel;

namespace ZenMvvm.Helpers
{
    /// <summary>
    /// For unit testing and mocking of <see cref="SafeActionExtensions"/>
    /// </summary>
    internal interface ISafeAction
    {
        /// <summary>
        /// For unit testing and mocking of <see cref="SafeActionExtensions"/>
        /// </summary>
        void InternalSafeInvoke<TException>(Action<object> action, object parameter, Action<TException> onException) where TException : Exception;

        /// <summary>
        /// For unit testing and mocking of <see cref="SafeActionExtensions"/>
        /// </summary>
        void InternalSafeInvoke<TException>(Action action, Action<TException> onException) where TException : Exception;
    }
}