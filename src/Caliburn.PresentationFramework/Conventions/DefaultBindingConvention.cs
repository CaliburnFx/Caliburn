namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Reflection;
    using System.Windows.Data;
    using Screens;
    using ViewModels;

    /// <summary>
    /// The default implementation of <see cref="IBindingConvention"/>.
    /// </summary>
    public class DefaultBindingConvention : IBindingConvention
    {
        private static readonly Type _screenType = typeof(IScreen);

        /// <summary>
        /// Indicates whether this convention is a match and should be applied.
        /// </summary>
        /// <param name="viewModelDescription">The view model description.</param>
        /// <param name="element">The element.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public bool Matches(IViewModelDescription viewModelDescription, IElementDescription element, PropertyInfo property)
        {
            return string.Compare(element.Name, property.Name, StringComparison.CurrentCultureIgnoreCase) == 0;
        }

        /// <summary>
        /// Creates the application of the convention.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="element">The element.</param>
        /// <param name="property">The property.</param>
        /// <returns>The convention application.</returns>
        public IViewApplicable CreateApplication(IViewModelDescription description, IElementDescription element, PropertyInfo property)
        {
            return new ApplicableBinding(
                element.Name,
                _screenType.IsAssignableFrom(property.PropertyType)
                    ? View.ModelProperty
                    : element.Convention.BindableProperty,
                property.Name,
                property.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay,
                false
                );
        }
    }
}