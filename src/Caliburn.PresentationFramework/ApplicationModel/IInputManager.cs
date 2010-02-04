#if !SILVERLIGHT_20

namespace Caliburn.PresentationFramework.ApplicationModel
{
    /// <summary>
    /// Implemented by services that provide focus and key binding functionality.
    /// </summary>
    public interface IInputManager
    {
        /// <summary>
        /// Focuses the view bound to the view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        void Focus(object viewModel);

        /// <summary>
        /// Focuses the control bound to the property on the view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="propertyPath">The property path.</param>
        void Focus(object viewModel, string propertyPath);
    }
}

#endif