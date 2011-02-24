namespace Caliburn.PresentationFramework.RoutedMessaging.Triggers
{
    using System;
    using System.Windows;
    using Core.Invocation;
    using RoutedMessaging;

    /// <summary>
    /// A message trigger that triggers by standard CLR events.
    /// </summary>
    public class EventMessageTrigger : BaseMessageTrigger
    {
        /// <summary>
        /// A dependency property representing the event name's value.
        /// </summary>
        public static readonly DependencyProperty EventNameProperty =
            DependencyProperty.Register(
                "EventName",
                typeof(string),
                typeof(EventMessageTrigger),
                null
                );

        /// <summary>
        /// Gets or sets the name of the event.
        /// </summary>
        /// <value>The name of the event.</value>
        public string EventName
        {
            get { return (string)GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }

        /// <summary>
        /// Wires the trigger into the interaction hierarchy.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Attach(IInteractionNode node)
        {
            EventHelper.WireEvent(
                node.UIElement,
                node.UIElement.GetType().GetEvent(EventName),
                OnEvent
                );

            base.Attach(node);
        }

        /// <summary>
        /// Called when the event trigger is raised.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void OnEvent(object sender, EventArgs args)
        {
            Node.ProcessMessage(Message, args);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "EventMessageTrigger(" + EventName + ")";
        }

#if !SILVERLIGHT
        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable"/> derived class.
        /// </summary>
        /// <returns>The new instance.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new EventMessageTrigger();
        }
#endif
    }
}