namespace Caliburn.PresentationFramework
{
    using System;
    using Core.Invocation;
    using Microsoft.Practices.ServiceLocation;
    using Triggers;

    /// <summary>
    /// An implamentation of <see cref="InteractionDefaults"/>.
    /// </summary>
    /// <typeparam name="T">The type of element these defaults apply to.</typeparam>
    public class GenericInteractionDefaults<T> : InteractionDefaults
    {
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly Action<T, object> _setter;
        private readonly Func<T, object> _getter;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericInteractionDefaults&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="eventHandlerFactory">The event handler factory.</param>
        /// <param name="defaultEventName">Default name of the event.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="getter">The getter.</param>
        public GenericInteractionDefaults(IEventHandlerFactory eventHandlerFactory, string defaultEventName,
                                          Action<T, object> setter, Func<T, object> getter)
            : base(typeof(T), defaultEventName)
        {
            _eventHandlerFactory = eventHandlerFactory;
            _setter = setter;
            _getter = getter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericInteractionDefaults&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="defaultEventName">Default name of the event.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="getter">The getter.</param>
        public GenericInteractionDefaults(string defaultEventName, Action<T, object> setter, Func<T, object> getter)
            : this(ServiceLocator.Current.GetInstance<IEventHandlerFactory>(), defaultEventName, setter, getter) {}

        /// <summary>
        /// Gets the default trigger.
        /// </summary>
        /// <returns></returns>
        /// <value>The default trigger.</value>
        public override IMessageTrigger GetDefaultTrigger()
        {
            return new EventMessageTrigger(_eventHandlerFactory) {EventName = DefaultEventName};
        }

        /// <summary>
        /// Gets the default value from the target.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns>The value.</returns>
        public override object GetDefaultValue(object target)
        {
            return _getter((T)target);
        }

        /// <summary>
        /// Sets the default value on the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public override void SetDefaultValue(object target, object value)
        {
            _setter((T)target, value);
        }
    }
}