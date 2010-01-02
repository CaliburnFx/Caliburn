namespace Caliburn.PresentationFramework
{
    using System.Windows;

    /// <summary>
    /// A service that manages all aspects of Caliburn's routed message mechanism.
    /// </summary>
    public interface IRoutedMessageController
    {
        /// <summary>
        /// Adds a message handler at the specified location in the UI hierarchy.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="handler">The message handler.</param>
        /// <param name="setContext">if set to <c>true</c> the handler will also be stored in the element's DataContext.</param>
        void AddHandler(DependencyObject uiElement, IRoutedMessageHandler handler, bool setContext);

        /// <summary>
        /// Attaches the trigger to the UI and prepares it to send messages.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="trigger">The trigger.</param>
        void AttachTrigger(DependencyObject uiElement, IMessageTrigger trigger);

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <param name="uiElement">The UI element to retrieve the parent for.</param>
        /// <returns></returns>
        IInteractionNode GetParent(DependencyObject uiElement);
    }
}