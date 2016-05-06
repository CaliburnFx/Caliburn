#if SILVERLIGHT
namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Markup;
    using System.ComponentModel;
    using Core.Invocation;
    using Views;

    /// <summary>
    /// Represents a parameter of a message.
    /// </summary>
    [ContentProperty("Value")]
    public class Parameter : INotifyPropertyChanged
    {
        object value;
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

                return value;
            }
            set
            {
				if (Equals(this.value, value)) return;
                this.value = value; 
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
            var fe = node.UIElement as FrameworkElement;
            if (fe != null)
            {
                RoutedEventHandler handler = null;
                handler = (s, e) =>{
                    this.Bind(node.UIElement, ElementName, Path);
                    fe.Loaded -= handler;
                };

                if((bool)fe.GetValue(View.IsLoadedProperty))
                    handler(null, null);
                else fe.Loaded += handler;
            }
            else this.Bind(node.UIElement, ElementName, Path);
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