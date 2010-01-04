namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Reflection;
    using System.Windows.Data;
    using Screens;
    using ViewModels;

    public class DefaultBindingConvention : IBindingConvention
    {
        private static readonly Type _screenType = typeof(IScreen);

        public bool Matches(IViewModelDescription viewModelDescription, IElementDescription element, PropertyInfo property)
        {
            return string.Compare(element.Name, property.Name, StringComparison.CurrentCultureIgnoreCase) == 0;
        }

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