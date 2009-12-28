namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using Filters;

    /// <summary>
    /// Designates an <see cref="IAction"/>.
    /// </summary>
    public class ActionAttribute : Attribute, IActionFactory
    {
        /// <summary>
        /// Creates an <see cref="IAction"/> using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="IAction"/>.</returns>
        public IAction Create(ActionCreationContext context)
        {
            var method = context.MethodFactory
                .CreateFrom(context.Method);

            var action = new SynchronousAction(
                method,
                context.MessageBinder,
                new FilterManager(context.TargetType, method, context.ServiceLocator).Combine(context.TargetFilters)
                );

            context.ApplyActionFilterConventions(action, method);

            return action;
        }
    }
}