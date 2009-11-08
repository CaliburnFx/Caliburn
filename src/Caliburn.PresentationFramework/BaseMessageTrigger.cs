namespace Caliburn.PresentationFramework
{
    using System.Windows;
    using System.Windows.Markup;

#if SILVERLIGHT
    using System.Windows.Controls;
#endif

    /// <summary>
    /// A base class to ease the implementation of <see cref="IMessageTrigger"/>.
    /// </summary>
#if !SILVERLIGHT
    [ContentProperty("Message")]
    public abstract class BaseMessageTrigger : Freezable, IMessageTrigger
    {
#else
    [ContentProperty("Message")]
    public abstract class BaseMessageTrigger : Control, IMessageTrigger
    {
#endif
        /// <summary>
        /// A dependency property used to store the message sent by the trigger.
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                "Message",
                typeof(IRoutedMessage),
                typeof(BaseMessageTrigger),
                null
                );

        private IInteractionNode _node;

        /// <summary>
        /// Gets the node within the interaction hierarchy that this trigger is a part of.
        /// </summary>
        /// <value>The node.</value>
        public IInteractionNode Node
        {
            get { return _node; }
        }

        /// <summary>
        /// Gets the action details that this trigger will send.
        /// </summary>
        /// <value>The action details.</value>
        public IRoutedMessage Message
        {
            get { return (IRoutedMessage)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value);}
        }

        /// <summary>
        /// Wires the trigger into the interaction hierarchy.
        /// </summary>
        /// <param name="node">The node.</param>
        public virtual void Attach(IInteractionNode node)
        {
            _node = node;

            if (Message != null)
            {
                Message.Initialize(node);
                Message.Invalidated += () => Node.UpdateAvailability(this);
            }
        }

        /// <summary>
        /// Updates the UI to reflect the availabilty of the trigger.
        /// </summary>
        /// <param name="isAvailable">if set to <c>true</c> [is available].</param>
        public virtual void UpdateAvailabilty(bool isAvailable)
        {
            var effect = Message.AvailabilityEffect ?? PresentationFramework.Message.GetAvailabilityEffect(Node.UIElement);
            effect.ApplyTo(Node.UIElement, isAvailable);
        }
    }
}