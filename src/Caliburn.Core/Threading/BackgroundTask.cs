namespace Caliburn.Core.Threading
{
    using System;

    /// <summary>
    /// An implementation of <see cref="IBackgroundTask"/>.
    /// </summary>
    public class BackgroundTask : IBackgroundTask
    {
        private readonly IThreadPool _threadPool;
        private readonly Func<object> _theDelegate;
        private bool _isBusy;
        private readonly BackgroundContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundTask"/> class.
        /// </summary>
        /// <param name="threadPool">The thread pool.</param>
        /// <param name="theDelegate">The delegate to execute.</param>
        public BackgroundTask(IThreadPool threadPool, Func<object> theDelegate)
        {
            _threadPool = threadPool;
            _theDelegate = theDelegate;
            _context = new BackgroundContext(this);
        }

        /// <summary>
        /// Gets a value indicating whether this task is currently processing.
        /// </summary>
        /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
        public bool IsBusy
        {
            get { return _isBusy; }
        }

        /// <summary>
        /// Gets a value indicating whether this task's cancellation is pending.
        /// </summary>
        /// <value><c>true</c> if cancellation is pending; otherwise, <c>false</c>.</value>
        public bool CancellationPending
        {
            get { return _context.IsCancelled; }
        }

        /// <summary>
        /// Enqueues the task with the specified user state.
        /// </summary>
        /// <param name="userState">The user supplied state.</param>
        public void Enqueue(object userState)
        {
            _threadPool.QueueUserWorkItem(
                state =>{
                    try
                    {
                        if(_context.IsCancelled)
                        {
                            Completed(
                                this,
                                new BackgroundTaskCompletedEventArgs(null, userState, _context.IsCancelled, null)
                                );
                        }
                        else
                        {
                            _context.Initialize(userState);

                            Starting(this, EventArgs.Empty);

                            _isBusy = true;
                            var result = _theDelegate();
                            _isBusy = false;

                            Completed(
                                this,
                                new BackgroundTaskCompletedEventArgs(result, userState, _context.IsCancelled, null)
                                );
                        }
                    }
                    catch(Exception e)
                    {
                        _isBusy = false;

                        Completed(
                            this,
                            new BackgroundTaskCompletedEventArgs(null, userState, _context.IsCancelled, e)
                            );
                    }
                    finally
                    {
                        _context.Teardown();
                    }
                }, userState);
        }

        /// <summary>
        /// Cancels the task.
        /// </summary>
        public void Cancel()
        {
            _context.IsCancelled = true;
        }

        /// <summary>
        /// Occurs on the background thread just prior to the execution of work.
        /// </summary>
        public event EventHandler Starting = delegate { };

        /// <summary>
        /// Occurs when the background task indicates that progress has changed.
        /// </summary>
        public event EventHandler<BackgroundTaskProgressChangedEventArgs> ProgressChanged = delegate { };

        /// <summary>
        /// Occurs when the background task has completed either successfully, by cancellation or with an error.
        /// </summary>
        public event EventHandler<BackgroundTaskCompletedEventArgs> Completed = delegate { };

        /// <summary>
        /// A simple implementation of <see cref="IBackgroundContext"/>.
        /// </summary>
        private class BackgroundContext : IBackgroundContext
        {
            private readonly BackgroundTask _task;
            private object _userState;

            /// <summary>
            /// Initializes a new instance of the <see cref="BackgroundContext"/> class.
            /// </summary>
            /// <param name="task">The task.</param>
            public BackgroundContext(BackgroundTask task)
            {
                _task = task;
            }

            /// <summary>
            /// Initializes the specified user state.
            /// </summary>
            /// <param name="userState">State of the user.</param>
            public void Initialize(object userState)
            {
                _userState = userState;
                CurrentContext = this;
            }

            /// <summary>
            /// Tears down this instance.
            /// </summary>
            public void Teardown()
            {
                _userState = null;
                CurrentContext = null;
                IsCancelled = false;
            }

            /// <summary>
            /// Gets a value indicating whether the current task has been cancelled.
            /// </summary>
            /// <value><c>true</c> if cancelled; otherwise, <c>false</c>.</value>
            public bool IsCancelled { get; set; }

            /// <summary>
            /// Enables the current task to update its progress.
            /// </summary>
            /// <param name="percentage">The percentage.</param>
            public void UpdateProgress(double percentage)
            {
                _task.ProgressChanged(
                    _task,
                    new BackgroundTaskProgressChangedEventArgs(_userState, percentage)
                    );
            }
        }

        [ThreadStatic] private static IBackgroundContext _currentContext;

        /// <summary>
        /// Gets or sets the context for the currently executing task.
        /// </summary>
        /// <value>The current context.</value>
        public static IBackgroundContext CurrentContext
        {
            get { return _currentContext; }
            set { _currentContext = value; }
        }
    }
}