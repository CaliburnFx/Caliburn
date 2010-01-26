namespace Caliburn.PresentationFramework
{
    using System.Collections.Generic;
    using System.Windows;
    using Core;

    /// <summary>
    /// Represents a node within the interaction hierarchy.
    /// </summary>
    public class InteractionNode : IInteractionNode
    {
        private readonly IRoutedMessageController _controller;
        private readonly DependencyObject _uiElement;
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
        public IEnumerable<IMessageTrigger> Triggers
        {
            get { return _triggers; }
        }

        /// <summary>
        /// Gets the UI element.
        /// </summary>
        /// <value>The UI element.</value>
        public DependencyObject UIElement
        {
            get { return _uiElement; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionNode"/> class.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="controller">The routed message controller.</param>
        public InteractionNode(DependencyObject uiElement, IRoutedMessageController controller)
        {
            _controller = controller;
            _uiElement = uiElement;

#if !SILVERLIGHT
            var element = _uiElement as FrameworkElement;
            if (element != null)
            {
                if (element.IsLoaded)
                    Element_Loaded(element, new RoutedEventArgs());
                else element.Loaded += Element_Loaded;
            }
            else
            {
                var fce = _uiElement as FrameworkContentElement;
                if (fce != null)
                {
                    if (fce.IsLoaded)
                        Element_Loaded(fce, new RoutedEventArgs());
                    else fce.Loaded += Element_Loaded;
                }
            }
#else
            var element = _uiElement as FrameworkElement;
            if(element != null) element.Loaded += Element_Loaded;
#endif
        }

        /// <summary>
        /// Finds the parent of this node.
        /// </summary>
        /// <returns>The parent or null if not found.</returns>
        public IInteractionNode FindParent()
        {
            return _uiElement != null ? _controller.GetParent(_uiElement) : null;
        }

        /// <summary>
        /// Determines whether this node can handle the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public bool Handles(IRoutedMessage message)
        {
            return _messageHandler != null && _messageHandler.Handles(message);
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
            if(!_elementIsLoaded) 
                return;

            FindHandlerOrFail(trigger.Message, true).UpdateAvailability(trigger);
        }

        /// <summary>
        /// Sets the message handler for this node.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        public void RegisterHandler(IRoutedMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
            _messageHandler.Initialize(this);
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

            if(_triggers == null) 
                return;

            foreach(var messageTrigger in _triggers)
            {
                var Handler = FindHandlerOrFail(messageTrigger.Message, false);
                if (Handler != null) 
                    Handler.MakeAwareOf(messageTrigger);
            }
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

                if (shouldFail)
                {
                    throw new CaliburnException(
                        string.Format(
                            "There was no handler found for the message {0}.",
                            message
                            )
                        );
                }
            }

            return currentNode != null ? currentNode.MessageHandler : null;
        }
    }
}