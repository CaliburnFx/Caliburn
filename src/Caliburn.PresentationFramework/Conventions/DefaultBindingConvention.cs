namespace Caliburn.PresentationFramework.Conventions
{
    using System.Reflection;
    using System.Windows.Data;
    using Core.Logging;
    using ViewModels;
    using Views;

    /// <summary>
    /// The default implementation of <see cref="IViewConvention{T}"/> for bindings.
    /// </summary>
    public class DefaultBindingConvention : ViewConventionBase<PropertyInfo>
    {
        static readonly ILog Log = LogManager.GetLog(typeof(DefaultBindingConvention));

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

            if (boundProperty == null)
                return null;

            Log.Info("Binding convention matched for {0}.", element.Name);

			var setMethod = boundProperty.GetSetMethod();
			var canWriteToProperty = boundProperty.CanWrite && setMethod != null && setMethod.IsPublic;

            return new ApplicableBinding(
                element,
                element.Convention.BindableProperty,
                correctedPath,
                canWriteToProperty ? BindingMode.TwoWay : BindingMode.OneWay,
                ShouldValidate(boundProperty),
                false,
                conventionManager.GetValueConverter(element.Convention.BindableProperty, boundProperty.PropertyType)
                );
        }
    }
}