namespace Caliburn.PresentationFramework.ViewModels
{
    using System.Windows;

    /// <summary>
    /// Implemented by services that bind a view to a model.
    /// </summary>
    public interface IViewModelBinder
    {
        /// <summary>
        /// Binds the specified viewModel to the view.
        /// </summary>
        /// <param name="viewModel">The model.</param>
        /// <param name="view">The view.</param>
        /// <param name="context">The context.</param>
        void Bind(object viewModel, DependencyObject view, object context);
    }
}