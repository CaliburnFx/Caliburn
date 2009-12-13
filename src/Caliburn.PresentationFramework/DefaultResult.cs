namespace Caliburn.PresentationFramework
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using Core;

    /// <summary>
    /// The default behavior for handling return values of bound methods.
    /// </summary>
    public class DefaultResult : IResult
    {
        private static readonly char[] _separator = new[] {'.'};
        private readonly IRoutedMessageController _routedMessageController;
        private readonly MessageProcessingOutcome _outcome;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultResult"/> class.
        /// </summary>
        /// <param name="routedMessageController">The routed message controller.</param>
        /// <param name="outcome">The outcome of processing the message.</param>
        public DefaultResult(IRoutedMessageController routedMessageController, MessageProcessingOutcome outcome)
        {
            _routedMessageController = routedMessageController;
            _outcome = outcome;
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event Action<IResult, Exception> Completed = delegate { };

        /// <summary>
        /// Executes the custom code.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="handlingNode">The handling node.</param>
        public virtual void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode)
        {
            if (_outcome.WasCancelled)
            {
                Completed(this, null);
                return;
            }

            var element = message.Source.UIElement as FrameworkElement;

            if(element != null)
            {
                if(string.IsNullOrEmpty(message.OutcomePath))
                {
                    var target = element.FindName<object>(message.DefaultOutcomeElement, false);

                    if (target != null)
                    {
                        var defaults = _routedMessageController.FindDefaultsOrFail(target);
                        defaults.SetDefaultValue(target, _outcome.Result);
                    }
                }
                else
                {
                    var parts = message.OutcomePath.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

                    if(parts.Length < 2)
                    {
                        Completed(this, new CaliburnException(string.Format("{0} is not a valid return path.", message.OutcomePath)));
                        return;
                    }

                    var target = element.FindName<object>(parts[0], true);
                    var setter = CreateSetter(target, parts.Skip(1).ToArray());

                    if (setter == null)
                    {
                        Completed(this, new CaliburnException(string.Format("{0} is not a valid property path.", parts.Skip(1).Aggregate((a, c) => a + c))));
                        return;
                    }

                    setter(_outcome.Result);
                }
            }
#if !SILVERLIGHT
            else
            {
                var fce = message.Source.UIElement as FrameworkContentElement;

                if(fce != null)
                {
                    if(string.IsNullOrEmpty(message.OutcomePath))
                    {
                        var target = fce.FindNameOrFail<object>(message.DefaultOutcomeElement);

                        if(target != null)
                        {
                            var defaults = _routedMessageController.FindDefaultsOrFail(target);
                            defaults.SetDefaultValue(target, _outcome.Result);
                        }
                    }
                    else
                    {
                        var parts = message.OutcomePath.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length < 2)
                        {
                            Completed(this, new CaliburnException(string.Format("{0} is not a valid return path.", message.OutcomePath)));
                            return;
                        }

                        var target = fce.FindNameOrFail<object>(parts[0]);
                        var setter = CreateSetter(target, parts.Skip(1).ToArray());

                        if(setter == null)
                        {
                            Completed(this, new CaliburnException(string.Format("{0} is not a valid property path.", parts.Skip(1).Aggregate((a, c) => a + c))));
                            return;    
                        }

                        setter(_outcome.Result);
                    }
                }
                else if(!string.IsNullOrEmpty(message.OutcomePath))
                {
                    Completed(this, new CaliburnException("Cannot determine parameters unless handler node is a FrameworkElement or FrameworkContentElement."));
                    return;
                }
            }
#else
            else if (!string.IsNullOrEmpty(message.OutcomePath))
            {
                Completed(this, new CaliburnException("Cannot determine parameters unless handler node is a FrameworkElement or FrameworkContentElement."));
                return;
            }
#endif

            Completed(this, null);
        }


        /// <summary>
        /// Finds the property setter.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <returns></returns>
        protected virtual Action<object> CreateSetter(object target, string[] propertyPath)
        {
            PropertyInfo propertyInfo;

            for(int i = 0; i < propertyPath.Length; i++)
            {
                string propertyName = propertyPath[i];
                propertyInfo = target.GetType().GetProperty(propertyName);

                if (propertyInfo == null)
                    return null;

                if(i < propertyPath.Length - 1)
                {
                    target = propertyInfo.GetValue(target, null);
                    if(target == null) return value => { };
                }
                else
                {
                    return value => propertyInfo.SetValue(
                                        target,
                                        DefaultMessageBinder.CoerceValueCore(
                                            propertyInfo.PropertyType,
                                            value
                                            ),
                                        null
                                        );
                }
            }

            return value => { };
        }
    }
}