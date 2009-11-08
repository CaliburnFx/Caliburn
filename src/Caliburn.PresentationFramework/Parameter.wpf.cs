#if !SILVERLIGHT

namespace Caliburn.PresentationFramework
{
    using System;
    using System.Windows;
    using System.Windows.Markup;

    /// <summary>
    /// Represents a parameter of a message.
    /// </summary>
    [ContentProperty("Value")]
    public class Parameter : Freezable
    {
        /// <summary>
        /// A dependency property representing the parameter's value.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(object),
                typeof(Parameter),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                              OnValueChanged)
                );

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        public Parameter() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="value">The value of the parameter.</param>
        public Parameter(object value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        public event Action ValueChanged = delegate { };

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue != e.OldValue)
            {
                var parameter = (Parameter)d;
                parameter.ValueChanged();
            }
        }

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable"/> derived class.
        /// </summary>
        /// <returns>The new instance.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new Parameter();
        }
    }
}

#endif