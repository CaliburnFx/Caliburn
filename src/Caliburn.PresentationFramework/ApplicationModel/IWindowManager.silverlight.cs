#if SILVERLIGHT_30 || SILVERLIGHT_40 && !WP7

namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;

    /// <summary>
    /// A service that manages windows.
    /// </summary>
    public interface IWindowManager
    {
        /// <summary>
        /// Shows a modal dialog for the specified model.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <param name="handleShutdownModel">The handle shutdown model.</param>
        void ShowDialog(object rootModel, object context, Action<ISubordinate, Action> handleShutdownModel);
    }
}

#endif