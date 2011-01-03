using System.Text.RegularExpressions;

namespace Caliburn.PresentationFramework.RoutedMessaging.Parsers
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using Conventions;
    using RoutedMessaging;
    using Views;

    /// <summary>
    /// An base implementation of <see cref="IMessageParser"/>.
    /// </summary>
    public abstract class MessageParserBase<T> : IMessageParser
        where T : IRoutedMessage, new()
    {
        static readonly Regex Regex = new Regex(@",(?=(?:[^']*'[^']*')*(?![^']*'))");
        readonly IConventionManager conventionManager;
        readonly IMessageBinder messageBinder;

#if !SILVERLIGHT

        private readonly UpdateSourceTrigger defaultTrigger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageParserBase{T}"/> class.
        /// </summary>
        protected MessageParserBase(IConventionManager conventionManager, IMessageBinder messageBinder)
            : this(conventionManager, messageBinder, UpdateSourceTrigger.PropertyChanged) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageParserBase{T}"/> class.
        /// </summary>
        /// <param name="conventionManager">The convention manager.</param>
        /// <param name="messageBinder">The message binder.</param>
        /// <param name="defaultTrigger">The default trigger.</param>
        protected MessageParserBase(IConventionManager conventionManager, IMessageBinder messageBinder, UpdateSourceTrigger defaultTrigger)
        {
            this.conventionManager = conventionManager;
            this.messageBinder = messageBinder;
            this.defaultTrigger = defaultTrigger;
        }

#else
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageParserBase{T}"/> class.
        /// </summary>
        /// <param name="messageBinder">The message binder.</param>
        protected MessageParserBase(IConventionManager conventionManager, IMessageBinder messageBinder)
        {
            this.conventionManager = conventionManager;
            this.messageBinder = messageBinder;
        }
#endif

        /// <summary>
        /// Parses the specified message text.
        /// </summary>
        /// <param name="target">The targeted UI element.</param>
        /// <param name="messageText">The message text.</param>
        /// <returns></returns>
        public IRoutedMessage Parse(DependencyObject target, string messageText)
        {
            var message = new T();

            var headAndTail = ParseHeadAndTail(messageText, message);

            var openingParenthesisIndex = headAndTail[0].IndexOf('(');
            if (openingParenthesisIndex < 0) openingParenthesisIndex = headAndTail[0].Length;
            var closingParenthesisIndex = headAndTail[0].LastIndexOf(')');
            if (closingParenthesisIndex < 0) closingParenthesisIndex = headAndTail[0].Length;

            var core = headAndTail[0].Substring(0, openingParenthesisIndex).Trim();

            SetCore(message, target, core);

            if (closingParenthesisIndex - openingParenthesisIndex > 1)
            {
                var paramString = headAndTail[0].Substring(openingParenthesisIndex + 1,
                    closingParenthesisIndex - openingParenthesisIndex - 1);

                var parameters = Regex.Split(paramString);

                foreach (var parameter in parameters)
                {
                    message.Parameters.Add(
                        CreateParameter(target, parameter.Trim())
                        );
                }
            }

            return message;
        }

        /// <summary>
        /// Sets the core value of the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="target">The target.</param>
        /// <param name="coreOfMessage">The core representation of the message.</param>
        protected abstract void SetCore(T message, DependencyObject target, string coreOfMessage);

        /// <summary>
        /// Parses the root portion of the message.
        /// </summary>
        /// <param name="messageText">The message text.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected virtual string[] ParseHeadAndTail(string messageText, T message)
        {
            int indexOfColon = messageText.LastIndexOf(':');

            if (indexOfColon == -1)
                return new[] { messageText.Trim() }; //no tail

            int indexOfParen = messageText.LastIndexOf(')');

            if (indexOfParen == -1)
                return messageText.Split(':').Select(x => x.Trim()).ToArray(); //no parameters

            if (indexOfColon < indexOfParen)
                return new[] { messageText.Trim() }; //no tail

            return new[]
            {
                messageText.Substring(0, indexOfColon).Trim(),
                messageText.Substring(indexOfColon + 1).Trim()
            };
        }

        Parameter CreateParameter(DependencyObject target, string parameter)
        {
            var actualParameter = new Parameter();

            if (parameter.StartsWith("'") && parameter.EndsWith("'"))
                actualParameter.Value = parameter.Substring(1, parameter.Length - 2);
            else if (messageBinder.IsSpecialValue(parameter) || char.IsNumber(parameter[0]))
                actualParameter.Value = parameter;
            else
            {
                var fe = target as FrameworkElement;
                if (fe != null)
                {
                    RoutedEventHandler handler = null;
                    handler = (s, e) =>{
                        BindParameter(fe, parameter, actualParameter);
                        fe.Loaded -= handler;
                    };
#if !SILVERLIGHT
                    if(fe.IsLoaded || (bool)fe.GetValue(View.IsLoadedProperty))
#else
                    if((bool)fe.GetValue(View.IsLoadedProperty))
#endif
                        handler(null, null);
                    else fe.Loaded += handler;
                }
#if !SILVERLIGHT
                else
                {
                    var fce = target as FrameworkContentElement;
                    if (fce != null)
                    {
                        RoutedEventHandler handler = null;
                        handler = (s, e) =>{
                            BindParameter(fce, parameter, actualParameter);
                            fce.Loaded -= handler;
                        };

                        if (fce.IsLoaded || (bool)fce.GetValue(View.IsLoadedProperty))
                            handler(null, null);
                        else fce.Loaded += handler;
                    }
                }
#endif
            }

            return actualParameter;
        }

        void BindParameter(DependencyObject target, string parameter, Parameter actualParameter)
        {
#if !SILVERLIGHT
            var nameAndBindingMode = parameter.Split(':')
                .Select(x => x.Trim()).ToArray();

            var index = nameAndBindingMode[0].IndexOf('.');

            if (index == -1)
                actualParameter.Bind(target, parameter, null);
            else
            {
                var elementName = nameAndBindingMode[0].Substring(0, index);
                var path = new PropertyPath(nameAndBindingMode[0].Substring(index + 1));

                var binding = elementName == "$this"
                                  ? new Binding
                                  {
                                      Path = path,
                                      Source = target,
                                      UpdateSourceTrigger = defaultTrigger
                                  }
                                  : new Binding
                                  {
                                      Path = path,
                                      ElementName = elementName,
                                      UpdateSourceTrigger = defaultTrigger
                                  };

                if (nameAndBindingMode.Length == 2)
                    binding.Mode = (BindingMode)Enum.Parse(typeof(BindingMode), nameAndBindingMode[1]);

                BindingOperations.SetBinding(actualParameter, Parameter.ValueProperty, binding);
            }
#else
            var index = parameter.IndexOf('.');

            if(index > 0)
            {
                actualParameter.ElementName = parameter.Substring(0, index);
                actualParameter.Path = parameter.Substring(index + 1);
            }
            else actualParameter.ElementName = parameter;
#endif
        }
    }
}