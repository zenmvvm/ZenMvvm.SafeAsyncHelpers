using System;
using Xunit;
using ZenMvvm.Helpers;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using Moq;

namespace ZenMvvm.Tests
{
    public class TestOverloads
    {
        public TestOverloads(Action action) {}
        public TestOverloads(Action action, IBusy viewModel) : this(action) { }
        public TestOverloads(Action action, IBusy viewModel, Action<Exception> onException) : this(action,viewModel) { }

        public TestOverloads(Func<Task> func) { }
        public TestOverloads(Func<Task> func, IBusy viewModel) : this(func) { }
        public TestOverloads(Func<Task> func, IBusy viewModel, Action<Exception> onException) : this(func, viewModel) { }

    }

    [Collection("SafeTests")]
    public class SafeCommandTests_IsBusy : IDisposable
    {
        private Mock<IBusy> iBusyMock;
        //Setup
        public SafeCommandTests_IsBusy()
        {
            iBusyMock = new Mock<IBusy>();
        }
        //Teardown
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
            //
        }

        const int DELAY = 50;
        [Fact]
        public void Execute_WithIBusy_IsBusyTrueWhileRunning()
        {

            var command = new SafeCommand(
                executeAction: () =>
                {
                    //State is true while executing
                    iBusyMock.VerifySet(o => o.IsBusy = true);
                    iBusyMock.Invocations.Clear();
                }
                , iBusyMock.Object);

            //Initial state is false
            Assert.False(iBusyMock.Object.IsBusy);

            //Act
            command.Execute(null);

            //State is false after execution
            iBusyMock.VerifySet(o => o.IsBusy = false);
            //Only invoked once after set to true
            Assert.Equal(1, iBusyMock.Invocations.Count);
        }

        [Fact]
        public void ExecuteAsync_WithIBusy_IsBusyTrueWhileRunning()
        {
            bool isExecuting = true;

            var command = new SafeCommand(async () =>
            {
                //State is true when start executing
                iBusyMock.VerifySet(o => o.IsBusy = true);
                await Task.Delay(DELAY);
                //State remains true throughout execution
                iBusyMock.VerifySet(o => o.IsBusy = true);
                iBusyMock.Invocations.Clear();
                isExecuting = false;
            }, iBusyMock.Object);

            var thread = new Thread(new ThreadStart(() => command.Execute(null)));

            //Initial state is false
            Assert.False(iBusyMock.Object.IsBusy);

            //Act
            thread.Start();
            while (isExecuting)
                Thread.Sleep(DELAY / 25);

            Thread.Sleep(5);
            //State is false after execution
            iBusyMock.VerifySet(o => o.IsBusy = false);
            //Only invoked once after set to true
            Assert.Equal(1, iBusyMock.Invocations.Count);
        }

        [Theory]
        [InlineData(7)]
        public void ExecuteT_WithIBusy_IsBusyTrueWhileRunning(int number)
        {
            var command = new SafeCommand<int>(executeAction: (i) => {
                //True while running
                iBusyMock.VerifySet(o=>o.IsBusy = true);
                iBusyMock.Invocations.Clear();
            }, iBusyMock.Object);

            //False before run
            Assert.False(iBusyMock.Object.IsBusy);

            //Act
            command.Execute(number);
            //False after run
            iBusyMock.VerifySet(o => o.IsBusy = false);
            Assert.Equal(1, iBusyMock.Invocations.Count);
        }

        [Theory]
        [InlineData(7)]
        public void ExecuteAsyncT_WithIBusy_IsBusyTrueWhileRunning(int number)
        {
            bool isExecuting = true;

            var command = new SafeCommand<int>(async (i) => {
                //True while running
                iBusyMock.VerifySet(o => o.IsBusy = true);
                await Task.Delay(DELAY);
                //Still True while running
                iBusyMock.VerifySet(o => o.IsBusy = true);
                iBusyMock.Invocations.Clear();
                isExecuting = false;
            }
            , iBusyMock.Object);

            var thread = new Thread(new ThreadStart(() => command.Execute(number)));

            //False before start
            Assert.False(iBusyMock.Object.IsBusy);
            thread.Start();
            while (isExecuting)
                Thread.Sleep(DELAY / 25);

            Thread.Sleep(5);
            //False after run
            iBusyMock.VerifySet(o => o.IsBusy = false);
            Assert.Equal(1, iBusyMock.Invocations.Count);
        }

        [Fact]
        public async Task Execute_ViewModelIsBusy_NotRun()
        {
            bool hasRun = false;
            iBusyMock.SetupGet(o => o.IsBusy).Returns(true);

            var command = new SafeCommand(executeAction: () => { hasRun = true; }, iBusyMock.Object);

            await Task.Run(()=>command.Execute(null));
            Assert.False(hasRun);
        }

        [Theory]
        [InlineData(7)]
        public async Task ExecuteT_ViewModelIsBusy_NotRun(int number)
        {
            bool hasRun = false;
            iBusyMock.SetupGet(o => o.IsBusy).Returns(true);

            var command = new SafeCommand<int>(executeAction: (i) => { hasRun = true; }, iBusyMock.Object);

            await Task.Run(()=>command.Execute(number));
            Assert.False(hasRun);
        }

        [Fact]
        public void ExecuteAsync_ViewModelIsBusy_NotRun()
        {
            bool hasRun = false;

            iBusyMock.SetupGet(o => o.IsBusy).Returns(true);
            var command = new SafeCommand(async () => { hasRun = true; await Task.Delay(DELAY); }, iBusyMock.Object);

            var thread = new Thread(new ThreadStart(() => command.Execute(null)));

            thread.Start();
            
            Thread.Sleep(DELAY);
            Assert.False(hasRun);
        }

        [Theory]
        [InlineData(7)]
        public void ExecuteAsyncT_ViewModelIsBusy_NotRun(int number)
        {
            bool hasRun = false;

            iBusyMock.SetupGet(o => o.IsBusy).Returns(true);
            var command = new SafeCommand<int>(async (i) => { hasRun = true; await Task.Delay(DELAY); }, iBusyMock.Object);

            var thread = new Thread(new ThreadStart(() => command.Execute(number)));

            thread.Start();

            Thread.Sleep(DELAY);
            Assert.False(hasRun);
        }

        
    }
}
