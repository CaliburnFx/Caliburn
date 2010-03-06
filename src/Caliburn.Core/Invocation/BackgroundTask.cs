namespace Caliburn.Core.Invocation
{
    using System;
    using System.ComponentModel;

    public class BackgroundTask : IBackgroundTask
    {
        [ThreadStatic]
        private static IBackgroundContext _currentContext;

        private readonly BackgroundWorker _worker;
        private readonly BackgroundContext _context;
        private bool _cancellationPending;

        public BackgroundTask(Func<object> theDelegate)
        {
            _worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };

            _context = new BackgroundContext(_worker);

            _worker.DoWork += (s, e) =>{
                CurrentContext = _context;

                Starting(this, EventArgs.Empty);

                e.Result = theDelegate();
                e.Cancel = _cancellationPending;

                CurrentContext = null;
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

        public void Start(object userState)
        {
            _worker.RunWorkerAsync(userState);
        }

        public void Cancel()
        {
            _cancellationPending = true;
            _worker.CancelAsync();
        }

        public bool IsBusy
        {
            get { return _worker.IsBusy; }
        }

        public bool CancellationPending
        {
            get { return _worker.CancellationPending; }
        }

        public event EventHandler Starting = delegate { };

        public event ProgressChangedEventHandler ProgressChanged
        {
            add { _worker.ProgressChanged += value; }
            remove { _worker.ProgressChanged -= value; }
        }

        public event RunWorkerCompletedEventHandler Completed
        {
            add { _worker.RunWorkerCompleted += value; }
            remove { _worker.RunWorkerCompleted -= value; }
        }

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
        }
    }
}