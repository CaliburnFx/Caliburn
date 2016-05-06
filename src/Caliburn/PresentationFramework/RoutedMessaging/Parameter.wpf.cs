#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Markup;
    using Core.Invocation;

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

        Func<object> updater;
        bool updating;

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
                if (updater != null && !updating)
                {
                    updating = true;
                    Value = updater();
                    updating = false;
                }

                return GetValue(ValueProperty);
            }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        public event Action ValueChanged = delegate { };

        static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (e.NewValue != null && e.NewValue.GetType().Name.Equals("NamedObject"))
                    return;

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
            this.updater = updater;
            EventHelper.WireEvent(source, eventInfo, OnUpdate);
        }

        /// <summary>
        /// Called when the parameter value needs to update.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void OnUpdate(object sender, EventArgs e)
        {
            Value = updater();
        }
    }
}

#endif