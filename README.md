# ![Logo](art/icon@64x64.png) ZenMvvm.SafeAsyncHelpers
The suite of SafeAsync helpers provide a safe, consistent way to manage exceptions when running asynchronous code, including a solution to safely fire-and-forget tasks. The library includes an implementation of ICommand, and a Messaging Centre.

[![Coverage](https://raw.githubusercontent.com/zenmvvm/ZenMvvm.SafeAsyncHelpers/develop/coverage/badge_linecoverage.svg)](https://htmlpreview.github.io/?https://raw.githubusercontent.com/zenmvvm/ZenMvvm.SafeAsyncHelpers/develop/coverage/index.html) [![NuGet](https://buildstats.info/nuget/ZenMvvm.SafeAsyncHelpers?includePreReleases=false)](https://www.nuget.org/packages/ZenMvvm.SafeAsyncHelpers/)

The core of SafeAsync is the `SafeContinueWith` extension method for the `Task` object.


 Xamarin Forms' `Command` and `MessagingCenter` have been refactored to implement "safe execution".

To prevent application crashes from unhandled exceptions, initialise SafeExecutionHelpers with a **default exception handler** that logs the exception.

```c#
SafeExecutionHelpers.SetDefaultExceptionHandler(
    (ex) => Console.WriteLine(ex.Message));
```



`SafeCommand` and `SafeMessagingCentere` are refactored to automatically execute in a try-catch block. The methods have been refactored to include an optional `Action<Exception> onException` argument. Safe Execution applies the following logic:

* If `onException` has been provided and the type of `Exception` thrown matches the type handled in the provided `onException`, execute the provided `onException` handler. 

* Otherwise look for a match in the user-defined `GenericExceptionHandlers`. Generic handlers are initialized as follows

  * ```c#
    SafeExecutionHelpers.Configure(s => s.GenericExceptionHandlers.Add(
      (ArgumentException ex) => 
      {
        //Generic handling of ArgumentException here
      }));
    ```

* If no match is found, execute the `DefaultExceptionHandler` if it has been defined. The default handler is agnostic to the type of exception. If defined, it has the effect of silencing unhandled exceptions.

* Finally, if no handler is found and no default handler is defined, throw a `SafeExecutionHelpersException` with the offending exception as its `InnerException`.



For Debugging purposes, one can configure SafeExecutionHelpers to always rethrow exceptions after they have been handled:

```c#
#if DEBUG
	SafeExecutionHelpers.Configure(s => s.ShouldAlwaysRethrowException = true);
#endif
```



> :memo:Tip: When providing an `onException` delegate, if the developer anticipates several different exception-types, this can be handled by using pattern-matching (available from C# version 9). For example:
>
> ```c#
>   onException: (Exception ex) =>
>   {
>       switch (ex)
>       {
>           //Type matching pattern - C# Version 9
>           case ArgumentException:
>           		// Handle bad argument
>               break;
>   	      case DivideByZeroException:
>           		// Handle divide by zero
>     	      	break;
>   	      case OverflowException:
>           		// Handle when integer is too large to be stored
>           		break;
>           default:
>               Console.WriteLine(ex.Message);
>               break;
>       }            
>   }
> ```
>



## SafeCommand

In addition to implementing "Safe Execution", the SafeCommand offers the following useful features:

*  If a ViewModel has a bindable `IsBusy` property, will set this to `true` while executing. When using this feature, the default handling of multiple invokations is as follows. If the command is fired multiple times, and the first invokation has not completed the second invokation will be blocked from executing. An example where this is handy is to avoid unstable behaviour from app users double-tapping. 
  * Optionally, the developer can set the `isBlocking` argument to false, in which case every invokation will be executed.
  * The corresponding View can bind to `IsBusy` to show an activity indicator when the Command is running. 
* SafeCommand has been refactored with overloads to execute Asynchronous code. This removes the code-smell  `Command(async () => ExecuteCommandAsync)`, instead writing `SafeCommand(ExecuteCommandAsync)`. 
* Whereas Xamarin.Forms.Command begins executing on the Main thread, SafeCommand begins executing immediately on the background thread. This prevents UI-blocking code. If the developer explicity wants the command to run on the UI-thread, he can set the `mustRunOnCurrentSyncContext` parameter to true.



> :memo:Tip: When letting SafeCommand manipulate IsBusy, ensure that when you use OneWay binding in the `<RefreshView>`.
>
> ```xaml
> <RefreshView IsRefreshing="{Binding IsBusy, Mode=OneWay}" 
>              Command="{Binding LoadItemsCommand}">
> ```



## SafeMessagingCenter

This class refactors Xamarin's MessagingCenter with the same features described in `SafeCommand` above. In addition SafeMessagingCenter has been extended with `SubscribeAny` and `UnsubscribeAny`. This lets MessagingCenter subscribe to a specified message that may come from any Sender. This prevents unnecessary code repetition if the same Action should be executed in response to a message sent from different classes.



## SafeTask and SafeAction Extensions

`MyAction.SafeInvoke()` and `MyTask.SafeContinueWith()` will apply the [Safe Execution](#safe-execution) logic when handling exceptions.

`SafeFireAndForget` will safely fire-and-forget a task (instead of awaiting it), applying the [Safe Execution](#safe-execution) logic when handling exceptions. `SafeFireAndForget` is adapted from Brandon Minnick's [AsyncAwaitBestPractices](https://github.com/brminnick/AsyncAwaitBestPractices), which in turn was inspired by John Thiriet's blog post, [Removing Async Void](https://johnthiriet.com/removing-async-void/).


