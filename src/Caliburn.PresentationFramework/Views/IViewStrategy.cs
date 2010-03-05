namespace Caliburn.PresentationFramework.Views
{
    using System;
    using System.Windows;

    /// <summary>
    /// Applies a specific strategy for locating views to the annotated instance.
    /// </summary>
    public interface IViewStrategy
    {
        /// <summary>
        /// Determines whether this strategy applies in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>true if it matches the context; false otherwise</returns>
        bool Matches(object context);

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