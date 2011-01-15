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
    using Core.Logging;
    using ViewModels;
    using Views;

    /// <summary>
    /// An implemenation of <see cref="IViewConvention{T}"/> that bindings SelectedItem and Header for Selectors and HeaderedItemsControls respectively.
    /// ItemsTemplates may be conventionally added as well.
    /// </summary>
    public class ItemsControlBindingConvention : ViewConventionBase<PropertyInfo>
    {
        static readonly ILog Log = LogManager.GetLog(typeof(ItemsControlBindingConvention));

        static readonly Type ItemsControlType = typeof(ItemsControl);
        static readonly Type SelectorControlType = typeof(Selector);

#if !SILVERLIGHT
        static readonly Type HeaderedItemsControlType = typeof(HeaderedItemsControl);
#endif
        /// <summary>
        /// Creates the application of the convention.
        /// </summary>
        /// <param name="conventionManager">The convention manager.</param>
        /// <param name="description">The description.</param>
        /// <param name="element">The element.</param>
        /// <param name="property">The property.</param>
        /// <returns>The convention application.</returns>
        public override IViewApplicable TryCreateApplication(IConventionManager conventionManager, IViewModelDescription description, ElementDescription element, PropertyInfo property)
        {
            var expectedPath = DeterminePropertyPath(element.Name);
            string correctedPath;
            var boundProperty = GetBoundProperty(property, expectedPath, out correctedPath);

            if (boundProperty == null || !ItemsControlType.IsAssignableFrom(element.Type))
                return null;

            string path = null;
            DependencyProperty bindableProperty = null;
            BindingMode mode = BindingMode.OneWay;
            bool checkTemplate = ShouldCheckTemplate(property);
            IValueConverter converter = null;

            if (SelectorControlType.IsAssignableFrom(element.Type))
            {
                string selectionPath;
                PropertyInfo selectionProperty;

                if (TryGetByPattern(property, correctedPath, out selectionPath, out selectionProperty,
                    originalName => originalName.MakeSingular(),
                    (info, baseName) =>
                        string.Compare(info.Name, "Current" + baseName, StringComparison.CurrentCultureIgnoreCase) == 0 ||
                            string.Compare(info.Name, "Active" + baseName, StringComparison.CurrentCultureIgnoreCase) == 0 ||
                                string.Compare(info.Name, "Selected" + baseName, StringComparison.CurrentCultureIgnoreCase) == 0
                    ))
                {
                    path = selectionPath;
                    bindableProperty = Selector.SelectedItemProperty;
                    mode = selectionProperty.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay;
                    converter = conventionManager.GetValueConverter(bindableProperty, boundProperty.PropertyType);

                    Log.Info("Selector binding convention added to {0}.", element.Name);
                }
                else return null;
            }
#if !SILVERLIGHT
            else if (HeaderedItemsControlType.IsAssignableFrom(element.Type))
            {
                string headerPath;
                PropertyInfo headerProperty;

                if (TryGetByPattern(property, correctedPath, out headerPath, out headerProperty,
                    originalName => originalName,
                    (info, baseName) =>
                        string.Compare(info.Name, baseName + "Header", StringComparison.CurrentCultureIgnoreCase) == 0
                    ))
                {
                    path = headerPath;
                    bindableProperty = HeaderedItemsControl.HeaderProperty;
                    mode = headerProperty.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay;
                    converter = conventionManager.GetValueConverter(bindableProperty, headerProperty.PropertyType);

                    Log.Info("Header binding convention added to {0}.", element.Name);
                }
                else return null;
            }
#endif

            return new ApplicableBinding(
                element,
                bindableProperty,
                path,
                mode,
                false,
                checkTemplate,
                converter
                );
        }

        static bool ShouldCheckTemplate(PropertyInfo property)
        {
            if (!property.PropertyType.IsGenericType)
                return false;

            var itemType = property.PropertyType.GetGenericArguments().First();
            return !itemType.IsValueType && !typeof(string).IsAssignableFrom(itemType);
        }
    }
}