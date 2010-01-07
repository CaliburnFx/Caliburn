namespace Caliburn.PresentationFramework.ViewModels
{
    using System.Windows;

    /// <summary>
    /// Implemented by classes that apply behavior to views.
    /// </summary>
    public interface IViewApplicable
    {
        /// <summary>
        /// Applies the behavior to the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        void ApplyTo(DependencyObject view);
    }
}