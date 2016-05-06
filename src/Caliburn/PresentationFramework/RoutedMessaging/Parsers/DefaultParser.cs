namespace Caliburn.PresentationFramework.RoutedMessaging.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Conventions;
    using Core;
    using Core.Logging;
    using RoutedMessaging;

#if SILVERLIGHT
    using System.Globalization;
#endif

    /// <summary>
    /// An implementation of <see cref="IParser"/>.
    /// </summary>
    public class DefaultParser : IParser
    {
        static readonly ILog Log = LogManager.GetLog(typeof(DefaultParser));

        readonly IConventionManager conventionManager;
        readonly Dictionary<string, ITriggerParser> triggerParsers = new Dictionary<string, ITriggerParser>();
        readonly Dictionary<string, IMessageParser> messageParsers = new Dictionary<string, IMessageParser>();
        string defaultMessageParserKey = "Action";
        string messageDelimiter = ";";

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultParser"/> class.
        /// </summary>
        public DefaultParser(IConventionManager conventionManager)
        {
            this.conventionManager = conventionManager;

            RegisterTriggerParser("Event", new EventTriggerParser());
            RegisterTriggerParser("Gesture", new GestureTriggerParser());

#if !SILVERLIGHT
            RegisterTriggerParser("AttachedEvent", new AttachedEventTriggerParser());
            RegisterTriggerParser("CommandSource", new CommandTriggerParser());
#endif
        }

        /// <summary>
        /// Gets or sets the message delimiter.
        /// </summary>
        /// <value>The message delimiter.</value>
        public string MessageDelimiter
        {
            get { return messageDelimiter; }
            set { messageDelimiter = value; }
        }

        /// <summary>
        /// Sets the default message parser.
        /// </summary>
        /// <param name="key">The key.</param>
        public void SetDefaultMessageParser(string key)
        {
            defaultMessageParserKey = key;
        }

        /// <summary>
        /// Registers a trigger parser.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="parser">The parser.</param>
        public void RegisterTriggerParser(string key, ITriggerParser parser)
        {
            triggerParsers[key] = parser;
            Log.Info("Registered {0} as {1}.", parser, key);
        }

        /// <summary>
        /// Registers a message parser.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="parser">The parser.</param>
        public void RegisterMessageParser(string key, IMessageParser parser)
        {
            messageParsers[key] = parser;
        }

        /// <summary>
        /// Parses the specified message text.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="messageText">The message text.</param>
        /// <returns>The triggers parsed from the text.</returns>
        public IEnumerable<IMessageTrigger> Parse(DependencyObject target, string messageText)
        {
            var messages = messageText.Split(new[] { messageDelimiter }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var message in messages)
            {
                yield return ParseMessage(message, target);
            }
        }

        /// <summary>
        /// Parses the specified message text.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="messageText">The message text.</param>
        /// <returns>The triggers parsed from the text.</returns>
        protected virtual IMessageTrigger ParseMessage(string messageText, DependencyObject target)
        {
            var triggerPlusMessage = messageText.Split('=');
            string messageDetail = triggerPlusMessage.Last()
                .Replace("[", string.Empty)
                .Replace("]", string.Empty)
                .Trim();

            IRoutedMessage message = null;

            foreach (var keyValuePair in messageParsers)
            {
                if (messageDetail.StartsWith(keyValuePair.Key, StringComparison.CurrentCultureIgnoreCase))
                {
                    message = keyValuePair.Value
                        .Parse(target, messageDetail.Remove(0, keyValuePair.Key.Length).Trim());
                    break;
                }
            }

            if (message == null)
            {
                message = messageParsers[defaultMessageParserKey]
                    .Parse(target, messageDetail);
                Log.Info("Using default parser {0} for {1} on {2}.", defaultMessageParserKey, messageText, target);
            }

            IMessageTrigger trigger = null;

            if (triggerPlusMessage.Length == 1)
            {
                var defaults = conventionManager.FindElementConventionOrFail(target);
                trigger = defaults.CreateTrigger();
                Log.Info("Using default trigger {0} for {1} on {2}.", trigger, messageText, target);
            }
            else
            {
                var triggerDetail = triggerPlusMessage[0]
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty)
                    .Trim();

                foreach (var keyValuePair in triggerParsers)
                {
                    if (triggerDetail.StartsWith(keyValuePair.Key, StringComparison.CurrentCultureIgnoreCase))
                    {
                        trigger = keyValuePair.Value
                            .Parse(target, triggerDetail.Remove(0, keyValuePair.Key.Length).Trim());
                        break;
                    }
                }
            }

            if (trigger == null)
            {
                var exception = new CaliburnException("Could not determine trigger type.");
                Log.Error(exception);
                throw exception;
            }

            trigger.Message = message;

            return trigger;
        }
    }
}