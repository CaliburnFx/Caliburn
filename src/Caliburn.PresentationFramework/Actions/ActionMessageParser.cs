namespace Caliburn.PresentationFramework.Actions
{
    using System.Windows;
    using System.Windows.Data;
    using Conventions;
    using RoutedMessaging;
    using RoutedMessaging.Parsers;

    /// <summary>
    /// An implementation of <see cref="ActionMessage"/> for <see cref="IMessageParser"/>.
    /// </summary>
    public class ActionMessageParser : MessageParserBase<ActionMessage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionMessageParser"/> class.
        /// </summary>
        /// <param name="conventionManager">The convention manager.</param>
        /// <param name="messageBinder">The message binder.</param>
        public ActionMessageParser(IConventionManager conventionManager, IMessageBinder messageBinder) 
            : base(conventionManager, messageBinder) {}

#if !SILVERLIGHT

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionMessageParser"/> class.
        /// </summary>
        /// <param name="conventionManager">The convention manager.</param>
        /// <param name="messageBinder">The message binder.</param>
        /// <param name="defaultTrigger">The default trigger.</param>
        public ActionMessageParser(IConventionManager conventionManager, IMessageBinder messageBinder, UpdateSourceTrigger defaultTrigger)
            : base(conventionManager, messageBinder, defaultTrigger) { }

#endif

        /// <summary>
        /// Parses the root portion of the message.
        /// </summary>
        /// <param name="messageText">The message text.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected override string[] ParseHeadAndTail(string messageText, ActionMessage message)
        {
            var headAndTail = base.ParseHeadAndTail(messageText, message);

            if(headAndTail.Length == 2)
                message.OutcomePath = headAndTail[1];

            return headAndTail;
        }

        /// <summary>
        /// Sets the core value of the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="target">The target.</param>
        /// <param name="coreOfMessage">The core representation of the message.</param>
        protected override void SetCore(ActionMessage message, DependencyObject target, string coreOfMessage)
        {
            message.MethodName = coreOfMessage;
        }
    }
}