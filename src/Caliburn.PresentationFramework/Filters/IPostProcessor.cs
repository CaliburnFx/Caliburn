namespace Caliburn.PresentationFramework.Filters
{
    using RoutedMessaging;

    /// <summary>
    /// A filter that is executed after something.
    /// </summary>
    public interface IPostProcessor : IFilter
    {
        /// <summary>
        /// Executes the filter.
        /// </summary>
        /// <param name="outcome">The outcome of processing the message</param>
        /// <param name="message">The message.</param>
        /// <param name="handlingNode">The handling node.</param>
        void Execute(IRoutedMessage message, IInteractionNode handlingNode, MessageProcessingOutcome outcome);
    }
}