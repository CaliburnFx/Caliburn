#if SILVERLIGHT
namespace Caliburn.PresentationFramework
{
    using System;
    using System.Reflection;
    using System.Windows.Markup;
    using System.ComponentModel;
    using Core.Invocation;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Represents a parameter of a message.
    /// </summary>
    [ContentProperty("Value")]
    public class Parameter : INotifyPropertyChanged
    {
        private object _value;
        private Func<object> _updater;
        private bool _updating;

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
                if (_updater != null && !_updating)
                {
                    _updating = true;
                    Value = _updater();
                    _updating = false;
                }

                return _value;
            }
            set
            {
                _value = value; 
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                ValueChanged();
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        public event Action ValueChanged = delegate { };

        /// <summary>
        /// Gets or sets the name of the element.
        /// </summary>
        /// <value>The name of the element.</value>
        public string ElementName { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; set; }

        /// <summary>
        /// Configures the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void Configure(IInteractionNode node)
        {
            this.Bind(node.UIElement, ElementName, Path);
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
                .SetActualHandler(parameters => {
                    Value = updater();
                });

            _updater = updater;
        }
    }
}
#endif