namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using System.Collections.Generic;
    using Filters;

    /// <summary>
    /// Hosts instances of <see cref="IAction"/> and related metadata.
    /// </summary>
    public interface IActionHost
    {
        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        /// <value>The type of the target.</value>
        Type TargetType { get; }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        IFilterManager Filters { get; }

        /// <summary>
        /// Gets the actions.
        /// </summary>
        /// <value>The actions.</value>
        IEnumerable<IAction> Actions { get; }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <param name="message">The action message.</param>
        /// <returns></returns>
        IAction GetAction(ActionMessage message);
    }
}