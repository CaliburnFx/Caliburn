#if SILVERLIGHT
namespace Caliburn.PresentationFramework
{
    using System;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Data;
    using System.Windows.Controls;
    using Caliburn.Core;
    using Caliburn.Core.Invocation;
    using System.Reflection;
    using System.ComponentModel;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Represents a parameter of a message.
    /// </summary>
    [ContentProperty("Value")]
    public class Parameter : INotifyPropertyChanged
    {
        private object _value;
        private bool _isLoaded;

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
            if (!string.IsNullOrEmpty(ElementName))
            {
                var element = node.UIElement as FrameworkElement;

                if (element != null)
                {
                    element.Loaded +=
                        (s, e) => {
                            if(_isLoaded) return;

                            _isLoaded = true;
                            var source = element.FindName<object>(ElementName, false);

                            if (source != null)
                            {
                                if(!string.IsNullOrEmpty(Path))
                                {
                                    var sourceType = source.GetType();
                                    var property = sourceType.GetProperty(Path);

                                    EventInfo changeEvent = null;
                                    
                                    if(Path == "SelectedItem")
                                        changeEvent = sourceType.GetEvent("SelectionChanged");
                                    if(changeEvent == null)
                                        changeEvent = sourceType.GetEvent(Path + "Changed");
                                    if(changeEvent == null)
                                        WireToDefaultEvent(sourceType, source, property);
                                    else
                                    {
                                        ServiceLocator.Current.GetInstance<IEventHandlerFactory>().Wire(source, changeEvent)
                                            .SetActualHandler(parameters => {
                                                Value = property.GetValue(source, null);
                                            });
                                    }
                                }
                                else WireToDefaultEvent(source.GetType(), source, null);
                            }
                        };
                }
            }
        }

        private void WireToDefaultEvent(Type type, object source, PropertyInfo property)
        {
            var defaults = ServiceLocator.Current.GetInstance<IRoutedMessageController>().GetInteractionDefaults(type);

            if(defaults == null)
                throw new CaliburnException(
                    "Insuficient information provided for wiring action parameters.  Please set interaction defaults for "  + type.FullName
                    );

            if(property == null)
                ServiceLocator.Current.GetInstance<IEventHandlerFactory>().Wire(source, defaults.DefaultEventName)
                    .SetActualHandler(
                    parameters => {
                        Value = defaults.GetDefaultValue(source);
                    });
            else
                ServiceLocator.Current.GetInstance<IEventHandlerFactory>().Wire(source, defaults.DefaultEventName)
                    .SetActualHandler(parameters => {
                        Value = property.GetValue(source, null);
                    });
        }
    }
}
#endif