namespace Caliburn.PresentationFramework.Conventions
{
    using System.Reflection;
    using System.Windows.Data;
    using Core.Logging;
    using Screens;
    using ViewModels;
    using Views;

    /// <summary>
    /// The default implementation of <see cref="IViewConvention{T}"/> for bindings.
    /// </summary>
    public class DefaultBindingConvention : ViewConventionBase<PropertyInfo>
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(DefaultBindingConvention));

        /// <summary>
        /// Creates the application of the convention.
        /// </summary>
        /// <param name="conventionManager">The convention manager.</param>
        /// <param name="description">The description.</param>
        /// <param name="element">The element.</param>
        /// <param name="property">The property.</param>
        /// <returns>The convention application.</returns>
        public override IViewApplicable TryCreateApplication(IConventionManager conventionManager, IViewModelDescription description, IElementDescription element, PropertyInfo property)
        {
            var path = DeterminePropertyPath(element.Name);
            var boundProperty = GetBoundProperty(property, path);

            if (boundProperty == null)
                return null;

            var dependencyProperty = typeof(IScreen).IsAssignableFrom(boundProperty.PropertyType)
                ? View.ModelProperty
                : element.Convention.BindableProperty;

            Log.Info("Binding convention matched for {0}.", element.Name);

            return new ApplicableBinding(
                element,
                dependencyProperty,
                path,
                boundProperty.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay,
                ShouldValidate(boundProperty),
                false,
                conventionManager.GetValueConverter(dependencyProperty, boundProperty.PropertyType)
                );
        }
    }
}