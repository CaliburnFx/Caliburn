namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Invocation;
    using Core.Logging;
    using Filters;
    using RoutedMessaging;

    /// <summary>
    /// A base implementation of <see cref="IAction"/>.
    /// </summary>
    public abstract class ActionBase : IAction
    {
        static readonly ILog Log = LogManager.GetLog(typeof(ActionBase));

        /// <summary>
        /// The method.
        /// </summary>
        protected readonly IMethod UnderlyingMethod;

        /// <summary>
        /// The binder.
        /// </summary>
        protected readonly IMessageBinder MessageBinder;

        /// <summary>
        /// The filters.
        /// </summary>
        protected readonly IFilterManager UnderlyingFilters;

        /// <summary>
        /// The required parameters.
        /// </summary>
        protected IList<RequiredParameter> UnderlyingRequirements;

        readonly bool blockInteraction;

        /// <summary>
        /// Gets a value indicating whether to block intaction with the trigger during async execution.
        /// </summary>
        /// <value><c>true</c> if should block; otherwise, <c>false</c>.</value>
        public bool BlockInteraction
        {
            get { return blockInteraction; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionBase"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="messageBinder">The method binder.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="blockInteraction">if set to <c>true</c> blocks interaction.</param>
        protected ActionBase(IMethod method, IMessageBinder messageBinder, IFilterManager filters, bool blockInteraction)
        {
            UnderlyingMethod = method;
            MessageBinder = messageBinder;
            UnderlyingFilters = filters;
            this.blockInteraction = blockInteraction;

            UnderlyingRequirements = UnderlyingMethod.Info.GetParameters()
                .Select(x => new RequiredParameter(x.Name, x.ParameterType))
                .ToList();
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return UnderlyingMethod.Info.Name; }
        }

        /// <summary>
        /// Gets the requirements.
        /// </summary>
        /// <value>The requirements.</value>
        public IList<RequiredParameter> Requirements
        {
            get { return UnderlyingRequirements; }
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
            get { return UnderlyingFilters; }
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <value>The method.</value>
        public IMethod Method
        {
            get { return UnderlyingMethod; }
        }

        /// <summary>
        /// Determines whether this action matches the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public bool Matches(ActionMessage message)
        {
            if (UnderlyingRequirements.Count == message.Parameters.Count)
            {
                bool isMatch = true;

                for (int i = 0; i < message.Parameters.Count; i++)
                {
                    var expectedType = UnderlyingRequirements[i].Type;
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

                Log.Info("Action {0} for {1}.", isMatch ? "found" : "not found", this);
                return isMatch;
            }

            Log.Info("Action not found for {0}.", this);
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
            if (!HasTriggerEffects())
                return true;

            var parameters = MessageBinder.DetermineParameters(
                actionMessage,
                UnderlyingRequirements,
                handlingNode,
                null
                );

            Log.Info("Evaluating trigger effects for {0}.", this);

            foreach (var filter in UnderlyingFilters.TriggerEffects)
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
        /// Determines whether this action has trigger effects.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if has trigger effects; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool HasTriggerEffects()
        {
            return Filters.TriggerEffects.Any();
        }

        /// <summary>
        /// Applies the rescue or fails.
        /// </summary>
        /// <param name="message">The action message.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <param name="ex">The exception.</param>
        protected virtual bool TryApplyRescue(IRoutedMessage message, IInteractionNode handlingNode, Exception ex)
        {
            foreach (var rescue in UnderlyingFilters.Rescues)
            {
                if (rescue.Handle(message, handlingNode, ex))
                {
                    Log.Info("Rescue applied for {0}.", this);
                    return true;
                }
            }

            Log.Warn("Rescue not applied for {0}.", this);
            return false;
        }

        /// <summary>
        /// Called when completed event needs to fire.
        /// </summary>
        protected virtual void OnCompleted()
        {
            Completed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Tries to update trigger.
        /// </summary>
        /// <param name="actionMessage">The action message.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <param name="forceDisabled">if set to <c>true</c> [force disabled].</param>
        protected virtual void TryUpdateTrigger(ActionMessage actionMessage, IInteractionNode handlingNode, bool forceDisabled)
        {
            if (!BlockInteraction)
                return;

            Log.Info("Updating trigger for {0}.", this);

            foreach (var messageTrigger in actionMessage.Source.Triggers)
            {
                if (!messageTrigger.Message.Equals(actionMessage))
                    continue;

                if (forceDisabled)
                {
                    messageTrigger.UpdateAvailabilty(false);
                    return;
                }

                bool isAvailable = ShouldTriggerBeAvailable(actionMessage, handlingNode);
                messageTrigger.UpdateAvailabilty(isAvailable);

                return;
            }
        }
    }
}