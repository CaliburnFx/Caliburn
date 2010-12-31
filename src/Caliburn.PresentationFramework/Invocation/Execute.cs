namespace Caliburn.PresentationFramework.Invocation
{
    using System;
    using System.ComponentModel;
    using System.Windows.Threading;
    using Core.Invocation;

    /// <summary>
    /// Utility methods supporting various forms of method execution.
    /// </summary>
    public static class Execute
    {
        static IDispatcher dispatcher = new FakeDispatcher();

        /// <summary>
        /// Initializes for execution.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        public static void Initialize(IDispatcher dispatcher)
        {
            Execute.dispatcher = dispatcher ?? new FakeDispatcher();
        }

        /// <summary>
        /// Executes the backgroundAction on a background thread.
        /// </summary>
        /// <param name="backgroundAction">The action.</param>
        public static IBackgroundTask OnBackgroundThread(Action backgroundAction)
        {
            return dispatcher.ExecuteOnBackgroundThread(backgroundAction, null, null);
        }

        /// <summary>
        /// Executes the backgroundAction on a background thread.
        /// </summary>
        /// <param name="backgroundAction">The action.</param>
        /// <param name="uiCallback">The UI callback.</param>
        public static IBackgroundTask OnBackgroundThread(Action backgroundAction, RunWorkerCompletedEventHandler uiCallback)
        {
            return dispatcher.ExecuteOnBackgroundThread(backgroundAction, uiCallback, null);
        }

        /// <summary>
        /// Executes the backgroundAction on a background thread.
        /// </summary>
        /// <param name="backgroundAction">The action.</param>
        /// <param name="uiCallback">The UI callback.</param>
        /// <param name="progressChanged">The progress change callback.</param>
        public static IBackgroundTask OnBackgroundThread(Action backgroundAction, RunWorkerCompletedEventHandler uiCallback, ProgressChangedEventHandler progressChanged)
        {
            return dispatcher.ExecuteOnBackgroundThread(backgroundAction, uiCallback, progressChanged);
        }

        /// <summary>
        /// Executes the backgroundAction on a background thread.
        /// </summary>
        /// <param name="backgroundAction">The action.</param>
        /// <param name="progressChanged">The progress change callback.</param>
        public static IBackgroundTask OnBackgroundThread(Action backgroundAction, ProgressChangedEventHandler progressChanged)
        {
            return dispatcher.ExecuteOnBackgroundThread(backgroundAction, null, progressChanged);
        }

        /// <summary>
        /// Executes the action on the UI thread.
        /// </summary>
        /// <param name="action">The action.</param>
        public static void OnUIThread(Action action)
        {
            dispatcher.ExecuteOnUIThread(action);
        }

        /// <summary>
        /// Executes the action on the UI thread asynchronously.
        /// </summary>
        /// <param name="action">The action.</param>
        public static IDispatcherOperation OnUIThreadAsync(Action action)
        {
            return dispatcher.BeginExecuteOnUIThread(action);
        }

        private class FakeDispatcher : IDispatcher
        {
#if !SILVERLIGHT
            public DispatcherPriority DefaultPriority { get; set; }
#endif

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

            public void ExecuteOnUIThread(Action uiAction)
            {
                uiAction();
            }

            public IDispatcherOperation BeginExecuteOnUIThread(Action uiAction)
            {
                return new FakeDispatcherOperation(uiAction);
            }

            private class ForegroundTask : IBackgroundTask
            {
                readonly Action @delegate;

                public ForegroundTask(Action @delegate)
                {
                    this.@delegate = @delegate;
                }

                public void Start(object userState)
                {
                    if (Starting != null)
                        Starting(this, EventArgs.Empty);

                    @delegate();

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

                public void Cancel()
                {
                }

                public bool IsBusy
                {
                    get { return true; }
                }

                public bool CancellationPending
                {
                    get { return false; }
                }

                public event EventHandler Starting;
                public event ProgressChangedEventHandler ProgressChanged;
                public event RunWorkerCompletedEventHandler Completed;
            }

            private class FakeDispatcherOperation : IDispatcherOperation
            {
                readonly Action action;

                public FakeDispatcherOperation(Action action)
                {
                    this.action = action;
                }

                public bool Abort()
                {
                    Aborted(this, EventArgs.Empty);
                    return true;
                }

                public void Wait()
                {
                    action();
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