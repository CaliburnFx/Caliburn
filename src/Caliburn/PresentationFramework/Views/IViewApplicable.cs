namespace Caliburn.PresentationFramework.Views
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
        /// <param name="isLoaded">Indicates whether the view element should be marked as loaded.</param>
        void ApplyTo(DependencyObject view, object isLoaded);
    }
}