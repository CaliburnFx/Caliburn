namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using Core;
    using ViewModels;

    public class ItemsControlBindingConvention : IBindingConvention
    {
        private static readonly Type _itemsControlType = typeof(ItemsControl);
        private static readonly Type _selectorControlType = typeof(Selector);

#if !SILVERLIGHT
        private static readonly Type _headeredItemsControlType = typeof(HeaderedItemsControl);
#endif

        public bool Matches(IViewModelDescription viewModelDescription, IElementDescription element, PropertyInfo property)
        {
            return string.Compare(element.Name, property.Name, StringComparison.CurrentCultureIgnoreCase) == 0
                   && _itemsControlType.IsAssignableFrom(element.Type);
        }

        public IViewApplicable CreateApplication(IViewModelDescription description, IElementDescription element, PropertyInfo property)
        {
            string selectionPropertyName = null;
            DependencyProperty bindableProperty = null;
            var mode = BindingMode.TwoWay;

            if (_selectorControlType.IsAssignableFrom(element.Type))
            {
                var selectionProperty = GetSelectionProperty(description, property);
                if (selectionProperty != null)
                {
                    selectionPropertyName = selectionProperty.Name;
                    bindableProperty = Selector.SelectedItemProperty;
                    mode = selectionProperty.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay;
                }
            }
#if !SILVERLIGHT
            else if(_headeredItemsControlType.IsAssignableFrom(element.Type))
            {
                var headerProperty = GetHeaderProperty(description, property);
                if (headerProperty != null)
                {
                    selectionPropertyName = headerProperty.Name;
                    bindableProperty = HeaderedItemsControl.HeaderProperty;
                    mode = headerProperty.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay;
                }
            }
#endif

            return new ApplicableBinding(
                element.Name,
                bindableProperty,
                selectionPropertyName,
                mode,
                true
                );
        }

        protected virtual PropertyInfo GetSelectionProperty(IViewModelDescription description, PropertyInfo property)
        {
            var singularName = property.Name.MakeSingular();
            var found = description.Properties
                .FirstOrDefault(x =>
                                x.Name == "Current" + singularName ||
                                x.Name == "Active" + singularName ||
                                x.Name == "Selected" + singularName
                );

            return found;
        }

#if !SILVERLIGHT
        protected virtual PropertyInfo GetHeaderProperty(IViewModelDescription description, PropertyInfo property)
        {
            var found = description.Properties
                .FirstOrDefault(x => x.Name == property.Name + "Header");

            return found;
        }
#endif
    }
}