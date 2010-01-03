namespace Caliburn.PresentationFramework
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using Core;

    /// <summary>
    /// Represents a node within the interaction hierarchy.
    /// </summary>
    public class InteractionNode : IInteractionNode
    {
        private readonly IRoutedMessageController _controller;
        private readonly DependencyObject _uiElementReference;
        private IRoutedMessageHandler _messageHandler;
        private List<IMessageTrigger> _triggers;
        private bool _elementIsLoaded;

        /// <summary>
        /// Gets the action target.
        /// </summary>
        /// <value>The action target.</value>
        public IRoutedMessageHandler MessageHandler
        {
            get { return _messageHandler; }
        }

        /// <summary>
        /// Gets the triggers.
        /// </summary>
        /// <value>The triggers.</value>
        public ICollection<IMessageTrigger> Triggers
        {
            get { return new ReadOnlyCollection<IMessageTrigger>(_triggers); }
        }

        /// <summary>
        /// Gets the UI element.
        /// </summary>
        /// <value>The UI element.</value>
        public DependencyObject UIElement
        {
            get { return _uiElementReference; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionNode"/> class.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="controller">The routed message controller.</param>
        public InteractionNode(DependencyObject uiElement, IRoutedMessageController controller)
        {
            _controller = controller;
            _uiElementReference = uiElement;
            _uiElementReference.OnLoad(Element_Loaded);
        }

        /// <summary>
        /// Finds the parent of this node.
        /// </summary>
        /// <returns>The parent or null if not found.</returns>
        public IInteractionNode FindParent()
        {
            var element = UIElement;

            if(element != null)
                return _controller.GetParent(element);

            return null;
        }

        /// <summary>
        /// Determines whether this node can handle the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public bool Handles(IRoutedMessage message)
        {
            if(_messageHandler != null)
                return _messageHandler.Handles(message);
            return false;
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">An object that provides additional context for message processing.</param>
        public void ProcessMessage(IRoutedMessage message, object context)
        {
            var handlerNode = FindHandlerNode(message, true);
            handlerNode.MessageHandler.Process(message, context);
        }

        /// <summary>
        /// Updates the availability of the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public void UpdateAvailability(IMessageTrigger trigger)
        {
            if(!_elementIsLoaded) return;

            var handlerNode = FindHandlerNode(trigger.Message, true);
            handlerNode.MessageHandler.UpdateAvailability(trigger);
        }

        /// <summary>
        /// Sets the message handler for this node.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        public void RegisterHandler(IRoutedMessageHandler messageHandler)
        {
            if(messageHandler != null)
            {
                _messageHandler = messageHandler;
                _messageHandler.Initialize(this);
            }
        }

        /// <summary>
        /// Adds the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public void AddTrigger(IMessageTrigger trigger)
        {
            if(_triggers == null)
                _triggers = new List<IMessageTrigger>();

            trigger.Attach(this);

            _triggers.Add(trigger);
        }

        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            _elementIsLoaded = true;

            if(_triggers == null) return;

            foreach(var messageTrigger in _triggers)
            {
                var handlerNode = FindHandlerNode(messageTrigger.Message, false);
                if(handlerNode != null)
                    handlerNode.MessageHandler.MakeAwareOf(messageTrigger);
            }
        }

        private IInteractionNode FindHandlerNode(IRoutedMessage message, bool shouldFail)
        {
            IInteractionNode currentNode = this;

            while(currentNode != null && !currentNode.Handles(message))
            {
                currentNode = currentNode.FindParent();
            }

            if(currentNode == null && shouldFail)
            {
                throw new CaliburnException(
                    string.Format(
                        "There was no handler found for the message {0}.",
                        message
                        )
                    );
            }

            return currentNode;
        }
    }
}