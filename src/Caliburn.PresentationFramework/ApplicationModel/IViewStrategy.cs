namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System.Windows;

    /// <summary>
    /// A strategy for determining which view to use for a given model.
    /// </summary>
    public interface IViewStrategy
    {
        /// <summary>
        /// Gets the view for displaying the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="displayLocation">The control into which the view will be injected.</param>
        /// <param name="context">Some additional context used to select the proper view.</param>
        /// <returns></returns>
        object GetView(object model, DependencyObject displayLocation, object context);
    }
}