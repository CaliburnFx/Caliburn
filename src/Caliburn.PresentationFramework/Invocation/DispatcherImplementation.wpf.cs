#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.Invocation
{
    using System;
    using System.Windows;
    using System.Windows.Threading;
    using Core.Invocation;
    using Core.Threading;

    /// <summary>
    /// An implementation of <see cref="IDispatcher"/> for WPF.
    /// </summary>
    public class DispatcherImplementation : IDispatcher
    {
        private readonly Dispatcher _dispatcher;
        private readonly IThreadPool _threadPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherImplementation"/> class.
        /// </summary>
        public DispatcherImplementation(IThreadPool threadPool)
        {
            _dispatcher = GetDispatcher();
            _threadPool = threadPool;
        }

        /// <summary>
        /// Gets the dispatcher.
        /// </summary>
        /// <returns></returns>
        protected virtual Dispatcher GetDispatcher()
        {
            if (Application.Current != null && Application.Current.Dispatcher != null)
                return Application.Current.Dispatcher;
            return Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// Executes code on the background thread.
        /// </summary>
        /// <param name="backgroundAction">The background action.</param>
        /// <param name="uiCallback">The UI callback.</param>
        /// <param name="progressChanged">The progress change callback.</param>
        public IBackgroundTask ExecuteOnBackgroundThread(Action backgroundAction, Action<BackgroundTaskCompletedEventArgs> uiCallback, Action<BackgroundTaskProgressChangedEventArgs> progressChanged)
        {
            var task = new BackgroundTask(
                _threadPool,
                () =>{
                    backgroundAction();
                    return null;
                });

            if (uiCallback != null)
                task.Completed += (s, e) => ExecuteOnUIThread(() => uiCallback(e));

            if (progressChanged != null)
                task.ProgressChanged += (s, e) => ExecuteOnUIThread(() => progressChanged(e));

            task.Enqueue(null);

            return task;
        }

        /// <summary>
        /// Executes code on the UI thread.
        /// </summary>
        /// <param name="uiAction">The UI action.</param>
        public void ExecuteOnUIThread(Action uiAction)
        {
            if(_dispatcher.CheckAccess())
                uiAction();
            else
            {
                _dispatcher.Invoke(
                    DispatcherPriority.Send,
                    uiAction
                    );
            }
        }

        /// <summary>
        /// Executes code on the UI thread asynchronously.
        /// </summary>
        /// <param name="uiAction">The UI action.</param>
        public IDispatcherOperation BeginExecuteOnUIThread(Action uiAction)
        {
            var operation = _dispatcher.BeginInvoke(
                uiAction,
                DispatcherPriority.Send
                );

            return new DispatcherOperationProxy(operation);
        }
    }
}

#endif