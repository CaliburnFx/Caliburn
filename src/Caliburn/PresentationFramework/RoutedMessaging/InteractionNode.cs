namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System.Collections.Generic;
    using System.Windows;
    using Core;
    using Core.Logging;
    using Triggers;
    using Views;

    /// <summary>
    /// Represents a node within the interaction hierarchy.
    /// </summary>
    public class InteractionNode : IInteractionNode
    {
        static readonly ILog Log = LogManager.GetLog(typeof(InteractionNode));

        readonly IRoutedMessageController controller;
        readonly DependencyObject uiElement;
        IRoutedMessageHandler messageHandler;
        List<IMessageTrigger> triggers;
        bool elementIsLoaded;

        /// <summary>
        /// Gets the action target.
        /// </summary>
        /// <value>The action target.</value>
        public IRoutedMessageHandler MessageHandler
        {
            get { return messageHandler; }
        }

        /// <summary>
        /// Gets the triggers.
        /// </summary>
        /// <value>The triggers.</value>
        public IEnumerable<IMessageTrigger> Triggers
        {
            get { return triggers; }
        }

        /// <summary>
        /// Gets the UI element.
        /// </summary>
        /// <value>The UI element.</value>
        public DependencyObject UIElement
        {
            get { return uiElement; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionNode"/> class.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="controller">The routed message controller.</param>
        public InteractionNode(DependencyObject uiElement, IRoutedMessageController controller)
        {
            this.controller = controller;
            this.uiElement = uiElement;

            elementIsLoaded = View.ExecuteOnLoad(uiElement, Element_Loaded);
        }

        /// <summary>
        /// Finds the parent of this node.
        /// </summary>
        /// <returns>The parent or null if not found.</returns>
        public IInteractionNode FindParent()
        {
            return uiElement != null ? controller.GetParent(uiElement) : null;
        }

        /// <summary>
        /// Determines whether this node can handle the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public bool Handles(IRoutedMessage message)
        {
            return messageHandler != null && messageHandler.Handles(message);
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">An object that provides additional context for message processing.</param>
        public void ProcessMessage(IRoutedMessage message, object context)
        {
            FindHandlerOrFail(message, true).Process(message, context);
        }

        /// <summary>
        /// Updates the availability of the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public void UpdateAvailability(IMessageTrigger trigger)
        {
            if(!elementIsLoaded) 
                return;

            FindHandlerOrFail(trigger.Message, true).UpdateAvailability(trigger);
        }

        /// <summary>
        /// Sets the message handler for this node.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        public void RegisterHandler(IRoutedMessageHandler messageHandler)
        {
            this.messageHandler = messageHandler;
            this.messageHandler.Initialize(this);
            Log.Info("Handler {0} registered.", messageHandler);
        }

        /// <summary>
        /// Adds the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public void AddTrigger(IMessageTrigger trigger)
        {
            if(triggers == null)
                triggers = new List<IMessageTrigger>();

            trigger.Attach(this);
            triggers.Add(trigger);

            if(elementIsLoaded)
                PrepareTrigger(trigger);

            Log.Info("Trigger {0} attached.", trigger);
        }

        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            if (triggers == null)
            {
                elementIsLoaded = true;
                return;
            }

            foreach(var messageTrigger in triggers)
            {
                PrepareTrigger(messageTrigger);
            }

            elementIsLoaded = true;
        }

        void PrepareTrigger(IMessageTrigger messageTrigger) {
            var handler = FindHandlerOrFail(messageTrigger.Message, false);
            if (handler != null) 
                handler.MakeAwareOf(messageTrigger);

            if (!elementIsLoaded)
                return;

            var eventTrigger = messageTrigger as EventMessageTrigger;
            if (eventTrigger != null && eventTrigger.EventName == "Loaded")
                ProcessMessage(eventTrigger.Message, new RoutedEventArgs());
        }

        private IRoutedMessageHandler FindHandlerOrFail(IRoutedMessage message, bool shouldFail)
        {
            IInteractionNode currentNode = this;

            while(currentNode != null && !currentNode.Handles(message))
            {
                currentNode = currentNode.FindParent();
            }

            if(currentNode == null)
            {
                foreach(var handler in message.GetDefaultHandlers(this))
                {
                    if (handler.Handles(message))
                    {
                        RegisterHandler(handler);
                        return handler;
                    }
                }

                var info = string.Format(
                    "There was no handler found for the message {0}.",
                    message
                    );

                if (shouldFail)
                {
                    var exception = new CaliburnException(info);
                    Log.Error(exception);
                    throw exception;
                }
                
                Log.Info(info);
            }

            return currentNode != null ? currentNode.MessageHandler : null;
        }
    }
}