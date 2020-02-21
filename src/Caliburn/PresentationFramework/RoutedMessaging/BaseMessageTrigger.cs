namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System.Windows;
    using System.Windows.Markup;
    using Core.Logging;

    /// <summary>
    /// A base class to ease the implementation of <see cref="IMessageTrigger"/>.
    /// </summary>
    [ContentProperty("Message")]
    public abstract class BaseMessageTrigger : Freezable, IMessageTrigger
    {
        static readonly ILog Log = LogManager.GetLog(typeof(BaseMessageTrigger));

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

        IInteractionNode node;

        /// <summary>
        /// Gets the node within the interaction hierarchy that this trigger is a part of.
        /// </summary>
        /// <value>The node.</value>
        public IInteractionNode Node
        {
            get { return node; }
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
            this.node = node;

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
            var effect = Message.AvailabilityEffect ?? RoutedMessaging.Message.GetAvailabilityEffect(Node.UIElement);
            effect.ApplyTo(Node.UIElement, isAvailable);
            Log.Info("Updated trigger availability {0} on {1} to {2} using {3}.", this, Node.UIElement, isAvailable, effect);
        }
    }
}
