namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System.Collections.Generic;

    /// <summary>
    /// Implemented by classes that handler routed messages.
    /// </summary>
    public interface IRoutedMessageHandler
    {
        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        IList<object> Metadata { get; }

        /// <summary>
        /// Initializes this handler on the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        void Initialize(IInteractionNode node);

        /// <summary>
        /// Gets the underlying object instance to which this handler routes requests.
        /// </summary>
        /// <returns></returns>
        object Unwrap();

        /// <summary>
        /// Indicates whether this instance can handle the speicified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        bool Handles(IRoutedMessage message);

        /// <summary>
        /// Processes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">An object that provides additional context for message processing.</param>
        void Process(IRoutedMessage message, object context);

        /// <summary>
        /// Updates the availability of the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        void UpdateAvailability(IMessageTrigger trigger);

        /// <summary>
        /// Makes the handler aware of a specific trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        void MakeAwareOf(IMessageTrigger trigger);
    }
}