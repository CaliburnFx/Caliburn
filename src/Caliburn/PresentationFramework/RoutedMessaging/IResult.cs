namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System;

    /// <summary>
    /// Allows custom code to execute after the return of an action.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        void Execute(ResultExecutionContext context);

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        event EventHandler<ResultCompletionEventArgs> Completed;
    }
}