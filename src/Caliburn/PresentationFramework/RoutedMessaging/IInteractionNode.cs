namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Represents a node within the interaction hierarchy.
    /// </summary>
    public interface IInteractionNode
    {
        /// <summary>
        /// Gets the message target.
        /// </summary>
        /// <value>The action target.</value>
        IRoutedMessageHandler MessageHandler { get; }

        /// <summary>
        /// Gets the triggers.
        /// </summary>
        /// <value>The triggers.</value>
        IEnumerable<IMessageTrigger> Triggers { get; }

        /// <summary>
        /// Gets the UI element.
        /// </summary>
        /// <value>The UI element.</value>
        DependencyObject UIElement { get; }

        /// <summary>
        /// Finds the parent of this node.
        /// </summary>
        /// <returns>The parent or null if not found.</returns>
        IInteractionNode FindParent();

        /// <summary>
        /// Determines whether this node can handle the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        bool Handles(IRoutedMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">An object that provides additional context for message processing.</param>
        void ProcessMessage(IRoutedMessage message, object context);

        /// <summary>
        /// Updates the availability of the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        void UpdateAvailability(IMessageTrigger trigger);

        /// <summary>
        /// Sets the message handler for this node.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        void RegisterHandler(IRoutedMessageHandler messageHandler);

        /// <summary>
        /// Adds the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        void AddTrigger(IMessageTrigger trigger);
    }
}