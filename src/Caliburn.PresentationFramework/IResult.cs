namespace Caliburn.PresentationFramework
{
    using System;

    /// <summary>
    /// Allows custom code to execute after the return of a method.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Executes the result within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        void Execute(ResultExecutionContext context);

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        event EventHandler<ResultCompletionEventArgs> Completed;
    }
}