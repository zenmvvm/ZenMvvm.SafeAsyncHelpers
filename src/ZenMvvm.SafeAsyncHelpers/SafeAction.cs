using System;
using System.ComponentModel;

namespace ZenMvvm.Helpers
{
    /// <summary>
    /// For unit testing and mocking of <see cref="SafeActionExtensions"/>
    /// </summary>
    internal class SafeAction : ISafeAction
    {
        /// <summary>
        /// For unit testing and mocking of <see cref="SafeActionExtensions"/>
        /// </summary>
        public void InternalSafeInvoke<TException>(
            Action<object> action,
            object parameter,
            Action<TException> onException)
            where TException : Exception
        {
            try
            {
                action(parameter);
            }
            catch (TException ex)
            {
                SafeExecutionHelpers.HandleException(ex, onException);
            }
        }

        /// <summary>
        /// For unit testing and mocking of <see cref="SafeActionExtensions"/>
        /// </summary>
        public void InternalSafeInvoke<TException>(Action action, Action<TException> onException)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException ex) when (SafeExecutionHelpers.Settings.DefaultExceptionHandler != null || onException != null)
            {
                SafeExecutionHelpers.HandleException(ex, onException);
            }
        }
    }
}
