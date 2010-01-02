namespace Caliburn.PresentationFramework.ViewModels
{
    /// <summary>
    /// Indicates the a model/presenter should be made aware of its view.
    /// </summary>
    public interface IViewAware
    {
        /// <summary>
        /// Called when the implementor's view is loaded.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="context">The context.</param>
        void ViewLoaded(object view, object context);
    }
}