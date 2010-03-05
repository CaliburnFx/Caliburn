namespace Caliburn.Core.Threading
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// A task that can execute asynchronously.
    /// </summary>
    public interface IBackgroundTask
    {
        /// <summary>
        /// Enqueues the task with the specified user state.
        /// </summary>
        /// <param name="userState">The user supplied state.</param>
        void Start(object userState);

        /// <summary>
        /// Cancels the task.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Gets a value indicating whether this task is currently processing.
        /// </summary>
        /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
        bool IsBusy { get; }

        /// <summary>
        /// Gets a value indicating whether this task's cancellation is pending.
        /// </summary>
        /// <value><c>true</c> if cancellation is pending; otherwise, <c>false</c>.</value>
        bool CancellationPending { get; }

        /// <summary>
        /// Occurs on the background thread just prior to the execution of work.
        /// </summary>
        event EventHandler Starting;

        /// <summary>
        /// Occurs when the background task indicates that progress has changed.
        /// </summary>
        event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Occurs when the background task has completed either successfully, by cancellation or with an error.
        /// </summary>
        event RunWorkerCompletedEventHandler Completed;
    }
}