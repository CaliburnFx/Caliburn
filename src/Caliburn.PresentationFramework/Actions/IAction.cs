namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using System.Collections.Generic;
    using Filters;
    using RoutedMessaging;

    /// <summary>
    /// Represents a single action.
    /// </summary>
    public interface IAction : IActionHandler
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
    }
}