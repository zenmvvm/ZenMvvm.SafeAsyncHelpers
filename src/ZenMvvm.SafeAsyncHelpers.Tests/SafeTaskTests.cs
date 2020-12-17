using System;
using System.Threading.Tasks;
using Xunit;
using ZenMvvm.Helpers;
using System.Threading;
using Moq;
using Zeebs.UnitTestHelpers;

namespace ZenMvvm.Tests
{
    [Collection("SafeTests")]
    public class SafeTaskTests : IDisposable
    {
        private readonly Mock<ISafeExecutionHelpers> mockHelpers;
        private readonly SpecificException specificException = new();
        private readonly SafeTask sut = new();
        private readonly DeterministicTaskScheduler dts = new(shouldThrowExceptions: false);

        //Setup
        public SafeTaskTests()
        {
            mockHelpers = new Mock<ISafeExecutionHelpers>();
            SafeExecutionHelpers.Implementation = mockHelpers.Object;
        }

        //Teardown
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
            SafeExecutionHelpers.RevertToDefaultImplementation();
        }

        class SpecificException : Exception { }

        [Fact]
        public void SafeContinueWith_NothingSet_HandleExceptionRuns()
        {
            sut.InternalHandleExceptionLogic<Exception>(
                Task.Factory.StartNew(
                    () => throw specificException,
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    dts),
                onException: null,
                dts);

            dts.RunTasksUntilIdle();

            Assert.Contains(specificException, dts.Exceptions);
            mockHelpers.Verify(h => h.HandleException<Exception>(specificException,null));
            
            SafeExecutionHelpers.RevertToDefaultImplementation();
        }

        [Fact]
        public void SafeContinueWith_OnExceptionExceptionSet_HandlesException()
        {
            Action<Exception> onException = new Mock<Action<Exception>>().Object;

            sut.InternalHandleExceptionLogic<Exception>(
                Task.Factory.StartNew(
                    ()=> throw specificException,
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    dts),
                onException, //The crux
                dts);

            dts.RunTasksUntilIdle();

            Assert.Contains(specificException, dts.Exceptions);
            mockHelpers.Verify(h => h.HandleException(specificException, onException));
        }

        [Fact]
        public void SafeContinueWith_DefaultExceptionHandlerSet_HandlesException()
        {
            Action<Exception> defaultExceptionHandler
                = new Mock<Action<Exception>>().Object;
            
            //Crux - DefaultHandler returns non-null delegate
            mockHelpers.SetupGet(h => h.Settings.DefaultExceptionHandler).Returns(defaultExceptionHandler);

            sut.InternalHandleExceptionLogic<Exception>(
                task: Task.Factory.StartNew(
                    () => throw specificException,
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    dts),
                onException: null,
                scheduler: dts);

            dts.RunTasksUntilIdle();

            Assert.Contains(specificException, dts.Exceptions);
            mockHelpers.Verify(h =>
                h.HandleException<Exception>(specificException, null));
        }
    }
}
