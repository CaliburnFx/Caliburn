#if !SILVERLIGHT

namespace Caliburn.PresentationFramework
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Markup;
    using Core.Invocation;
    using Microsoft.Practices.ServiceLocation;

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

        private Func<object> _updater;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        public Parameter() { }

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
            get
            {
                if (_updater != null)
                    Value = _updater();

                return GetValue(ValueProperty);
            }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        public event Action ValueChanged = delegate { };

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
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

        /// <summary>
        /// Wires the parameter for value updates.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="eventInfo">The event info.</param>
        /// <param name="updater">The updater.</param>
        public void Wire(object source, EventInfo eventInfo, Func<object> updater)
        {
            ServiceLocator.Current.GetInstance<IEventHandlerFactory>().Wire(source, eventInfo)
                .SetActualHandler(parameters =>
                {
                    Value = updater();
                });

            _updater = updater;
        }
    }
}

#endif