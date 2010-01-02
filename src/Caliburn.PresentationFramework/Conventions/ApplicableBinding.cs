namespace Caliburn.PresentationFramework.Conventions
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Markup;
    using ApplicationModel;

    public class ApplicableBinding : IViewApplicable
    {
        private readonly string _elementName;
        private readonly DependencyProperty _dependencyProperty;
        private readonly string _path;
        private readonly BindingMode _mode;
        private readonly bool _checkItemTemplate;

        public ApplicableBinding(string elementName, DependencyProperty dependencyProperty, string path, BindingMode mode, bool checkItemTemplate)
        {
            _elementName = elementName;
            _dependencyProperty = dependencyProperty;
            _path = path;
            _mode = mode;
            _checkItemTemplate = checkItemTemplate;
        }

        public void ApplyTo(DependencyObject view)
        {
            var element = (FrameworkElement)view.FindControl(_elementName);

            if (_dependencyProperty != null && ValueNotSet(element))
                element.SetBinding(_dependencyProperty, new Binding(_path) {Mode = _mode});

            if(!_checkItemTemplate) 
                return;

            var itemsControl = (ItemsControl)element;

            if (NeedsItemTemplate(itemsControl))
                itemsControl.ItemTemplate = CreateItemTemplate(itemsControl);
        }

        protected virtual bool ValueNotSet(FrameworkElement element)
        {
#if !SILVERLIGHT_20
            return element.GetBindingExpression(_dependencyProperty) == null &&
                   element.GetValue(_dependencyProperty) == DependencyProperty.UnsetValue;
#else
            return element.GetValue(_dependencyProperty) == DependencyProperty.UnsetValue;
#endif
        }

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