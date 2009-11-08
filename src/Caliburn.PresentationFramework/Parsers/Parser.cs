namespace Caliburn.PresentationFramework.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Core;

#if SILVERLIGHT
    using System.Globalization;
#endif

    /// <summary>
    /// An implementation of <see cref="IParser"/>.
    /// </summary>
    public class Parser : IParser
    {
        private readonly Dictionary<string, ITriggerParser> _triggerParsers = new Dictionary<string, ITriggerParser>();
        private readonly Dictionary<string, IMessageParser> _messageParsers = new Dictionary<string, IMessageParser>();
        private readonly IRoutedMessageController _controller;
        private string _defaultMessageParserKey = "Action";
        private string _messageDelimiter = ";";

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public Parser(IRoutedMessageController controller)
        {
            _controller = controller;

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
            get { return _messageDelimiter; }
            set { _messageDelimiter = value; }
        }

        /// <summary>
        /// Sets the default message parser.
        /// </summary>
        /// <param name="key">The key.</param>
        public void SetDefaultMessageParser(string key)
        {
            _defaultMessageParserKey = key;
        }

        /// <summary>
        /// Registers a trigger parser.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="parser">The parser.</param>
        public void RegisterTriggerParser(string key, ITriggerParser parser)
        {
            _triggerParsers[key] = parser;
        }

        /// <summary>
        /// Registers a message parser.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="parser">The parser.</param>
        public void RegisterMessageParser(string key, IMessageParser parser)
        {
            _messageParsers[key] = parser;
        }

        /// <summary>
        /// Parses the specified message text.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="messageText">The message text.</param>
        /// <returns>The triggers parsed from the text.</returns>
        public IEnumerable<IMessageTrigger> Parse(DependencyObject target, string messageText)
        {
            var messages = messageText.Split(new[] { _messageDelimiter }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var message in messages)
            {
                yield return ParseMessage(message, target);
            }
        }

        /// <summary>
        /// Parses an individual message.
        /// </summary>
        /// <param name="messageText">The message text.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        protected virtual IMessageTrigger ParseMessage(string messageText, DependencyObject target) 
        {
            var triggerPlusMessage = messageText.Split('=');
            IMessageTrigger trigger = null;
            string messageDetail;

            if(triggerPlusMessage.Length == 1)
            {
                var defaults = _controller.FindDefaultsOrFail(target);
                trigger = defaults.GetDefaultTrigger();

                messageDetail = triggerPlusMessage[0]
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty)
                    .Trim();
            }
            else
            {
                var triggerDetail = triggerPlusMessage[0]
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty)
                    .Trim();

                messageDetail = triggerPlusMessage[1]
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty)
                    .Trim();

                foreach(var keyValuePair in _triggerParsers)
                {
                    if (triggerDetail.StartsWith(keyValuePair.Key, StringComparison.CurrentCultureIgnoreCase))
                    {
                        trigger = keyValuePair.Value
                            .Parse(target, triggerDetail.Remove(0, keyValuePair.Key.Length).Trim());
                        break;
                    }
                }
            }

            if(trigger == null)
                throw new CaliburnException("Could not determine trigger type.");

            foreach (var keyValuePair in _messageParsers)
            {
                if (messageDetail.StartsWith(keyValuePair.Key, StringComparison.CurrentCultureIgnoreCase))
                {
                    trigger.Message = keyValuePair.Value
                        .Parse(target, messageDetail.Remove(0, keyValuePair.Key.Length).Trim());
                    break;
                }
            }

            if(trigger.Message == null)
            {
                trigger.Message = _messageParsers[_defaultMessageParserKey]
                    .Parse(target, messageDetail);
            }

            return trigger;
        }
    }
}