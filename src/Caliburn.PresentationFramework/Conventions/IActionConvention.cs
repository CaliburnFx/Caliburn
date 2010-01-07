namespace Caliburn.PresentationFramework.Conventions
{
    using Actions;
    using ViewModels;

    /// <summary>
    /// Implemented by action conventions.
    /// </summary>
    public interface IActionConvention
    {
        /// <summary>
        /// MIndicates whether this convention is a match and should be applied.
        /// </summary>
        /// <param name="viewModelDescription">The view model description.</param>
        /// <param name="element">The element.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        bool Matches(IViewModelDescription viewModelDescription, IElementDescription element, IAction action);
        
        /// <summary>
        /// Creates the application of the convention.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="element">The element.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        IViewApplicable CreateApplication(IViewModelDescription description, IElementDescription element, IAction action);
    }
}