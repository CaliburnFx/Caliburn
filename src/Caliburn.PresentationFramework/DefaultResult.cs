namespace Caliburn.PresentationFramework
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Conventions;
    using Core;

    /// <summary>
    /// The default behavior for handling return values of bound methods.
    /// </summary>
    public class DefaultResult : IResult
    {
        private static readonly char[] _separator = new[] {'.'};
        private readonly IConventionManager _conventionManager;
        private readonly MessageProcessingOutcome _outcome;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultResult"/> class.
        /// </summary>
        /// <param name="conventionManager">The convention manager.</param>
        /// <param name="outcome">The outcome of processing the message.</param>
        public DefaultResult(IConventionManager conventionManager, MessageProcessingOutcome outcome)
        {
            _conventionManager = conventionManager;
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

            var element = message.Source.UIElement;

            if (string.IsNullOrEmpty(message.OutcomePath))
            {
                var target = element.FindNameExhaustive<object>(message.DefaultOutcomeElement, false);

                if (target != null)
                {
                    var defaults = _conventionManager.FindElementConventionOrFail(target);
                    defaults.SetValue(target, _outcome.Result);
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

                var target = element.FindNameExhaustive<object>(parts[0], true);
                var setter = CreateSetter(target, parts.Skip(1).ToArray());

                if (setter == null)
                {
                    Completed(this, new CaliburnException(string.Format("{0} is not a valid property path.", parts.Skip(1).Aggregate((a, c) => a + c))));
                    return;
                }

                setter(_outcome.Result);
            }

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