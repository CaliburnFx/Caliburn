#if SILVERLIGHT
namespace Caliburn.PresentationFramework
{
    using System;
    using System.Windows.Markup;
    using System.ComponentModel;

    /// <summary>
    /// Represents a parameter of a message.
    /// </summary>
    [ContentProperty("Value")]
    public class Parameter : INotifyPropertyChanged
    {
        private object _value;

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
            get { return _value; }
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
    }
}
#endif