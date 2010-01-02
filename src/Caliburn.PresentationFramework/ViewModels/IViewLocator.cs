namespace Caliburn.PresentationFramework.ViewModels
{
    using System.Windows;

    /// <summary>
    /// A strategy for determining which view to use for a given model.
    /// </summary>
    public interface IViewLocator
    {
        /// <summary>
        /// Gets the view for displaying the specified viewModel.
        /// </summary>
        /// <param name="viewModel">The model.</param>
        /// <param name="displayLocation">The control into which the view will be injected.</param>
        /// <param name="context">Some additional context used to select the proper view.</param>
        /// <returns></returns>
        DependencyObject Locate(object viewModel, DependencyObject displayLocation, object context);
    }
}