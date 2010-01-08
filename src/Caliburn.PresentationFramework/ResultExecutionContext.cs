namespace Caliburn.PresentationFramework
{
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The context in which a result executes.
    /// </summary>
    public class ResultExecutionContext
    {
        private readonly IRoutedMessageWithOutcome _message;
        private readonly IInteractionNode _handlingNode;
        private readonly IServiceLocator _serviceLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultExecutionContext"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="message">The message.</param>
        /// <param name="handlingNode">The handling node.</param>
        public ResultExecutionContext(IServiceLocator serviceLocator, IRoutedMessageWithOutcome message, IInteractionNode handlingNode)
        {
            _message = message;
            _handlingNode = handlingNode;
            _serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public IRoutedMessageWithOutcome Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Gets the handling node.
        /// </summary>
        /// <value>The handling node.</value>
        public IInteractionNode HandlingNode
        {
            get { return _handlingNode; }
        }

        /// <summary>
        /// Gets the service locator.
        /// </summary>
        /// <value>The service locator.</value>
        public IServiceLocator ServiceLocator
        {
            get { return _serviceLocator; }
        }
    }
}