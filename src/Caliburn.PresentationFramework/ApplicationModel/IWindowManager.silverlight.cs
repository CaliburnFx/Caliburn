﻿#if SILVERLIGHT

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
        void ShowDialog(object rootModel, object context);
    }
}

#endif