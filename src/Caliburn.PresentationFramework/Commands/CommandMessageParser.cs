namespace Caliburn.PresentationFramework.Commands
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using Microsoft.Practices.ServiceLocation;
    using Parsers;

    /// <summary>
    /// An implementation of <see cref="IMessageParser"/> for commands.
    /// </summary>
    public class CommandMessageParser : MessageParserBase<CommandMessage>
    {
        private readonly CommandSource _commandSource;

#if !SILVERLIGHT

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMessageParser"/> class.
        /// </summary>
        /// <param name="messageBinder">The message binder.</param>
        /// <param name="commandSource">The location that the parser will use to get the command.</param>
        public CommandMessageParser(IMessageBinder messageBinder, CommandSource commandSource)
            : this(messageBinder, UpdateSourceTrigger.PropertyChanged, commandSource) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMessageParser"/> class.
        /// </summary>
        /// <param name="messageBinder">The binder.</param>
        /// <param name="defaultTrigger">The default trigger.</param>
        /// <param name="commandSource">The location that the parser will use to get the command.</param>
        public CommandMessageParser(IMessageBinder messageBinder, UpdateSourceTrigger defaultTrigger, CommandSource commandSource)
            : base(messageBinder, defaultTrigger)
        {
            _commandSource = commandSource;
        }

#else
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMessageParser"/> class.
        /// </summary>
        /// <param name="messageBinder">The binder.</param>
        /// <param name="commandSource">The location that the parser will use to get the command.</param>
        public CommandMessageParser(IMessageBinder messageBinder, CommandSource commandSource)
            : base(messageBinder)
        {
            _commandSource = commandSource;
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
            switch(_commandSource)
            {
                case CommandSource.Resource:
                    var frameworkElement = target as FrameworkElement;

                    if(frameworkElement != null)
                    {
                        frameworkElement.Loaded +=
                            delegate { message.Command = frameworkElement.GetResource<object>(coreOfMessage); };
                    }
#if !SILVERLIGHT
                    else
                    {
                        var fce = target as FrameworkContentElement;

                        if(fce != null)
                        {
                            fce.Loaded +=
                                delegate { message.Command = fce.GetResource<object>(coreOfMessage); };
                        }
                    }
#endif
                    break;
                case CommandSource.Container:
                    message.Command = ServiceLocator.Current.GetInstance(null, coreOfMessage);
                    break;
                case CommandSource.Bound:
                    var binding = new Binding(coreOfMessage);
#if !SILVERLIGHT
                    BindingOperations.SetBinding(message, CommandMessage.CommandProperty, binding);
#else
                    var frameworkElement2 = target as FrameworkElement;

                    if (frameworkElement2 != null)
                    {
                        frameworkElement2.Loaded +=
                            delegate {
                                if (frameworkElement2.DataContext != null)
                                    message.DataContext = frameworkElement2.DataContext;
                            };

                        message.SetBinding(CommandMessage.CommandProperty, binding);
                    }
#endif
                    break;
                default:
                    throw new NotSupportedException(_commandSource + " is not a supported command source.");
            }
        }
    }
}