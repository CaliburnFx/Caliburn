namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using Core.Invocation;
    using Core.InversionOfControl;
    using Core.Logging;
    using Filters;
	using System.Threading;
    using RoutedMessaging;

    /// <summary>
    /// An asynchronous <see cref="IAction"/>.
    /// </summary>
    public class AsynchronousAction : ActionBase
    {
        static readonly ILog Log = LogManager.GetLog(typeof(AsynchronousAction));
        readonly IServiceLocator serviceLocator;

        [ThreadStatic]
        static IBackgroundTask currentTask;

        /// <summary>
        /// Gets or sets the current background task.
        /// </summary>
        /// <value>The current task.</value>
        public static IBackgroundTask CurrentTask
        {
            get { return currentTask; }
            set { currentTask = value; }
        }

		int runningCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsynchronousAction"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="method">The method.</param>
        /// <param name="messageBinder">The method binder.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="blockInteraction">if set to <c>true</c> blocks interaction.</param>
        public AsynchronousAction(IServiceLocator serviceLocator, IMethod method, IMessageBinder messageBinder, IFilterManager filters, bool blockInteraction)
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

                CurrentTask = UnderlyingMethod.CreateBackgroundTask(handlingNode.MessageHandler.Unwrap(), parameters);

                foreach (var filter in UnderlyingFilters.PreProcessors)
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
                {
                    Log.Error(ex);
                    throw;
                }
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
                (s, e) => Invocation.Execute.OnUIThread(
                              () =>{
                                  Interlocked.Decrement(ref runningCount);
                                  if(e.Error != null)
                                  {
                                      TryUpdateTrigger(actionMessage, handlingNode, false);
                                      if(!TryApplyRescue(actionMessage, handlingNode, e.Error))
                                      {
                                          Log.Error(e.Error);
                                          throw e.Error;
                                      }
                                      OnCompleted();
                                  }
                                  else
                                  {
                                      try
                                      {
                                          var outcome = new MessageProcessingOutcome(
                                              e.Cancelled ? null : e.Result,
                                              UnderlyingMethod.Info.ReturnType,
                                              e.Cancelled
                                              );

                                          foreach(var filter in UnderlyingFilters.PostProcessors)
                                          {
                                              filter.Execute(actionMessage, handlingNode, outcome);
                                          }

                                          var result = MessageBinder.CreateResult(outcome);

                                          result.Completed += (r, arg) =>{
                                              TryUpdateTrigger(actionMessage, handlingNode, false);

                                              if(arg.Error != null)
                                              {
                                                  if(!TryApplyRescue(actionMessage, handlingNode, arg.Error))
                                                  {
                                                      Log.Error(arg.Error);
                                                      throw arg.Error;
                                                  }
                                              }

                                              OnCompleted();
                                          };

                                          result.Execute(new ResultExecutionContext(serviceLocator, actionMessage, handlingNode));
                                      }
                                      catch(Exception ex)
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
                              });

			Interlocked.Increment(ref runningCount);
            CurrentTask.Start(this);
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
			if (BlockInteraction && runningCount > 1) return false;
			return base.ShouldTriggerBeAvailable(actionMessage, handlingNode);
		}
    }
}