namespace Caliburn.PresentationFramework.Conventions
{
    using Actions;
    using ViewModels;

    public interface IActionConvention
    {
        bool Matches(IViewModelDescription viewModelDescription, IElementDescription element, IAction action);
        IViewApplicable CreateApplication(IViewModelDescription description, IElementDescription element, IAction action);
    }
}