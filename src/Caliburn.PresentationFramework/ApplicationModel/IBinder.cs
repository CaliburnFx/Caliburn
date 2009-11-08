namespace Caliburn.PresentationFramework.ApplicationModel
{
    /// <summary>
    /// Implemented by services that bind a view to a model.
    /// </summary>
    public interface IBinder
    {
        /// <summary>
        /// Binds the specified model to the view.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="view">The view.</param>
        /// <param name="context">The context.</param>
        void Bind(object model, object view, object context);
    }
}