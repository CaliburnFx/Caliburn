namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using System.Collections.Generic;
    using Core.Invocation;
    using Core.Metadata;
    using Filters;

    /// <summary>
    /// Hosts instances of <see cref="IAction"/> and related metadata.
    /// </summary>
    public interface IActionHost : IMetadataContainer, IEnumerable<IAction>
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
        /// Gets the action.
        /// </summary>
        /// <param name="message">The action message.</param>
        /// <returns></returns>
        IAction GetAction(ActionMessage message);

        /// <summary>
        /// Gets the filter manager for a given method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        IFilterManager GetFilterManager(IMethod method);
    }
}