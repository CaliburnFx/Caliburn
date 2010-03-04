namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Windows;
    using Core.Invocation;
    using Microsoft.Practices.ServiceLocation;
    using RoutedMessaging;
    using RoutedMessaging.Triggers;

    /// <summary>
    /// The default implementation of <see cref="IElementConvention"/>.
    /// </summary>
    /// <typeparam name="T">The type of element the convention applies to.</typeparam>
    public class DefaultElementConvention<T> : IElementConvention
        where T : DependencyObject
    {
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly string _defaultEventName;
        private readonly Action<T, object> _setter;
        private readonly Func<T, object> _getter;
        private readonly DependencyProperty _bindableProperty;
        private readonly Func<T, bool> _shouldOverrideBindableProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultElementConvention&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="eventHandlerFactory">The event handler factory.</param>
        /// <param name="defaultEventName">Default name of the event.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="getter">The getter.</param>
        /// <param name="bindableProperty">The bindable property.</param>
        /// <param name="shouldOverrideBindableProperty">Custom logic for determining whether the bindable property should be replaced by View.Model.</param>
        public DefaultElementConvention(IEventHandlerFactory eventHandlerFactory, string defaultEventName, DependencyProperty bindableProperty, Action<T, object> setter, Func<T, object> getter, Func<T, bool> shouldOverrideBindableProperty)
        {
            _eventHandlerFactory = eventHandlerFactory;
            _defaultEventName = defaultEventName;
            _bindableProperty = bindableProperty;
            _setter = setter;
            _getter = getter;
            _shouldOverrideBindableProperty = shouldOverrideBindableProperty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultElementConvention&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="defaultEventName">Default name of the event.</param>
        /// <param name="bindableProperty">The bindable property.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="getter">The getter.</param>
        /// <param name="shouldOverrideBindableProperty">Custom logic for determining whether the bindable property should be replaced by View.Model.</param>
        public DefaultElementConvention(string defaultEventName, DependencyProperty bindableProperty, Action<T, object> setter, Func<T, object> getter, Func<T, bool> shouldOverrideBindableProperty)
            : this(ServiceLocator.Current.GetInstance<IEventHandlerFactory>(), defaultEventName, bindableProperty, setter, getter, shouldOverrideBindableProperty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultElementConvention&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="defaultEventName">Default name of the event.</param>
        /// <param name="bindableProperty">The bindable property.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="getter">The getter.</param>
        public DefaultElementConvention(string defaultEventName, DependencyProperty bindableProperty, Action<T, object> setter, Func<T, object> getter)
            : this(ServiceLocator.Current.GetInstance<IEventHandlerFactory>(), defaultEventName, bindableProperty, setter, getter, null) {}

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
        /// Inidicates whether or not the BindableProperty should be overriden by the View.Model property.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public bool ShouldOverrideBindablePropertyWithViewModel(DependencyObject element)
        {
            return _shouldOverrideBindableProperty != null && _shouldOverrideBindableProperty((T)element);
        }

        /// <summary>
        /// Gets the default value from the target.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns>The value.</returns>
        public object GetValue(DependencyObject target)
        {
            return _getter((T)target);
        }

        /// <summary>
        /// Sets the default value on the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public void SetValue(DependencyObject target, object value)
        {
            _setter((T)target, value);
        }
    }
}