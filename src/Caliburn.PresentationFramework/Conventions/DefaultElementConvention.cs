namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Windows;
    using Core.Invocation;
    using Microsoft.Practices.ServiceLocation;
    using Triggers;

    /// <summary>
    /// The default implementation of <see cref="IElementConvention"/>.
    /// </summary>
    /// <typeparam name="T">The type of element the convention applies to.</typeparam>
    public class DefaultElementConvention<T> : IElementConvention
    {
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly string _defaultEventName;
        private readonly Action<T, object> _setter;
        private readonly Func<T, object> _getter;
        private readonly DependencyProperty _bindableProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultElementConvention&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="eventHandlerFactory">The event handler factory.</param>
        /// <param name="defaultEventName">Default name of the event.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="getter">The getter.</param>
        /// <param name="bindableProperty">The bindable property.</param>
        public DefaultElementConvention(IEventHandlerFactory eventHandlerFactory, string defaultEventName, DependencyProperty bindableProperty, Action<T, object> setter, Func<T, object> getter)
        {
            _eventHandlerFactory = eventHandlerFactory;
            _defaultEventName = defaultEventName;
            _bindableProperty = bindableProperty;
            _setter = setter;
            _getter = getter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultElementConvention&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="defaultEventName">Default name of the event.</param>
        /// <param name="bindableProperty">The bindable property.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="getter">The getter.</param>
        public DefaultElementConvention(string defaultEventName, DependencyProperty bindableProperty, Action<T, object> setter, Func<T, object> getter)
            : this(ServiceLocator.Current.GetInstance<IEventHandlerFactory>(), defaultEventName, bindableProperty, setter, getter) {}

        /// <summary>
        /// Gets the type of the element to which the conventions apply.
        /// </summary>
        /// <value>The type of the element.</value>
        public Type Type
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Gets the default property used in databinding.
        /// </summary>
        /// <value>The bindable property.</value>
        public DependencyProperty BindableProperty
        {
            get { return _bindableProperty; }
        }

        /// <summary>
        /// Gets the name of the default event.
        /// </summary>
        /// <value>The name of the event.</value>
        public string EventName
        {
            get { return _defaultEventName; }
        }

        /// <summary>
        /// Gets the default trigger.
        /// </summary>
        /// <returns></returns>
        /// <value>The default trigger.</value>
        public IMessageTrigger CreateTrigger()
        {
            return new EventMessageTrigger(_eventHandlerFactory) {EventName = _defaultEventName};
        }

        /// <summary>
        /// Gets the default value from the target.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns>The value.</returns>
        public object GetValue(object target)
        {
            return _getter((T)target);
        }

        /// <summary>
        /// Sets the default value on the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public void SetValue(object target, object value)
        {
            _setter((T)target, value);
        }
    }
}