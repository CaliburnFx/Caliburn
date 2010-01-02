namespace Caliburn.PresentationFramework.Conventions
{
    using System.Reflection;
    using ViewModels;

    public interface IBindingConvention
    {
        bool Matches(IViewModelDescription viewModelDescription, IElementDescription element, PropertyInfo property);
        IViewApplicable CreateApplication(IViewModelDescription description, IElementDescription element, PropertyInfo property);
    }
}