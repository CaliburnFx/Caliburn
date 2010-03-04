namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using System.Collections.Generic;
    using Filters;
    using RoutedMessaging;

    /// <summary>
    /// Represents a single action.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the requirements.
        /// </summary>
        /// <value>The requirements.</value>
        IList<RequiredParameter> Requirements { get; }

        /// <summary>
        /// Occurs when action has completed.
        /// </summary>
        event EventHandler Completed;

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        IFilterManager Filters { get; }

        /// <summary>
        /// Determines whether this action matches the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        bool Matches(ActionMessage message);

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