namespace Caliburn.PresentationFramework.Actions
{
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Core.Logging;
    using RoutedMessaging;

    /// <summary>
    /// An implementation of <see cref="IRoutedMessageController"/> for action messages.
    /// </summary>
    public class ActionMessageHandler : IRoutedMessageHandler
    {
        static readonly ILog Log = LogManager.GetLog(typeof(ActionMessageHandler));

        readonly object target;
        readonly IActionHost host;
        IInteractionNode node;
        List<object> metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionMessageHandler"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="target">The target.</param>
        public ActionMessageHandler(IActionHost host, object target)
        {
            this.target = target;
            this.host = host;

            this.host.Actions.SelectMany(x => x.Filters.HandlerAware)
                .Union(this.host.Filters.HandlerAware)
                .Apply(x => x.MakeAwareOf(this));
        }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>The host.</value>
        public IActionHost Host
        {
            get { return host; }
        }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public IList<object> Metadata
        {
            get
            {
                if (metadata == null)
                    metadata = new List<object>();

                return metadata;
            }
        }

        /// <summary>
        /// Initializes this handler on the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void Initialize(IInteractionNode node)
        {
            this.node = node;
        }

        /// <summary>
        /// Gets the data context value.
        /// </summary>
        /// <returns></returns>
        public object Unwrap()
        {
            return target;
        }

        /// <summary>
        /// Determines whethyer the target can handle the specified action.
        /// </summary>
        /// <param name="message">The action details.</param>
        /// <returns></returns>
        public bool Handles(IRoutedMessage message)
        {
            var actionMessage = message as ActionMessage;
            return actionMessage != null && FindActionHandler(actionMessage) != null;
        }

        /// <summary>
        /// Processes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">An object that provides additional context for message processing.</param>
        public void Process(IRoutedMessage message, object context)
        {
            var actionMessage = message as ActionMessage;

            if (actionMessage != null)
            {
                FindActionHandler(actionMessage).Execute(actionMessage, node, context);
                Log.Info("Processed message {0}.", actionMessage);
            }
            else
            {
                var ex = new CaliburnException("The handler cannot process this message.");
                Log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Updates the availability of the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public void UpdateAvailability(IMessageTrigger trigger)
        {
            var actionMessage = trigger.Message as ActionMessage;

            if(actionMessage == null)
            {
                var ex = new CaliburnException("The handler cannot update availability for this trigger.");
                Log.Error(ex);
                throw ex;
            }
            
            Log.Info("Requesting update avaiability for {0}.", actionMessage);

            var action = FindActionHandler(actionMessage);
            bool isAvailable = action.ShouldTriggerBeAvailable(actionMessage, node);
            trigger.UpdateAvailabilty(isAvailable);
        }

        /// <summary>
        /// Makes the handler aware of a specific trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public void MakeAwareOf(IMessageTrigger trigger)
        {
            var actionMessage = trigger.Message as ActionMessage;
            if(actionMessage == null) return;

            var handler = FindActionHandler(actionMessage);
            bool isAvailable = handler.ShouldTriggerBeAvailable(actionMessage, node);
            trigger.UpdateAvailabilty(isAvailable);

            Log.Info("Making handlers aware of {0}.", trigger);

            var action = handler as IAction;
            if(action != null)
                action.Filters.HandlerAware.Apply(x => x.MakeAwareOf(this, trigger));
        }

        IActionHandler FindActionHandler(ActionMessage message)
        {
            return host.GetAction(message) ?? target as IActionHandler;
        }
    }
}