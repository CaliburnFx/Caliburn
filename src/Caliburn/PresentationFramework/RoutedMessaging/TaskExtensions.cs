namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System;
    using System.Threading.Tasks;
    using Core.InversionOfControl;

    /// <summary>
    /// Extension methods to bring <see cref="System.Threading.Tasks.Task"/> and <see cref="IResult"/> together.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Executes an <see cref="IResult"/> asynchronous.
        /// </summary>
        /// <param name="result">The coroutine to execute.</param>
        /// <param name="context">The context to execute the coroutine within.</param>
        /// <returns>A task that represents the asynchronous coroutine.</returns>
        public static Task ExecuteAsync(this IResult result, ResultExecutionContext context = null)
        {
            return InternalExecuteAsync<object>(result, context);
        }

        /// <summary>
        /// Executes an <see cref="IResult"/> asynchronous.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="result">The coroutine to execute.</param>
        /// <param name="context">The context to execute the coroutine within.</param>
        /// <returns>A task that represents the asynchronous coroutine.</returns>
        public static Task<TResult> ExecuteAsync<TResult>(this IResult<TResult> result,
                                                          ResultExecutionContext context = null)
        {
            return InternalExecuteAsync<TResult>(result, context);
        }

        static Task<TResult> InternalExecuteAsync<TResult>(IResult result, ResultExecutionContext context)
        {
            var taskSource = new TaskCompletionSource<TResult>();

            EventHandler<ResultCompletionEventArgs> completed = null;
            completed = (s, e) => {
                result.Completed -= completed;

                if (e.Error != null)
                    taskSource.SetException(e.Error);
                else if (e.WasCancelled)
                    taskSource.SetCanceled();
                else
                {
                    var rr = result as IResult<TResult>;
                    taskSource.SetResult(rr != null ? rr.Result : default(TResult));
                }
            };

            try
            {
                IoC.BuildUp(result);
                result.Completed += completed;
                result.Execute(context ?? new ResultExecutionContext(IoC.Get<IServiceLocator>(), null, null));
            }
            catch (Exception ex)
            {
                result.Completed -= completed;
                taskSource.SetException(ex);
            }

            return taskSource.Task;
        }

        /// <summary>
        /// Encapsulates a task inside a couroutine.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns>The coroutine that encapsulates the task.</returns>
        public static TaskResult AsResult(this Task task)
        {
            return new TaskResult(task);
        }

        /// <summary>
        /// Encapsulates a task inside a couroutine.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="task">The task.</param>
        /// <returns>The coroutine that encapsulates the task.</returns>
        public static TaskResult<TResult> AsResult<TResult>(this Task<TResult> task)
        {
            return new TaskResult<TResult>(task);
        }
    }
}