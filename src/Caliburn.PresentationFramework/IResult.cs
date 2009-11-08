namespace Caliburn.PresentationFramework
{
    using System;

    /// <summary>
    /// Allows custom code to execute after the return of a method.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Executes the custom code.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="handlingNode">The handling node.</param>
        void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode);

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        event Action<IResult, Exception> Completed;
    }
}