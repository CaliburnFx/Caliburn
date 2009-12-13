namespace Caliburn.Core.Invocation
{
    using System;
    using Threading;

    /// <summary>
    /// Utility methods supporting various forms of method execution.
    /// </summary>
    public static class Execute
    {
        private static IDispatcher _dispatcher = new DefaultDispatcher();

        /// <summary>
        /// Initializes for execution.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        public static void Initialize(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Executes the backgroundAction on a background thread.
        /// </summary>
        /// <param name="backgroundAction">The action.</param>
        public static IBackgroundTask OnBackgroundThread(Action backgroundAction)
        {
            return _dispatcher.ExecuteOnBackgroundThread(backgroundAction, null, null);
        }

        /// <summary>
        /// Executes the backgroundAction on a background thread.
        /// </summary>
        /// <param name="backgroundAction">The action.</param>
        /// <param name="uiCallback">The UI callback.</param>
        public static IBackgroundTask OnBackgroundThread(Action backgroundAction, Action<BackgroundTaskCompletedEventArgs> uiCallback)
        {
            return _dispatcher.ExecuteOnBackgroundThread(backgroundAction, uiCallback, null);
        }

        /// <summary>
        /// Executes the backgroundAction on a background thread.
        /// </summary>
        /// <param name="backgroundAction">The action.</param>
        /// <param name="uiCallback">The UI callback.</param>
        /// <param name="progressChanged">The progress change callback.</param>
        public static IBackgroundTask OnBackgroundThread(Action backgroundAction, Action<BackgroundTaskCompletedEventArgs> uiCallback, Action<BackgroundTaskProgressChangedEventArgs> progressChanged)
        {
            return _dispatcher.ExecuteOnBackgroundThread(backgroundAction, uiCallback, progressChanged);
        }

        /// <summary>
        /// Executes the backgroundAction on a background thread.
        /// </summary>
        /// <param name="backgroundAction">The action.</param>
        /// <param name="progressChanged">The progress change callback.</param>
        public static IBackgroundTask OnBackgroundThread(Action backgroundAction, Action<BackgroundTaskProgressChangedEventArgs> progressChanged)
        {
            return _dispatcher.ExecuteOnBackgroundThread(backgroundAction, null, progressChanged);
        }

        /// <summary>
        /// Executes the action on the UI thread.
        /// </summary>
        /// <param name="action">The action.</param>
        public static void OnUIThread(Action action)
        {
            _dispatcher.ExecuteOnUIThread(action);
        }

        /// <summary>
        /// Executes the action on the UI thread asynchronously.
        /// </summary>
        /// <param name="action">The action.</param>
        public static IDispatcherOperation OnUIThreadAsync(Action action)
        {
            return _dispatcher.BeginExecuteOnUIThread(action);
        }
    }
}