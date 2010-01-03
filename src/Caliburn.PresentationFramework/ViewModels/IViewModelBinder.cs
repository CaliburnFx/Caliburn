namespace Caliburn.PresentationFramework.ViewModels
{
    using System.Windows;

    /// <summary>
    /// Implemented by services that bind a view to a view model.
    /// </summary>
    public interface IViewModelBinder
    {
        /// <summary>
        /// Gets or sets a value indicating whether to apply conventions by default.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if conventions should be applied by default; otherwise, <c>false</c>.
        /// </value>
        bool ApplyConventionsByDefault { get; set; }

        /// <summary>
        /// Binds the specified viewModel to the view.
        /// </summary>
        /// <param name="viewModel">The model.</param>
        /// <param name="view">The view.</param>
        /// <param name="context">The context.</param>
        void Bind(object viewModel, DependencyObject view, object context);
    }
}