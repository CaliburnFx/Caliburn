#if SILVERLIGHT

namespace Caliburn.PresentationFramework.RoutedMessaging.Triggers
{
    /// <summary>
    /// Specifies constants that define actions performed by the mouse.
    /// </summary>
    public enum MouseAction
    {
        /// <summary>
        /// No action.
        /// </summary>
        None,

        /// <summary>
        /// A left mouse button click.
        /// </summary>
        LeftClick,

        /// <summary>
        /// A left mouse button double click.
        /// </summary>
        LeftDoubleClick,

#if SILVERLIGHT_40

        /// <summary>
        /// A right mouse button click.
        /// </summary>
        RightClick,

        /// <summary>
        /// A right mouse button double click.
        /// </summary>
        RightDoubleClick,

#endif
    }
}

#endif