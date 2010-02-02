namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using Core;
    using ViewModels;
using Caliburn.PresentationFramework.Converters;

    /// <summary>
    /// An implemenation of <see cref="IViewConvention{T}"/> that bindings SelectedItem and Header for Selectors and HeaderedItemsControls respectively.
    /// ItemsTemplates may be conventionally added as well.
    /// </summary>
    public class ItemsControlBindingConvention : ViewConventionBase<PropertyInfo>
    {
        private static readonly Type _itemsControlType = typeof(ItemsControl);
        private static readonly Type _selectorControlType = typeof(Selector);
        private static readonly EnumConverter _enumConverter = new EnumConverter();

#if !SILVERLIGHT
        private static readonly Type _headeredItemsControlType = typeof(HeaderedItemsControl);
#endif
        /// <summary>
        /// Creates the application of the convention.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="element">The element.</param>
        /// <param name="property">The property.</param>
        /// <returns>The convention application.</returns>
        public override IViewApplicable TryCreateApplication(IViewModelDescription description, IElementDescription element, PropertyInfo property)
        {
            var originalPath = DeterminePropertyPath(element.Name);
            var boundProperty = GetBoundProperty(property, originalPath);

            if (boundProperty == null || !_itemsControlType.IsAssignableFrom(element.Type))
                return null;

            string path = null;
            DependencyProperty bindableProperty = null;
            BindingMode mode = BindingMode.OneWay;
            bool checkTemplate = true;
            IValueConverter converter = null;

            if (_selectorControlType.IsAssignableFrom(element.Type))
            {
                string selectionPath;
                PropertyInfo selectionProperty;

                if (TryGetByPattern(property, originalPath, out selectionPath, out selectionProperty,
                    originalName => originalName.MakeSingular(),
                    (info, baseName) =>
                        info.Name == "Current" + baseName ||
                        info.Name == "Active" + baseName ||
                        info.Name == "Selected" + baseName)
                    )
                {
                    path = selectionPath;
                    bindableProperty = Selector.SelectedItemProperty;
                    mode = selectionProperty.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay;
                    checkTemplate = ShouldCheckTemplate(selectionProperty);
                    converter = typeof(BindableCollection<BindableEnum>).IsAssignableFrom(boundProperty.PropertyType)
                                    ? _enumConverter
                                    : null;
                }
                else return null;
            }
#if !SILVERLIGHT
            else if (_headeredItemsControlType.IsAssignableFrom(element.Type))
            {
                string headerPath;
                PropertyInfo headerProperty;

                if (TryGetByPattern(property, originalPath, out headerPath, out headerProperty,
                    originalName => originalName,
                    (info, baseName) =>
                        info.Name == baseName + "Header")
                    )
                {
                    path = headerPath;
                    bindableProperty = HeaderedItemsControl.HeaderProperty;
                    mode = headerProperty.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay;
                    checkTemplate = ShouldCheckTemplate(headerProperty);
                }
                else return null;
            }
#endif

            return new ApplicableBinding(
                element.Name,
                bindableProperty,
                path,
                mode,
                false,
                checkTemplate,
                converter
                );
        }

        private bool ShouldCheckTemplate(PropertyInfo property)
        {
            return !property.PropertyType.IsEnum &&
                !property.PropertyType.IsPrimitive &&
                !typeof(string).IsAssignableFrom(property.PropertyType);
        }
    }
}