namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using Core.InversionOfControl;

    /// <summary>
    /// The context in which an <see cref="IResult"/> executes.
    /// </summary>
    public class ResultExecutionContext
    {
        readonly IRoutedMessageWithOutcome message;
        readonly IInteractionNode handlingNode;
        readonly IServiceLocator serviceLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultExecutionContext"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="message">The message.</param>
        /// <param name="handlingNode">The handling node.</param>
        public ResultExecutionContext(IServiceLocator serviceLocator, IRoutedMessageWithOutcome message, IInteractionNode handlingNode)
        {
            this.message = message;
            this.handlingNode = handlingNode;
            this.serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public IRoutedMessageWithOutcome Message
        {
            get { return message; }
        }

        /// <summary>
        /// Gets the handling node.
        /// </summary>
        /// <value>The handling node.</value>
        public IInteractionNode HandlingNode
        {
            get { return handlingNode; }
        }

        /// <summary>
        /// Gets the service locator.
        /// </summary>
        /// <value>The service locator.</value>
        public IServiceLocator ServiceLocator
        {
            get { return serviceLocator; }
        }
    }
}