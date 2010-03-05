namespace Caliburn.PresentationFramework.Views
{
    using System.Windows;

    /// <summary>
    /// Indicates that a model should be made aware of its view.
    /// </summary>
    public interface IViewAware
    {
        /// <summary>
        /// Attaches a view to this instance.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="context">The context.</param>
        void AttachView(DependencyObject view, object context);

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The view</returns>
        DependencyObject GetView(object context);
    }
}