#if SILVERLIGHT

namespace Caliburn.ShellFramework.History
{
    using System;

    /// <summary>
    /// Coordinates state of the history and the application.
    /// </summary>
    public interface IHistoryCoordinator
    {
        /// <summary>
        /// Starts the coordinator with the specified configuration callback.
        /// </summary>
        /// <param name="configurator">The configuration callback.</param>
        void Start(Action<HistoryConfiguration> configurator);

        /// <summary>
        /// Refreshes the coordinated conductor from history.
        /// </summary>
        void Refresh();
    }
}

#endif