using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICSP.Core.Extensions
{
  public static class TaskExtensions
  {
    public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
    {
      var tcs = new TaskCompletionSource<bool>();

      using(cancellationToken.Register(

                  s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))

        if(task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))

          throw new OperationCanceledException(cancellationToken);

      return await task.ConfigureAwait(false);

    }
    
    public static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
    {
      var tcs = new TaskCompletionSource<bool>();

      using(cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))

        if(task != await Task.WhenAny(task, tcs.Task))
          throw new OperationCanceledException(cancellationToken);

      await task.ConfigureAwait(false);
    }
    
    public static async Task<T> WithWaitCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
    {
      // The tasck completion source. 
      var tcs = new TaskCompletionSource<bool>();

      // Register with the cancellation token.
      using(cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
      {
        // If the task waited on is the cancellation token...
        if(task != await Task.WhenAny(task, tcs.Task))
          throw new OperationCanceledException(cancellationToken);
      }

      // Wait for one or the other to complete.
      return await task;
    }
  }
}
