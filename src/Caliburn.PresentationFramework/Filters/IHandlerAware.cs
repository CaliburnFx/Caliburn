namespace Caliburn.PresentationFramework.Filters
{
    using RoutedMessaging;

    /// <summary>
    /// A filter that is aware individual <see cref="IMessageTrigger"/> and <see cref="IRoutedMessageHandler"/> instances.
    /// </summary>
    public interface IHandlerAware : IFilter
    {
        /// <summary>
        /// Makes the filter aware of the <see cref="IRoutedMessageHandler"/>.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        void MakeAwareOf(IRoutedMessageHandler messageHandler);

        /// <summary>
        /// Makes the filter aware of the <see cref="IMessageTrigger"/>.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        /// <param name="trigger">The trigger.</param>
        void MakeAwareOf(IRoutedMessageHandler messageHandler, IMessageTrigger trigger);
    }
}