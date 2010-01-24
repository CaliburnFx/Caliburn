namespace Caliburn.PresentationFramework.Conventions
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Markup;
    using ViewModels;

    /// <summary>
    /// An <see cref="IViewApplicable"/> that sets a databinding on an element.
    /// </summary>
    public class ApplicableBinding : IViewApplicable
    {
        private readonly string _elementName;
        private readonly DependencyProperty _dependencyProperty;
        private readonly string _path;
        private readonly BindingMode _mode;
        private readonly bool _validate;
        private readonly bool _checkItemTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicableBinding"/> class.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        /// <param name="path">The path.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="validate">Inidicates whether or not to turn on validation for the binding.</param>
        /// <param name="checkItemTemplate">if set to <c>true</c> [check item template].</param>
        public ApplicableBinding(string elementName, DependencyProperty dependencyProperty, string path, BindingMode mode, bool validate, bool checkItemTemplate)
        {
            _elementName = elementName;
            _dependencyProperty = dependencyProperty;
            _path = path;
            _mode = mode;
            _validate = validate;
            _checkItemTemplate = checkItemTemplate;
        }

        /// <summary>
        /// Applies the behavior to the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        public void ApplyTo(DependencyObject view)
        {
            var element = view.FindName(_elementName);
            var binding = new Binding(_path) {Mode = _mode};

#if SILVERLIGHT_40 || NET
            if (_validate)
                binding.ValidatesOnDataErrors = true;
#elif SILVERLIGHT_30
            if(_validate)
                binding.ValidatesOnExceptions = true;
#endif

            if (_dependencyProperty != null && ValueNotSet(element))
                element.SetBinding(_dependencyProperty, binding);

            if(!_checkItemTemplate) 
                return;

            var itemsControl = (ItemsControl)element;

            if (NeedsItemTemplate(itemsControl))
                itemsControl.ItemTemplate = CreateItemTemplate(itemsControl);
        }

        /// <summary>
        /// Values the not set.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        protected virtual bool ValueNotSet(DependencyObject element)
        {
#if !SILVERLIGHT_20
            return element.GetBindingExpression(_dependencyProperty) == null;
#else
            return true;
#endif
        }

        /// <summary>
        /// Needses the item template.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns></returns>
        protected virtual bool NeedsItemTemplate(ItemsControl control)
        {
            return control.ItemTemplate == null && string.IsNullOrEmpty(control.DisplayMemberPath);
        }

        private const string _templateCore =
            "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:am='clr-namespace:Caliburn.PresentationFramework.ApplicationModel;assembly=Caliburn.PresentationFramework'> " +
            "<ContentControl am:View.Model=\"{Binding}\" ";

        /// <summary>
        /// Creates an item template which binds view models.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns></returns>
        protected virtual DataTemplate CreateItemTemplate(ItemsControl itemsControl)
        {
            var context = View.GetContext(itemsControl);
            var template = _templateCore;

            if (context != null)
                template += "am:View.Context=\"" + context + "\"";

            template += " /></DataTemplate>";

#if SILVERLIGHT
            return (DataTemplate)XamlReader.Load(template);
#else
            return (DataTemplate)XamlReader.Parse(template);
#endif
        }
    }
}