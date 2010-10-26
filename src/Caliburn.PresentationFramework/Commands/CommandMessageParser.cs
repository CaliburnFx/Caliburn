namespace Caliburn.PresentationFramework.Commands
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using Conventions;
    using Core.InversionOfControl;
    using Core.Logging;
    using RoutedMessaging;
    using RoutedMessaging.Parsers;

    /// <summary>
    /// An implementation of <see cref="IMessageParser"/> for commands.
    /// </summary>
    public class CommandMessageParser : MessageParserBase<CommandMessage>
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(CommandMessageParser));

        private readonly CommandSource commandSource;

#if !SILVERLIGHT

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMessageParser"/> class.
        /// </summary>
        /// <param name="conventionManager">The convention mangager.</param>
        /// <param name="messageBinder">The message binder.</param>
        /// <param name="commandSource">The location that the parser will use to get the command.</param>
        public CommandMessageParser(IConventionManager conventionManager, IMessageBinder messageBinder, CommandSource commandSource)
            : this(conventionManager, messageBinder, UpdateSourceTrigger.PropertyChanged, commandSource) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMessageParser"/> class.
        /// </summary>
        /// <param name="conventionManager">The convention manager.</param>
        /// <param name="messageBinder">The binder.</param>
        /// <param name="defaultTrigger">The default trigger.</param>
        /// <param name="commandSource">The location that the parser will use to get the command.</param>
        public CommandMessageParser(IConventionManager conventionManager, IMessageBinder messageBinder, UpdateSourceTrigger defaultTrigger, CommandSource commandSource)
            : base(conventionManager, messageBinder, defaultTrigger)
        {
            this.commandSource = commandSource;
        }

#else
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMessageParser"/> class.
        /// </summary>
        /// <param name="messageBinder">The binder.</param>
        /// <param name="commandSource">The location that the parser will use to get the command.</param>
        public CommandMessageParser(IConventionManager conventionManager, IMessageBinder messageBinder, CommandSource commandSource)
            : base(conventionManager, messageBinder)
        {
            this.commandSource = commandSource;
        }

#endif

        /// <summary>
        /// Parses the root portion of the message.
        /// </summary>
        /// <param name="messageText">The message text.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected override string[] ParseHeadAndTail(string messageText, CommandMessage message)
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
        protected override void SetCore(CommandMessage message, DependencyObject target, string coreOfMessage)
        {
            switch(commandSource)
            {
                case CommandSource.Resource:
                    target.OnLoad(delegate{
                        message.Command = target.GetResource<object>(coreOfMessage);
                    });
                    break;
                case CommandSource.Container:
                    message.Command = IoC.GetInstance(null, coreOfMessage);
                    break;
                case CommandSource.Bound:
                    var binding = new Binding(coreOfMessage);
                    message.SetBinding(CommandMessage.CommandProperty, binding);
#if SILVERLIGHT
                    var frameworkElement = target as FrameworkElement;

                    if (frameworkElement != null)
                    {
                        frameworkElement.Loaded +=
                            delegate {
                                if (frameworkElement.DataContext != null)
                                    message.DataContext = frameworkElement.DataContext;
                            };
                    }
#endif
                    break;
                default:
                    var ex = new NotSupportedException(commandSource + " is not a supported command source.");
                    Log.Error(ex);
                    throw ex;
            }
        }
    }
}