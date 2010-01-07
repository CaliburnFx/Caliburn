namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;

    /// <summary>
    /// A class capable of notifying subscribers of events related to its lifecycle.
    /// </summary>
    public interface ILifecycleNotifier
    {
        /// <summary>
        /// Occurs when initialized.
        /// </summary>
        event EventHandler Initialized;

        /// <summary>
        /// Occurs before attempting to shutdown.
        /// </summary>
        event EventHandler AttemptingShutdown;

        /// <summary>
        /// Occurs after the this instance was shutdown.
        /// </summary>
        event EventHandler WasShutdown;

        /// <summary>
        /// Occurs when activated.
        /// </summary>
        event EventHandler Activated;

        /// <summary>
        /// Occurs when deactivated.
        /// </summary>
        event EventHandler Deactivated;
    }
}