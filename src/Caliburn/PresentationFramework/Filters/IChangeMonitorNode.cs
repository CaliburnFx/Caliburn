namespace Caliburn.PresentationFramework.Filters
{
    using System;

    /// <summary>
    /// Used to track property paths for change notification.
    /// </summary>
    public interface IChangeMonitorNode : IDisposable {
        /// <summary>
        /// The parent node.
        /// </summary>
        IChangeMonitorNode Parent { get; }

        /// <summary>
        /// Indicates whether to stop monitoring changes. 
        /// </summary>
        /// <returns></returns>
        bool ShouldStopMonitoring();

        /// <summary>
        /// Raises change notification.
        /// </summary>
        void NotifyChange();
    }
}