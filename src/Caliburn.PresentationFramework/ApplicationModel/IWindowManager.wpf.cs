#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.ApplicationModel
{
    /// <summary>
    /// A service that manages windows.
    /// </summary>
    public interface IWindowManager
    {
        /// <summary>
        /// Shows a window for the specified model.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        void Show(object rootModel, object context);
        
        /// <summary>
        /// Shows a modal dialog for the specified model.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        bool? ShowDialog(object rootModel, object context);
    }
}

#endif