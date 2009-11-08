namespace Caliburn.PresentationFramework
{
    /// <summary>
    /// Implementors enable various methods of triggering the sending of messages.
    /// </summary>
    public interface IMessageTrigger
    {
        /// <summary>
        /// Gets the node within the interaction hierarchy that this trigger is a part of.
        /// </summary>
        /// <value>The node.</value>
        IInteractionNode Node { get; }

        /// <summary>
        /// Gets the message that this trigger will send.
        /// </summary>
        /// <value>The message.</value>
        IRoutedMessage Message { get; set; }

        /// <summary>
        /// Attaches the trigger to the interaction hierarchy.
        /// </summary>
        /// <param name="node">The node.</param>
        void Attach(IInteractionNode node);

        /// <summary>
        /// Updates the UI to reflect the availabilty of the trigger.
        /// </summary>
        /// <param name="isAvailable">if set to <c>true</c> [is available].</param>
        void UpdateAvailabilty(bool isAvailable);
    }
}