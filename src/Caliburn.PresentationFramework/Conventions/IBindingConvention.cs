namespace Caliburn.PresentationFramework.Conventions
{
    using System.Reflection;
    using ViewModels;

    /// <summary>
    /// Implemented by data binding conventions.
    /// </summary>
    public interface IBindingConvention
    {
        /// <summary>
        /// Indicates whether this convention is a match and should be applied.
        /// </summary>
        /// <param name="viewModelDescription">The view model description.</param>
        /// <param name="element">The element.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        bool Matches(IViewModelDescription viewModelDescription, IElementDescription element, PropertyInfo property);
        
        /// <summary>
        /// Creates the application of the convention.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="element">The element.</param>
        /// <param name="property">The property.</param>
        /// <returns>The convention application.</returns>
        IViewApplicable CreateApplication(IViewModelDescription description, IElementDescription element, PropertyInfo property);
    }
}