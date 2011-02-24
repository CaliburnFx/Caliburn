#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.RoutedMessaging.Triggers
{
    using System.Windows;
    using Core;
    using Core.Logging;
    using RoutedMessaging;

    /// <summary>
    /// A message trigger that triggers by routed/attached events.
    /// </summary>
    public class AttachedEventMessageTrigger : BaseMessageTrigger
    {
        static readonly ILog Log = LogManager.GetLog(typeof(AttachedEventMessageTrigger));

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
                var ex = new CaliburnException(
                    string.Format(
                        "You cannot use a RoutedEventMessageTrigger with an instance of {0}.  The source element must inherit from UIElement.",
                        node.UIElement.GetType().FullName
                        )
                    );

                Log.Error(ex);
                throw ex;
            }

            base.Attach(node);
        }

        void Event_Occurred(object sender, RoutedEventArgs e)
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

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "AttachedEventMessageTrigger(" + RoutedEvent.Name + ")";
        }
    }
}

#endif