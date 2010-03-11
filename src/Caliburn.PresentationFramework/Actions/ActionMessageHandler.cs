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
        private static readonly ILog Log = LogManager.GetLog(typeof(ActionMessageHandler));

        private readonly object _target;
        private readonly IActionHost _host;
        private IInteractionNode _node;
        private List<object> _metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionMessageHandler"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="target">The target.</param>
        public ActionMessageHandler(IActionHost host, object target)
        {
            _target = target;
            _host = host;

            _host.Actions.SelectMany(x => x.Filters.HandlerAware)
                .Union(_host.Filters.HandlerAware)
                .Apply(x => x.MakeAwareOf(this));
        }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>The host.</value>
        public IActionHost Host
        {
            get { return _host; }
        }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public IList<object> Metadata
        {
            get
            {
                if (_metadata == null)
                    _metadata = new List<object>();

                return _metadata;
            }
        }

        /// <summary>
        /// Initializes this handler on the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void Initialize(IInteractionNode node)
        {
            _node = node;
        }

        /// <summary>
        /// Gets the data context value.
        /// </summary>
        /// <returns></returns>
        public object Unwrap()
        {
            return _target;
        }

        /// <summary>
        /// Determines whethyer the target can handle the specified action.
        /// </summary>
        /// <param name="message">The action details.</param>
        /// <returns></returns>
        public bool Handles(IRoutedMessage message)
        {
            var actionMessage = message as ActionMessage;
            return actionMessage != null && _host.GetAction(actionMessage) != null;
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
                _host.GetAction(actionMessage).Execute(actionMessage, _node, context);
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

            var action = _host.GetAction(actionMessage);
            if(!action.HasTriggerEffects()) return;

            bool isAvailable = action.ShouldTriggerBeAvailable(actionMessage, _node);
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

            var action = _host.GetAction(actionMessage);

            if(action.HasTriggerEffects())
            {
                bool isAvailable = action.ShouldTriggerBeAvailable(actionMessage, _node);
                trigger.UpdateAvailabilty(isAvailable);
            }

            Log.Info("Making handlers aware of {0}.", trigger);
            action.Filters.HandlerAware.Apply(x => x.MakeAwareOf(this, trigger));
        }
    }
}