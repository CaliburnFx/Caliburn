namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using System.Reflection;
    using Core.Invocation;
    using Core.InversionOfControl;
    using Filters;
    using RoutedMessaging;

    /// <summary>
    /// Designates an action as asynchronous.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AsyncActionAttribute : Attribute, IInitializable, IPostProcessor, IActionFactory
    {
        IMethod callback;

        /// <summary>
        /// Gets or sets a value indicating whether to block interaction with the trigger during asynchronous execution.
        /// </summary>
        /// <value><c>true</c> if should block; otherwise, <c>false</c>.</value>
        public bool BlockInteraction { get; set; }

        /// <summary>
        /// Gets or sets the callback method.
        /// </summary>
        /// <value>The callback.</value>
        public string Callback { get; set; }

        /// <summary>
        /// Gets the order the filter will be evaluated in.
        /// </summary>
        /// <value>The order.</value>
        public int Priority { get; set; }

        /// <summary>
        /// Initializes the filter.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="member">The member.</param>
        /// <param name="serviceLocator">The service locator.</param>
        public virtual void Initialize(Type targetType, MemberInfo member, IServiceLocator serviceLocator)
        {
            if(string.IsNullOrEmpty(Callback)) return;

            var methodInfo = targetType.GetMethod(
                Callback,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static
                );

            if (methodInfo == null)
                throw new Exception(
                    string.Format("Method '{0}' could not be found on '{1}'.",
                        Callback,
                        targetType.FullName
                        )
                    );

            callback = serviceLocator.GetInstance<IMethodFactory>().CreateFrom(methodInfo);
        }


        /// <summary>
        /// Executes the filter.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <param name="outcome">The outcome of processing the message</param>
        public virtual void Execute(IRoutedMessage message, IInteractionNode handlingNode, MessageProcessingOutcome outcome)
        {
            if(callback == null || outcome.WasCancelled)
                return;

            outcome.Result = callback.Invoke(handlingNode.MessageHandler.Unwrap(), outcome.Result);
            outcome.ResultType = callback.Info.ReturnType;
        }

        /// <summary>
        /// Creates an <see cref="IAction"/> using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="IAction"/>.</returns>
        public IAction Create(ActionCreationContext context)
        {
            var method = context.MethodFactory
                .CreateFrom(context.Method);

            var action = new AsynchronousAction(
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