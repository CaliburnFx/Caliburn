namespace Caliburn.Core.Invocation
{
    using System;
    using System.ComponentModel;
    using Logging;

    /// <summary>
    /// The default implementation of <see cref="IBackgroundTask"/>.
    /// </summary>
    public class BackgroundTask : IBackgroundTask
    {
        [ThreadStatic]
        static IBackgroundContext currentContext;

        static readonly ILog Log = LogManager.GetLog(typeof(BackgroundTask));

        readonly BackgroundWorker worker;
        bool cancellationPending;
        readonly BackgroundContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundTask"/> class.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        public BackgroundTask(Func<object> theDelegate)
        {
            worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };

            context = new BackgroundContext(worker);

            worker.DoWork += (s, e) =>{
                Log.Info("Starting background task.");

                CurrentContext = context;
                Starting(this, EventArgs.Empty);

                e.Result = theDelegate();
                e.Cancel = cancellationPending;

                CurrentContext = null;
                Log.Info("Completed background task.");
            };

            worker.ProgressChanged += (s, e) =>{
                ProgressChanged(this, new ProgressChangedEventArgs(e.ProgressPercentage, e.UserState));
            };

            worker.RunWorkerCompleted += (s, e) =>{
                Completed(this, new RunWorkerCompletedEventArgs(e.Error != null || e.Cancelled ? null : e.Result, e.Error, e.Cancelled));
            };
        }

        /// <summary>
        /// Gets or sets the context for the currently executing task.
        /// </summary>
        /// <value>The current context.</value>
        public static IBackgroundContext CurrentContext
        {
            get { return currentContext; }
            set { currentContext = value; }
        }

        /// <summary>
        /// Enqueues the task with the specified user state.
        /// </summary>
        /// <param name="userState">The user supplied state.</param>
        public void Start(object userState)
        {
            Log.Info("Queueing background task.");
            worker.RunWorkerAsync(userState);
        }

        /// <summary>
        /// Cancels the task.
        /// </summary>
        public void Cancel()
        {
            cancellationPending = true;
            Log.Info("Cancelling background task.");
            worker.CancelAsync();
        }

        /// <summary>
        /// Gets a value indicating whether this task is currently processing.
        /// </summary>
        /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
        public bool IsBusy
        {
            get { return worker.IsBusy; }
        }

        /// <summary>
        /// Gets a value indicating whether this task's cancellation is pending.
        /// </summary>
        /// <value><c>true</c> if cancellation is pending; otherwise, <c>false</c>.</value>
        public bool CancellationPending
        {
            get { return worker.CancellationPending; }
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
            readonly BackgroundWorker capturedWorker;

            /// <summary>
            /// Initializes a new instance of the <see cref="BackgroundContext"/> class.
            /// </summary>
            public BackgroundContext(BackgroundWorker worker)
            {
                capturedWorker = worker;
            }

            /// <summary>
            /// Gets a value indicating whether the current task has been cancelled.
            /// </summary>
            /// <value><c>true</c> if cancelled; otherwise, <c>false</c>.</value>
            public bool CancellationPending
            {
                get { return capturedWorker.CancellationPending; }
            }

            /// <summary>
            /// Enables the current task to update its progress.
            /// </summary>
            /// <param name="percentage">The percentage.</param>
            public void ReportProgress(int percentage)
            {
                capturedWorker.ReportProgress(percentage);
            }

            /// <summary>
            /// Enables the current task to update its progress.
            /// </summary>
            /// <param name="percentage">The percentage.</param>
            /// <param name="userState"></param>
            public void ReportProgress(int percentage, object userState)
            {
                capturedWorker.ReportProgress(percentage, userState);
            }
        }
    }
}