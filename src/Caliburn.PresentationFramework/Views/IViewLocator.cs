namespace Caliburn.PresentationFramework.Views
{
    using System;
    using System.Windows;

    /// <summary>
    /// A strategy for determining which view to use for a given model.
    /// </summary>
    public interface IViewLocator
    {
        /// <summary>
        /// Locates the view for the specified model instance.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="displayLocation">The display location.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        DependencyObject Locate(object model, DependencyObject displayLocation, object context);

        /// <summary>
        /// Locates the View for the specified model type.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="displayLocation">The display location.</param>
        /// <param name="context">The context.</param>
        /// <returns>The view.</returns>
        DependencyObject Locate(Type modelType, DependencyObject displayLocation, object context);
    }
}