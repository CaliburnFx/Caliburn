namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;

    /// <summary>
    /// A class capable of notifying subscribers of events related to the lifecycle of an <see cref="IPresenter"/>.
    /// </summary>
    public interface ILifecycleNotifier
    {
        /// <summary>
        /// Occurs when [initialized].
        /// </summary>
        event EventHandler Initialized;

        /// <summary>
        /// Occurs when [was shutdown].
        /// </summary>
        event EventHandler WasShutdown;

        /// <summary>
        /// Occurs when [activated].
        /// </summary>
        event EventHandler Activated;

        /// <summary>
        /// Occurs when [deactivated].
        /// </summary>
        event EventHandler Deactivated;
    }
}