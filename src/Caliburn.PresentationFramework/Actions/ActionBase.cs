namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Invocation;
    using Filters;

    /// <summary>
    /// A base implementation of <see cref="IAction"/>.
    /// </summary>
    public abstract class ActionBase : IAction
    {
        /// <summary>
        /// The method.
        /// </summary>
        protected readonly IMethod _method;
        /// <summary>
        /// The binder.
        /// </summary>
        protected readonly IMessageBinder _messageBinder;
        /// <summary>
        /// The filters.
        /// </summary>
        protected readonly IFilterManager _filters;
        /// <summary>
        /// The required parameters.
        /// </summary>
        protected IList<RequiredParameter> _requirements;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionBase"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="messageBinder">The method binder.</param>
        /// <param name="filters">The filters.</param>
        protected ActionBase(IMethod method, IMessageBinder messageBinder, IFilterManager filters)
        {
            _method = method;
            _messageBinder = messageBinder;
            _filters = filters;

            _requirements = _method.Info.GetParameters()
                .Select(x => new RequiredParameter(x.Name, x.ParameterType))
                .ToList();
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _method.Info.Name; }
        }

        /// <summary>
        /// Gets the requirements.
        /// </summary>
        /// <value>The requirements.</value>
        public IList<RequiredParameter> Requirements
        {
            get { return _requirements; }
        }

        /// <summary>
        /// Occurs when action has completed.
        /// </summary>
        public event EventHandler Completed = delegate { };

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        public IFilterManager Filters
        {
            get { return _filters; }
        }

        /// <summary>
        /// Determines whether this action matches the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public bool Matches(ActionMessage message)
        {
            if (_requirements.Count == message.Parameters.Count)
            {
                bool isMatch = true;

                for (int i = 0; i < message.Parameters.Count; i++)
                {
                    var expectedType = _requirements[i].Type;
                    var value = message.Parameters[i].Value;

                    if (value == null)
                    {
                        if (expectedType.IsClass || expectedType.IsInterface) continue;

                        isMatch = false;
                        break;
                    }

                    if (expectedType.IsAssignableFrom(value.GetType())) continue;

                    isMatch = false;
                    break;
                }

                return isMatch;
            }

            return false;
        }

        /// <summary>
        /// Determines how this instance affects trigger availability.
        /// </summary>
        /// <param name="actionMessage">The action message.</param>
        /// <param name="handlingNode">The node.</param>
        /// <returns>
        /// 	<c>true</c> if this instance enables triggers; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool ShouldTriggerBeAvailable(ActionMessage actionMessage, IInteractionNode handlingNode)
        {
            var parameters = _messageBinder.DetermineParameters(
                actionMessage,
                _requirements,
                handlingNode,
                null
                );

            foreach (var filter in _filters.TriggerEffects)
            {
                if (!filter.Execute(actionMessage, handlingNode, parameters)) 
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Executes the specified this action on the specified target.
        /// </summary>
        /// <param name="actionMessage">The action message.</param>
        /// <param name="handlingNode">The node.</param>
        /// <param name="context">The context.</param>
        public abstract void Execute(ActionMessage actionMessage, IInteractionNode handlingNode, object context);

        /// <summary>
        /// Applies the rescue or fails.
        /// </summary>
        /// <param name="message">The action message.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <param name="ex">The exception.</param>
        protected virtual bool TryApplyRescue(IRoutedMessage message, IInteractionNode handlingNode, Exception ex)
        {
            foreach (var rescue in _filters.Rescues)
            {
                if (rescue.Handle(message, handlingNode, ex))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Called when completed event needs to fire.
        /// </summary>
        protected virtual void OnCompleted()
        {
            Completed(this, EventArgs.Empty);
        }
    }
}