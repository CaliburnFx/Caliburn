namespace Caliburn.PresentationFramework.Actions
{
    using System.Linq;
    using Core.Invocation;
    using Filters;

    /// <summary>
    /// Hosts extension methods related to actions.
    /// </summary>
    public static class ActionExtensions
    {
        /// <summary>
        /// Determines whether the action has trigger effects.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>
        /// 	<c>true</c> if has trigger effects; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasTriggerEffects(this IAction action)
        {
            return action.Filters.TriggerEffects.Any();
        }

        /// <summary>
        /// Creates the filter manager for the method based on the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static IFilterManager CreateFilterManager(this ActionCreationContext context, IMethod method)
        {
            return new FilterManager(context.TargetType, method.Info, context.ServiceLocator).Combine(context.TargetFilters);
        }
    }
}