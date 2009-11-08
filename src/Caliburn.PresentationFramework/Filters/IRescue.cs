namespace Caliburn.PresentationFramework.Filters
{
    using System;

    /// <summary>
    /// A <see cref="IFilter"/> that performs a rescue.
    /// </summary>
    public interface IRescue : IFilter
    {
        /// <summary>
        /// Handles an <see cref="Exception"/>.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <returns>true if the exception was handled, false otherwise</returns>
        bool Handle(IRoutedMessage message, IInteractionNode handlingNode, Exception exception);
    }
}