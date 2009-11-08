namespace Caliburn.Core.Invocation
{
    using System;
    using Threading;

    /// <summary>
    /// Utility methods supporting various forms of method execution.
    /// </summary>
    public static class Execute
    {
        private static IDispatcher _dispatcher = new SimpleDispatcher();

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

        /// <summary>
        /// A simple implementation of <see cref="IDispatcher"/> suitable only during test scenarios.
        /// </summary>
        /// <remarks>This implementation only executes code on the calling thread.</remarks>
        public class SimpleDispatcher : IDispatcher
        {
            /// <summary>
            /// Executes code on the background thread.
            /// </summary>
            /// <param name="backgroundAction">The background action.</param>
            /// <param name="uiCallback">The UI callback.</param>
            /// <param name="progressChanged">The progress change callback.</param>
            public IBackgroundTask ExecuteOnBackgroundThread(Action backgroundAction, Action<BackgroundTaskCompletedEventArgs> uiCallback, Action<BackgroundTaskProgressChangedEventArgs> progressChanged)
            {
                var task = new ForegroundTask(backgroundAction);

                if (uiCallback != null)
                    task.Completed += (s, e) => uiCallback(e);

                if (progressChanged != null)
                    task.ProgressChanged += (s, e) => progressChanged(e);

                task.Enqueue(null);

                return task;
            }

            /// <summary>
            /// Executes code on the UI thread.
            /// </summary>
            /// <param name="uiAction">The UI action.</param>
            public void ExecuteOnUIThread(Action uiAction)
            {
                uiAction();
            }

            /// <summary>
            /// Executes code on the UI thread asynchronously.
            /// </summary>
            /// <param name="uiAction">The UI action.</param>
            public IDispatcherOperation BeginExecuteOnUIThread(Action uiAction)
            {
                return new FakeDispatcherOperation(uiAction);
            }

            /// <summary>
            /// Used by the <see cref="SimpleDispatcher"/> to execute background tasks on the main thread.
            /// </summary>
            private class ForegroundTask : IBackgroundTask
            {
                private readonly Action _delegate;

                /// <summary>
                /// Initializes a new instance of the <see cref="ForegroundTask"/> class.
                /// </summary>
                /// <param name="delegate">The @delegate.</param>
                public ForegroundTask(Action @delegate)
                {
                    _delegate = @delegate;
                }

                /// <summary>
                /// Enqueues the task with the specified user state.
                /// </summary>
                /// <param name="userState">The user supplied state.</param>
                public void Enqueue(object userState)
                {
                    if (Starting != null)
                        Starting(this, EventArgs.Empty);

                    _delegate();

                    if (ProgressChanged != null)
                        ProgressChanged(
                            this,
                            new BackgroundTaskProgressChangedEventArgs(null, 1)
                            );

                    if(Completed != null)
                        Completed(
                            this,
                            new BackgroundTaskCompletedEventArgs(
                                null,
                                null,
                                false,
                                null
                                )
                            );
                }

                /// <summary>
                /// Cancels the task.
                /// </summary>
                public void Cancel()
                {
                }

                /// <summary>
                /// Gets a value indicating whether this task is currently processing.
                /// </summary>
                /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
                public bool IsBusy
                {
                    get { return true; }
                }

                /// <summary>
                /// Gets a value indicating whether this task's cancellation is pending.
                /// </summary>
                /// <value><c>true</c> if cancellation is pending; otherwise, <c>false</c>.</value>
                public bool CancellationPending
                {
                    get { return false; }
                }

                /// <summary>
                /// Occurs on the background thread just prior to the execution of work.
                /// </summary>
                public event EventHandler Starting;

                /// <summary>
                /// Occurs when the background task indicates that progress has changed.
                /// </summary>
                public event EventHandler<BackgroundTaskProgressChangedEventArgs> ProgressChanged;

                /// <summary>
                /// Occurs when the background task has completed either successfully, by cancellation or with an error.
                /// </summary>
                public event EventHandler<BackgroundTaskCompletedEventArgs> Completed;
            }

            private class FakeDispatcherOperation : IDispatcherOperation
            {
                private readonly Action _action;

                public FakeDispatcherOperation(Action action)
                {
                    _action = action;
                }

                public bool Abort()
                {
                    Aborted(this, EventArgs.Empty);
                    return true;
                }

                public void Wait()
                {
                    _action();
                    Completed(this, EventArgs.Empty);
                }

                public object Result
                {
                    get { return null; }
                }

                public event EventHandler Aborted = delegate { };
                public event EventHandler Completed = delegate { };
            }
        }
    }
}