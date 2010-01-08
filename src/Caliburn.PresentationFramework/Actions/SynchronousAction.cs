namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using Core.Invocation;
    using Filters;

    /// <summary>
    /// A synchronous <see cref="IAction"/>.
    /// </summary>
    public class SynchronousAction : ActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronousAction"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="messageBinder">The method binder.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="blockInteraction">if set to <c>true</c> blocks interaction.</param>
        public SynchronousAction(IMethod method, IMessageBinder messageBinder, IFilterManager filters, bool blockInteraction)
            : base(method, messageBinder, filters, blockInteraction) {}

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
                TryUpdateTrigger(actionMessage, handlingNode, true);

                var parameters = _messageBinder.DetermineParameters(
                    actionMessage,
                    _requirements,
                    handlingNode,
                    context
                    );

                foreach (var filter in _filters.PreProcessors)
                {
                    if (filter.Execute(actionMessage, handlingNode, parameters))
                        continue;

                    TryUpdateTrigger(actionMessage, handlingNode, false);
                    return;
                }

                var outcome = new MessageProcessingOutcome(
                    _method.Invoke(handlingNode.MessageHandler.Unwrap(), parameters),
                    _method.Info.ReturnType,
                    false
                    );

                foreach (var filter in _filters.PostProcessors)
                {
                    filter.Execute(actionMessage, handlingNode, outcome);
                }

                HandleOutcome(actionMessage, handlingNode, outcome);
            }
            catch (Exception ex)
            {
                TryUpdateTrigger(actionMessage, handlingNode, false);
                if(!TryApplyRescue(actionMessage, handlingNode, ex))
                    throw;
                OnCompleted();
            }
        }

        private void HandleOutcome(ActionMessage message, IInteractionNode handlingNode, MessageProcessingOutcome outcome)
        {
            var result = _messageBinder.CreateResult(outcome);

            result.Completed += (r, ex) =>{
                TryUpdateTrigger(message, handlingNode, false);

                if(ex != null)
                {
                    if(!TryApplyRescue(message, handlingNode, ex))
                        throw ex;
                }

                OnCompleted();
            };

            result.Execute(message, handlingNode);
        }
    }
}