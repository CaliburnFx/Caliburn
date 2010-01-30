namespace Caliburn.PresentationFramework.Conventions
{
    using System.Reflection;
    using System.Windows.Data;
    using ViewModels;

    /// <summary>
    /// The default implementation of <see cref="IViewConvention{T}"/> for bindings.
    /// </summary>
    public class DefaultBindingConvention : ViewConventionBase<PropertyInfo>
    {
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
                element.Convention.BindableProperty,
                path,
                boundProperty.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay,
                ShouldValidate(boundProperty),
                false
                );
        }
    }
}