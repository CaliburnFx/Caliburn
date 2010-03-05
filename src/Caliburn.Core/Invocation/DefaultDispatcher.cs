namespace Caliburn.Core.Invocation
{
    using System;
    using System.ComponentModel;
    using Threading;

    /// <summary>
    /// A simple implementation of <see cref="IDispatcher"/> suitable only during test scenarios.
    /// </summary>
    /// <remarks>This implementation only executes code on the calling thread.</remarks>
    public class DefaultDispatcher : IDispatcher
    {
        /// <summary>
        /// Executes code on the background thread.
        /// </summary>
        /// <param name="backgroundAction">The background action.</param>
        /// <param name="uiCallback">The UI callback.</param>
        /// <param name="progressChanged">The progress change callback.</param>
        public IBackgroundTask ExecuteOnBackgroundThread(Action backgroundAction, RunWorkerCompletedEventHandler uiCallback, ProgressChangedEventHandler progressChanged)
        {
            var task = new ForegroundTask(backgroundAction);

            if (uiCallback != null)
                task.Completed += uiCallback;

            if (progressChanged != null)
                task.ProgressChanged += progressChanged;

            task.Start(null);

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
        /// Used by the <see cref="DefaultDispatcher"/> to execute background tasks on the main thread.
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
            public void Start(object userState)
            {
                if (Starting != null)
                    Starting(this, EventArgs.Empty);

                _delegate();

                if (ProgressChanged != null)
                    ProgressChanged(
                        this,
                        new ProgressChangedEventArgs(100, userState)
                        );

                if (Completed != null)
                    Completed(
                        this,
                        new RunWorkerCompletedEventArgs(null, null, false)
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
            public event ProgressChangedEventHandler ProgressChanged;

            /// <summary>
            /// Occurs when the background task has completed either successfully, by cancellation or with an error.
            /// </summary>
            public event RunWorkerCompletedEventHandler Completed;
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