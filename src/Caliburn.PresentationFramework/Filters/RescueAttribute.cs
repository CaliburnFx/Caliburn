namespace Caliburn.PresentationFramework.Filters
{
    using System;
    using RoutedMessaging;

    /// <summary>
    /// A basic rescue filter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RescueAttribute : MethodCallFilterBase, IRescue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RescueAttribute"/> class.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        public RescueAttribute(string methodName)
            : base(methodName) {}


        /// <summary>
        /// Handles an <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <param name="exception">The exception.</param>
        /// <returns>
        /// true if the exception was handled, false otherwise
        /// </returns>
        public bool Handle(IRoutedMessage message, IInteractionNode handlingNode, Exception exception)
        {
            var result = Method.Invoke(handlingNode.MessageHandler.Unwrap(), exception);

            if(Method.Info.ReturnType == typeof(bool))
                return (bool)result;
            return true;
        }
    }
}