namespace Caliburn.Core.Invocation
{
    using System;
    using System.ComponentModel;
    using Logging;

    public class BackgroundTask : IBackgroundTask
    {
        [ThreadStatic]
        private static IBackgroundContext _currentContext;

        private static readonly ILog Log = LogManager.GetLog(typeof(BackgroundTask));

        private readonly BackgroundWorker _worker;
        private bool _cancellationPending;
        private readonly BackgroundContext _context;

        public BackgroundTask(Func<object> theDelegate)
        {
            _worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };

            _context = new BackgroundContext(_worker);

            _worker.DoWork += (s, e) =>{
                Log.Info("Starting background task.");

                CurrentContext = _context;
                Starting(this, EventArgs.Empty);

                e.Result = theDelegate();
                e.Cancel = _cancellationPending;

                CurrentContext = null;
                Log.Info("Completed background task.");
            };

            _worker.ProgressChanged += (s, e) =>{
                ProgressChanged(this, new ProgressChangedEventArgs(e.ProgressPercentage, e.UserState));
            };

            _worker.RunWorkerCompleted += (s, e) =>{
                Completed(this, new RunWorkerCompletedEventArgs(e.Error != null || e.Cancelled ? null : e.Result, e.Error, e.Cancelled));
            };
        }

        /// <summary>
        /// Gets or sets the context for the currently executing task.
        /// </summary>
        /// <value>The current context.</value>
        public static IBackgroundContext CurrentContext
        {
            get { return _currentContext; }
            set { _currentContext = value; }
        }

        /// <summary>
        /// Enqueues the task with the specified user state.
        /// </summary>
        /// <param name="userState">The user supplied state.</param>
        public void Start(object userState)
        {
            Log.Info("Queueing background task.");
            _worker.RunWorkerAsync(userState);
        }

        /// <summary>
        /// Cancels the task.
        /// </summary>
        public void Cancel()
        {
            _cancellationPending = true;
            Log.Info("Cancelling background task.");
            _worker.CancelAsync();
        }

        /// <summary>
        /// Gets a value indicating whether this task is currently processing.
        /// </summary>
        /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
        public bool IsBusy
        {
            get { return _worker.IsBusy; }
        }

        /// <summary>
        /// Gets a value indicating whether this task's cancellation is pending.
        /// </summary>
        /// <value><c>true</c> if cancellation is pending; otherwise, <c>false</c>.</value>
        public bool CancellationPending
        {
            get { return _worker.CancellationPending; }
        }

        /// <summary>
        /// Occurs on the background thread just prior to the execution of work.
        /// </summary>
        public event EventHandler Starting = delegate { };

        /// <summary>
        /// Occurs on the UI thread when the background task indicates that progress has changed.
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged = delegate { };

        /// <summary>
        /// Occurs on the UI thread when the background task has completed either successfully, by cancellation or with an error.
        /// </summary>
        public event RunWorkerCompletedEventHandler Completed = delegate { };

        /// <summary>
        /// A simple implementation of <see cref="IBackgroundContext"/>.
        /// </summary>
        private class BackgroundContext : IBackgroundContext
        {
            private readonly BackgroundWorker _worker;

            /// <summary>
            /// Initializes a new instance of the <see cref="BackgroundContext"/> class.
            /// </summary>
            public BackgroundContext(BackgroundWorker worker)
            {
                _worker = worker;
            }

            /// <summary>
            /// Gets a value indicating whether the current task has been cancelled.
            /// </summary>
            /// <value><c>true</c> if cancelled; otherwise, <c>false</c>.</value>
            public bool CancellationPending
            {
                get { return _worker.CancellationPending; }
            }

            /// <summary>
            /// Enables the current task to update its progress.
            /// </summary>
            /// <param name="percentage">The percentage.</param>
            public void ReportProgress(int percentage)
            {
                _worker.ReportProgress(percentage);
            }

            /// <summary>
            /// Enables the current task to update its progress.
            /// </summary>
            /// <param name="percentage">The percentage.</param>
            /// <param name="userState"></param>
            public void ReportProgress(int percentage, object userState)
            {
                _worker.ReportProgress(percentage, userState);
            }
        }
    }
}