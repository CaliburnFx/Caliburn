#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.Triggers
{
    using System.Windows;
    using Core;

    /// <summary>
    /// A message trigger that triggers by routed/attached events.
    /// </summary>
    public class AttachedEventMessageTrigger : BaseMessageTrigger
    {
        /// <summary>
        /// A dependency property representing the routed event's value.
        /// </summary>
        public static readonly DependencyProperty RoutedEventProperty =
            DependencyProperty.Register(
                "RoutedEvent",
                typeof(RoutedEvent),
                typeof(AttachedEventMessageTrigger)
                );

        /// <summary>
        /// Gets or sets the routed event.
        /// </summary>
        /// <value>The routed event.</value>
        public RoutedEvent RoutedEvent
        {
            get { return (RoutedEvent)GetValue(RoutedEventProperty); }
            set { SetValue(RoutedEventProperty, value); }
        }

        /// <summary>
        /// Wires the trigger into the interaction hierarchy.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Attach(IInteractionNode node)
        {
            var element = node.UIElement as UIElement;

            if(element != null)
                element.AddHandler(RoutedEvent, new RoutedEventHandler(Event_Occurred));
            else
            {
                throw new CaliburnException(
                    string.Format(
                        "You cannot use a RoutedEventMessageTrigger with an instance of {0}.  The source element must inherit from UIElement.",
                        node.UIElement.GetType().FullName
                        )
                    );
            }

            base.Attach(node);
        }

        private void Event_Occurred(object sender, RoutedEventArgs e)
        {
            Node.ProcessMessage(Message, e);
        }

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable"/> derived class.
        /// </summary>
        /// <returns>The new instance.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new AttachedEventMessageTrigger();
        }
    }
}

#endif