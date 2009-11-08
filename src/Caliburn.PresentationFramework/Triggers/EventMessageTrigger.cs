namespace Caliburn.PresentationFramework.Triggers
{
    using System.Windows;
    using Core.Invocation;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// A message trigger that triggers by standard CLR events.
    /// </summary>
    public class EventMessageTrigger : BaseMessageTrigger
    {
        private IEventHandlerFactory _eventHandlerFactory;

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

        private IEventHandlerFactory EventHandlerFactory
        {
            get
            {
                if(_eventHandlerFactory == null)
                    _eventHandlerFactory = ServiceLocator.Current.GetInstance<IEventHandlerFactory>();

                return _eventHandlerFactory;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventMessageTrigger"/> class.
        /// </summary>
        public EventMessageTrigger() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="EventMessageTrigger"/> class.
        /// </summary>
        /// <param name="eventHandlerFactory">The event handler factory.</param>
        public EventMessageTrigger(IEventHandlerFactory eventHandlerFactory)
        {
            _eventHandlerFactory = eventHandlerFactory;
        }

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
            var handler = EventHandlerFactory.Wire(
                node.UIElement,
                EventName
                );

            handler.SetActualHandler(Event_Occurred);

            base.Attach(node);
        }

        private void Event_Occurred(object[] obj)
        {
            Node.ProcessMessage(Message, obj[1]);
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