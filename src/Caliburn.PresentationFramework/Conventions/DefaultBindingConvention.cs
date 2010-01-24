namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Reflection;
    using System.Windows.Data;
    using Screens;
    using ViewModels;

    /// <summary>
    /// The default implementation of <see cref="IViewConvention{T}"/> for bindings.
    /// </summary>
    public class DefaultBindingConvention : ViewConventionBase<PropertyInfo>
    {
        private static readonly Type _screenType = typeof(IScreen);

        /// <summary>
        /// Creates the application of the convention.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="element">The element.</param>
        /// <param name="property">The property.</param>
        /// <returns>The convention application.</returns>
        public override IViewApplicable TryCreateApplication(IViewModelDescription description, IElementDescription element, PropertyInfo property)
        {
            var path = DeterminePropertyPath(element.Name);
            var boundProperty = GetBoundProperty(property, path);

            if (boundProperty == null)
                return null;

            return new ApplicableBinding(
                element.Name,
                _screenType.IsAssignableFrom(boundProperty.PropertyType)
                    ? View.ModelProperty
                    : element.Convention.BindableProperty,
                path,
                boundProperty.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay,
                ShouldValidate(property),
                false
                );
        }
    }
}