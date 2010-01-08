namespace Caliburn.PresentationFramework.Actions
{
    using System;

    /// <summary>
    /// Designates an <see cref="IAction"/>.
    /// </summary>
    public class ActionAttribute : Attribute, IActionFactory
    {
        /// <summary>
        /// Gets or sets a value indicating whether to block interaction with the trigger during asynchronous execution.
        /// </summary>
        /// <value><c>true</c> if should block; otherwise, <c>false</c>.</value>
        public bool BlockInteraction { get; set; }

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
                context.ServiceLocator,
                method,
                context.MessageBinder,
                context.CreateFilterManager(method),
                BlockInteraction
                );

            context.ConventionManager
                .ApplyActionCreationConventions(action, method);

            return action;
        }
    }
}