namespace Caliburn.PresentationFramework
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// An implementation of <see cref="IRoutedMessageController"/>.
    /// </summary>
    public class DefaultRoutedMessageController : IRoutedMessageController
    {
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
            {
                var unwrappedValue = handler.Unwrap();

                var frameworkElement = uiElement as FrameworkElement;
                if(frameworkElement != null)
                    frameworkElement.DataContext = unwrappedValue;
#if !SILVERLIGHT
                else
                {
                    var frameworkContentElement = uiElement as FrameworkContentElement;
                    if(frameworkContentElement != null)
                        frameworkContentElement.DataContext = unwrappedValue;
                }
#endif
            }
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
#if !SILVERLIGHT
                currentElement = LogicalTreeHelper.GetParent(currentElement) ??
                                 VisualTreeHelper.GetParent(currentElement);
#else
                currentElement = VisualTreeHelper.GetParent(currentElement);
#endif
                if(currentElement != null)
                    currentNode = currentElement.GetValue(NodeProperty) as IInteractionNode;
            }

            return currentNode;
        }

        private IInteractionNode FindOrAddNode(DependencyObject uiElement)
        {
            var node = uiElement.GetValue(NodeProperty) as IInteractionNode;

            if(node == null)
            {
                node = new InteractionNode(uiElement, this);
                uiElement.SetValue(NodeProperty, node);
            }

            return node;
        }
    }
}