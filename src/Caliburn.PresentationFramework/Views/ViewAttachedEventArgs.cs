namespace Caliburn.PresentationFramework.Views
{
    using System;

    /// <summary>
    /// The event args for the <see cref="IViewAware.ViewAttached"/> event.
    /// </summary>
    public class ViewAttachedEventArgs : EventArgs
    {
        /// <summary>
        /// The view.
        /// </summary>
        public object View { get; set; }

        /// <summary>
        /// The context.
        /// </summary>
        public object Context { get; set; }
    }
}