namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using Core.Invocation;
    using Core.Threading;
    using Filters;
	using System.Threading;

    /// <summary>
    /// An asynchronous <see cref="IAction"/>.
    /// </summary>
    public class AsynchronousAction : ActionBase
    {
        [ThreadStatic]
        private static IBackgroundTask _currentTask;

        /// <summary>
        /// Gets or sets the current background task.
        /// </summary>
        /// <value>The current task.</value>
        public static IBackgroundTask CurrentTask
        {
            get { return _currentTask; }
            set { _currentTask = value; }
        }

        private readonly bool _blockInteraction;
		private int _runningCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsynchronousAction"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="messageBinder">The method binder.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="blockInteraction">if set to <c>true</c> [block interaction].</param>
        public AsynchronousAction(IMethod method, IMessageBinder messageBinder, IFilterManager filters, bool blockInteraction)
            : base(method, messageBinder, filters)
        {
            _blockInteraction = blockInteraction;
        }

        /// <summary>
        /// Gets a value indicating whether to block intaction with the trigger during async execution.
        /// </summary>
        /// <value><c>true</c> if should block; otherwise, <c>false</c>.</value>
        public bool BlockInteraction
        {
            get { return _blockInteraction; }
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
                TryUpdateTrigger(actionMessage, handlingNode, true);

                var parameters = _messageBinder.DetermineParameters(
                    actionMessage,
                    _requirements,
                    handlingNode,
                    context
                    );

                CurrentTask = _method.CreateBackgroundTask(handlingNode.MessageHandler.Unwrap(), parameters);

                foreach (var filter in _filters.PreProcessors)
                {
                    if(filter.Execute(actionMessage, handlingNode, parameters)) 
                        continue;

                    TryUpdateTrigger(actionMessage, handlingNode, false);
                    return;
                }

                DoExecute(actionMessage, handlingNode, parameters);
            }
            catch (Exception ex)
            {
                TryUpdateTrigger(actionMessage, handlingNode, false);
                if(!TryApplyRescue(actionMessage, handlingNode, ex))
                    throw;
                OnCompleted();
            }
        }

        /// <summary>
        /// Executes the core logic, specific to the action type.
        /// </summary>
        /// <param name="actionMessage">The action message.</param>
        /// <param name="handlingNode">The node.</param>
        /// <param name="parameters">The parameters.</param>
        protected void DoExecute(ActionMessage actionMessage, IInteractionNode handlingNode, object[] parameters)
        {
            CurrentTask.Completed +=
                (s, e) => Core.Invocation.Execute.OnUIThread(
                              () =>{
								  Interlocked.Decrement(ref _runningCount);
                                  if(e.Error != null)
                                  {
                                      TryUpdateTrigger(actionMessage, handlingNode, false);
                                      if(!TryApplyRescue(actionMessage, handlingNode, e.Error))
                                          throw e.Error;
                                      OnCompleted();
                                  }
                                  else
                                  {
                                      try
                                      {
                                          var outcome = new MessageProcessingOutcome(
                                              e.Result,
                                              _method.Info.ReturnType,
                                              e.Cancelled
                                              );

                                          foreach(var filter in _filters.PostProcessors)
                                          {
                                              filter.Execute(actionMessage, handlingNode, outcome);
                                          }

                                          var result = _messageBinder.CreateResult(outcome);

                                          result.Completed += (r,ex) => {
                                              TryUpdateTrigger(actionMessage, handlingNode, false);

                                              if(ex != null)
                                              {
                                                  if (!TryApplyRescue(actionMessage, handlingNode, ex))
                                                      throw ex;
                                              }
                                              
                                              OnCompleted();
                                          };

                                          result.Execute(actionMessage, handlingNode);
                                      }
                                      catch(Exception ex)
                                      {
                                          TryUpdateTrigger(actionMessage, handlingNode, false);
                                          if (!TryApplyRescue(actionMessage, handlingNode, ex))
                                              throw;
                                          OnCompleted();
                                      }
                                  }
                              });

			Interlocked.Increment(ref _runningCount);
            CurrentTask.Enqueue(this);
            CurrentTask = null;
        }

        /// <summary>
        /// Determines how this instance affects trigger availability.
        /// </summary>
        /// <param name="actionMessage">The action message.</param>
        /// <param name="handlingNode">The node.</param>
        /// <returns>
        /// 	<c>true</c> if this instance enables triggers; otherwise, <c>false</c>.
        /// </returns>
		public override bool ShouldTriggerBeAvailable(ActionMessage actionMessage, IInteractionNode handlingNode)
		{
			if (BlockInteraction && _runningCount > 0) return false;
			return base.ShouldTriggerBeAvailable(actionMessage, handlingNode);
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

            foreach (var messageTrigger in actionMessage.Source.Triggers)
            {
                if (!messageTrigger.Message.Equals(actionMessage))
                    continue;

                if (forceDisabled)
                {
                    messageTrigger.UpdateAvailabilty(false);
                    return;
                }

                if (this.HasTriggerEffects())
                {
                    bool isAvailable = ShouldTriggerBeAvailable(actionMessage, handlingNode);
                    messageTrigger.UpdateAvailabilty(isAvailable);
                }
                else messageTrigger.UpdateAvailabilty(true);

                return;
            }
        }
    }
}