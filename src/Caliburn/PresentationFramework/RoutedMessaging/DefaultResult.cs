namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using Conventions;
    using Core;

    /// <summary>
    /// The default behavior for handling return values of bound methods.
    /// </summary>
    public class DefaultResult : IResult
    {
        static readonly char[] Separator = new[] {'.'};
        readonly IConventionManager conventionManager;
        readonly MessageProcessingOutcome outcome;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultResult"/> class.
        /// </summary>
        /// <param name="conventionManager">The convention manager.</param>
        /// <param name="outcome">The outcome of processing the message.</param>
        public DefaultResult(IConventionManager conventionManager, MessageProcessingOutcome outcome)
        {
            this.conventionManager = conventionManager;
            this.outcome = outcome;
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        /// <summary>
        /// Executes the result within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Execute(ResultExecutionContext context)
        {
            if (outcome.WasCancelled)
            {
                Completed(this, new ResultCompletionEventArgs {WasCancelled = outcome.WasCancelled});
                return;
            }

            var element = context.Message.Source.UIElement;

            if (string.IsNullOrEmpty(context.Message.OutcomePath))
            {
                var target = element.FindNameExhaustive<DependencyObject>(context.Message.DefaultOutcomeElement, false);

                if (target != null)
                {
                    var defaults = conventionManager.FindElementConventionOrFail(target);
                    defaults.SetValue(target, outcome.Result);
                }
            }
            else
            {
                var parts = context.Message.OutcomePath.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 2)
                {
                    Completed(
                        this,
                        new ResultCompletionEventArgs
                        {
                            Error = new CaliburnException(
                                string.Format("{0} is not a valid return path.", context.Message.OutcomePath)
                                )
                        });
                    return;
                }

                var target = element.FindNameExhaustive<object>(parts[0], true);
                var setter = CreateSetter(target, parts.Skip(1).ToArray());

                if (setter == null)
                {
                    Completed(
                        this,
                        new ResultCompletionEventArgs
                        {
                            Error = new CaliburnException(
                                string.Format("{0} is not a valid property path.", parts.Skip(1).Aggregate((a, c) => a + c))
                                )
                        });
                    return;
                }

                setter(outcome.Result);
            }

            Completed(this, new ResultCompletionEventArgs());
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