namespace Caliburn.Core.Invocation
{
    using System;
    using Threading;

    /// <summary>
    /// Abstracts a dispatcher capable of executing code on a background or UI thread.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Executes code on the background thread.
        /// </summary>
        /// <param name="backgroundAction">The background action.</param>
        /// <param name="uiCallback">The UI callback.</param>
        /// <param name="progressChanged">The progress change callback.</param>
        IBackgroundTask ExecuteOnBackgroundThread(Action backgroundAction, Action<BackgroundTaskCompletedEventArgs> uiCallback, Action<BackgroundTaskProgressChangedEventArgs> progressChanged);

        /// <summary>
        /// Executes code on the UI thread.
        /// </summary>
        /// <param name="uiAction">The UI action.</param>
        void ExecuteOnUIThread(Action uiAction);

        /// <summary>
        /// Executes code on the UI thread asynchronously.
        /// </summary>
        /// <param name="uiAction">The UI action.</param>
        IDispatcherOperation BeginExecuteOnUIThread(Action uiAction);
    }
}