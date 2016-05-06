namespace Caliburn.PresentationFramework.Actions
{
    using RoutedMessaging;

    /// <summary>
    /// Implemented by classes which can handle an <see cref="ActionMessage"/>.
    /// </summary>
    public interface IActionHandler
    {
        /// <summary>
        /// Determines how this instance affects trigger availability.
        /// </summary>
        /// <param name="actionMessage">The action message.</param>
        /// <param name="handlingNode">The node.</param>
        /// <returns>
        /// 	<c>true</c> if this instance enables triggers; otherwise, <c>false</c>.
        /// </returns>
        bool ShouldTriggerBeAvailable(ActionMessage actionMessage, IInteractionNode handlingNode);

        /// <summary>
        /// Executes the specified action on the specified target.
        /// </summary>
        /// <param name="actionMessage">The action message.</param>
        /// <param name="handlingNode">The node.</param>
        /// <param name="context">The context.</param>
        void Execute(ActionMessage actionMessage, IInteractionNode handlingNode, object context);
    }
}