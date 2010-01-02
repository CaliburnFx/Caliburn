namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Reflection;
    using System.Windows.Data;
    using ApplicationModel;
    using ViewModels;

    public class DefaultBindingConvention : IBindingConvention
    {
        private static readonly Type _presenterType = typeof(IPresenter);

        public bool Matches(IViewModelDescription viewModelDescription, IElementDescription element, PropertyInfo property)
        {
            return string.Compare(element.Name, property.Name, StringComparison.CurrentCultureIgnoreCase) == 0;
        }

        public IViewApplicable CreateApplication(IViewModelDescription description, IElementDescription element, PropertyInfo property)
        {
            return new ApplicableBinding(
                element.Name,
                _presenterType.IsAssignableFrom(property.PropertyType)
                    ? View.ModelProperty
                    : element.Convention.BindableProperty,
                property.Name,
                property.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay,
                false
                );
        }
    }
}