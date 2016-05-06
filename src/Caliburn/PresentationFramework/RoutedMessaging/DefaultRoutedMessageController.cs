namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System.Windows;
    using Core.Logging;

    /// <summary>
    /// An implementation of <see cref="IRoutedMessageController"/>.
    /// </summary>
    public class DefaultRoutedMessageController : IRoutedMessageController
    {
        static readonly ILog Log = LogManager.GetLog(typeof(DefaultRoutedMessageController));

        /// <summary>
        /// Used to maintain the state of the interaction hierarchy.
        /// </summary>
        public static readonly DependencyProperty NodeProperty =
            DependencyProperty.RegisterAttached(
                "Node",
                typeof(IInteractionNode),
                typeof(DefaultRoutedMessageController),
                null
                );

        /// <summary>
        /// Adds a message handler at the specified location in the UI hierarchy.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="handler">The message handler.</param>
        /// <param name="setContext">if set to <c>true</c> the handler will also be stored in the element's DataContext and ViewMetadata will be set.</param>
        public void AddHandler(DependencyObject uiElement, IRoutedMessageHandler handler, bool setContext)
        {
            var node = FindOrAddNode(uiElement);
            node.RegisterHandler(handler);

            if(setContext)
                uiElement.SetDataContext(handler.Unwrap());
        }

        /// <summary>
        /// Attaches the trigger and prepares it to send actions.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="trigger">The trigger.</param>
        public void AttachTrigger(DependencyObject uiElement, IMessageTrigger trigger)
        {
            if(trigger.Message is IRoutedMessageHandler)
            {
                var node = new InteractionNode(uiElement, this);
                node.RegisterHandler(trigger.Message as IRoutedMessageHandler);
                node.AddTrigger(trigger);
            }
            else
            {
                var node = FindOrAddNode(uiElement);
                node.AddTrigger(trigger);
            }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <param name="uiElement">The UI element to retrieve the parent for.</param>
        /// <returns></returns>
        public IInteractionNode GetParent(DependencyObject uiElement)
        {
            DependencyObject currentElement = uiElement;
            IInteractionNode currentNode = null;

            while(currentElement != null && currentNode == null)
            {
                currentElement = currentElement.GetParent();

                if(currentElement != null)
                    currentNode = currentElement.GetValue(NodeProperty) as IInteractionNode;
            }

            return currentNode;
        }

        IInteractionNode FindOrAddNode(DependencyObject uiElement)
        {
            var node = uiElement.GetValue(NodeProperty) as IInteractionNode;

            if(node == null)
            {
                node = new InteractionNode(uiElement, this);
                uiElement.SetValue(NodeProperty, node);
                Log.Info("New interaction node added to hierarchy at {0}.", uiElement);
            }

            return node;
        }
    }
}