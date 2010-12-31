namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using Core.Invocation;
    using Core.InversionOfControl;
    using Core.Logging;
    using Filters;
    using RoutedMessaging;

    /// <summary>
    /// A synchronous <see cref="IAction"/>.
    /// </summary>
    public class SynchronousAction : ActionBase
    {
        static readonly ILog Log = LogManager.GetLog(typeof(SynchronousAction));

        readonly IServiceLocator serviceLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronousAction"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="method">The method.</param>
        /// <param name="messageBinder">The method binder.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="blockInteraction">if set to <c>true</c> blocks interaction.</param>
        public SynchronousAction(IServiceLocator serviceLocator, IMethod method, IMessageBinder messageBinder, IFilterManager filters, bool blockInteraction)
            : base(method, messageBinder, filters, blockInteraction)
        {
            this.serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Executes the specified this action on the specified target.
        /// </summary>
        /// <param name="actionMessage">The action message.</param>
        /// <param name="handlingNode">The node.</param>
        /// <param name="context">The context.</param>
        public override void Execute(ActionMessage actionMessage, IInteractionNode handlingNode, object context)
        {
            try
            {
                var parameters = MessageBinder.DetermineParameters(
                    actionMessage,
                    UnderlyingRequirements,
                    handlingNode,
                    context
                    );

                TryUpdateTrigger(actionMessage, handlingNode, true);

                foreach (var filter in UnderlyingFilters.PreProcessors)
                {
                    if (filter.Execute(actionMessage, handlingNode, parameters))
                        continue;

                    TryUpdateTrigger(actionMessage, handlingNode, false);
                    return;
                }

                var outcome = new MessageProcessingOutcome(
                    UnderlyingMethod.Invoke(handlingNode.MessageHandler.Unwrap(), parameters),
                    UnderlyingMethod.Info.ReturnType,
                    false
                    );

                foreach (var filter in UnderlyingFilters.PostProcessors)
                {
                    filter.Execute(actionMessage, handlingNode, outcome);
                }

                HandleOutcome(actionMessage, handlingNode, outcome);
            }
            catch (Exception ex)
            {
                TryUpdateTrigger(actionMessage, handlingNode, false);
                if(!TryApplyRescue(actionMessage, handlingNode, ex))
                {
                    Log.Error(ex);
                    throw;
                }
                OnCompleted();
            }
        }

        private void HandleOutcome(ActionMessage message, IInteractionNode handlingNode, MessageProcessingOutcome outcome)
        {
            var result = MessageBinder.CreateResult(outcome);

            result.Completed += (s, e) =>{
                TryUpdateTrigger(message, handlingNode, false);

                if(e.Error != null)
                {
                    if(!TryApplyRescue(message, handlingNode, e.Error))
                    {
                        Log.Error(e.Error);
                        throw e.Error;
                    }
                }

                OnCompleted();
            };

            result.Execute(new ResultExecutionContext(serviceLocator, message, handlingNode));
        }
    }
}