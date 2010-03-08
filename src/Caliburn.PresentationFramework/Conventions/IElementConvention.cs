namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Windows;
    using RoutedMessaging;

    /// <summary>
    /// Represents an element's conventions.
    /// </summary>
    public interface IElementConvention
    {
        /// <summary>
        /// Gets the type of the element to which the conventions apply.
        /// </summary>
        /// <value>The type of the element.</value>
        Type Type { get; }

        /// <summary>
        /// Gets the default trigger.
        /// </summary>
        /// <value>The default trigger.</value>
        IMessageTrigger CreateTrigger();

        /// <summary>
        /// Gets the name of the default event.
        /// </summary>
        /// <value>The name of the event.</value>
        string EventName { get; }

        /// <summary>
        /// Gets the default property used in databinding.
        /// </summary>
        /// <value>The bindable property.</value>
        DependencyProperty BindableProperty { get; }

        /// <summary>
        /// Inspects the element instance to confirm that the bindable property is correct.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        DependencyProperty EnsureBindableProperty(DependencyObject element, DependencyProperty property);

        /// <summary>
        /// Gets the default value for the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The value.</returns>
        object GetValue(DependencyObject element);

        /// <summary>
        /// Sets the default value on the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        void SetValue(DependencyObject element, object value);
    }
}