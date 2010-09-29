namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Windows;
    using RoutedMessaging;
    using RoutedMessaging.Triggers;

    /// <summary>
    /// The default implementation of <see cref="IElementConvention"/>.
    /// </summary>
    /// <typeparam name="T">The type of element the convention applies to.</typeparam>
    public class DefaultElementConvention<T> : IElementConvention
        where T : DependencyObject
    {
        private readonly string defaultEventName;
        private readonly Action<T, object> setter;
        private readonly Func<T, object> getter;
        private readonly DependencyProperty bindableProperty;
        private readonly Func<T, DependencyProperty, DependencyProperty> ensureBindableProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultElementConvention&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="defaultEventName">Default name of the event.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="getter">The getter.</param>
        /// <param name="bindableProperty">The bindable property.</param>
        /// <param name="ensureBindableProperty">Custom logic for determining whether the bindable property should be replaced by another.</param>
        public DefaultElementConvention(string defaultEventName, DependencyProperty bindableProperty, Action<T, object> setter, Func<T, object> getter, Func<T, DependencyProperty, DependencyProperty> ensureBindableProperty)
        {
            this.defaultEventName = defaultEventName;
            this.bindableProperty = bindableProperty;
            this.setter = setter;
            this.getter = getter;
            this.ensureBindableProperty = ensureBindableProperty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultElementConvention&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="defaultEventName">Default name of the event.</param>
        /// <param name="bindableProperty">The bindable property.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="getter">The getter.</param>
        public DefaultElementConvention(string defaultEventName, DependencyProperty bindableProperty, Action<T, object> setter, Func<T, object> getter)
            : this(defaultEventName, bindableProperty, setter, getter, null) {}

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
            get { return bindableProperty; }
        }

        /// <summary>
        /// Gets the name of the default event.
        /// </summary>
        /// <value>The name of the event.</value>
        public string EventName
        {
            get { return defaultEventName; }
        }

        /// <summary>
        /// Gets the default trigger.
        /// </summary>
        /// <returns></returns>
        /// <value>The default trigger.</value>
        public IMessageTrigger CreateTrigger()
        {
            return new EventMessageTrigger {EventName = defaultEventName};
        }

        /// <summary>
        /// Inspects the element instance to confirm that the bindable property is correct.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public DependencyProperty EnsureBindableProperty(DependencyObject element, DependencyProperty property)
        {
            return ensureBindableProperty != null
                ? ensureBindableProperty((T)element, property)
                : property;
        }

        /// <summary>
        /// Gets the default value from the target.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns>The value.</returns>
        public object GetValue(DependencyObject target)
        {
            return getter((T)target);
        }

        /// <summary>
        /// Sets the default value on the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public void SetValue(DependencyObject target, object value)
        {
            setter((T)target, value);
        }
    }
}