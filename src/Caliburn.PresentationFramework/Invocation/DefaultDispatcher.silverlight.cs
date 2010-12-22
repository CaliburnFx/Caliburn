#if SILVERLIGHT

namespace Caliburn.PresentationFramework.Invocation
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using Core.Invocation;

    /// <summary>
    /// An implementation of <see cref="IDispatcher"/> for Silverlight.
    /// </summary>
    public class DefaultDispatcher : IDispatcher
    {
        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDispatcher"/> class.
        /// </summary>
        public DefaultDispatcher()
        {
            _dispatcher = GetDispatcher();
        }

        /// <summary>
        /// Gets the dispatcher.
        /// </summary>
        /// <returns></returns>
        protected virtual Dispatcher GetDispatcher()
        {
            return Deployment.Current.Dispatcher;
        }

        /// <summary>
        /// Executes code on the background thread.
        /// </summary>
        /// <param name="backgroundAction">The background action.</param>
        /// <param name="uiCallback">The UI callback.</param>
        /// <param name="progressChanged">The progress change callback.</param>
        public IBackgroundTask ExecuteOnBackgroundThread(Action backgroundAction, RunWorkerCompletedEventHandler uiCallback, ProgressChangedEventHandler progressChanged)
        {
            var task = new BackgroundTask(
                () =>{
                    backgroundAction();
                    return null;
                });

            if (uiCallback != null)
                task.Completed += (s, e) => ExecuteOnUIThread(() => uiCallback(s, e));

            if (progressChanged != null)
                task.ProgressChanged += (s, e) => ExecuteOnUIThread(() => progressChanged(s, e));

            task.Start(null);

            return task;
        }

        /// <summary>
        /// Executes code on the UI thread.
        /// </summary>
        /// <param name="uiAction">The UI action.</param>
        public void ExecuteOnUIThread(Action uiAction)
        {
            if (_dispatcher.CheckAccess())
                uiAction();
            else
            {
                var waitHandle = new ManualResetEvent(false);
                Exception exception = null;
                _dispatcher.BeginInvoke(() => {
                    try
                    {
                        uiAction();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                    waitHandle.Set();
                });
                waitHandle.WaitOne();
                if (exception != null)
                    throw new TargetInvocationException("An error occurred while dispatching a call to the UI Thread", exception);
            } 
        }

        /// <summary>
        /// Executes code on the UI thread asynchronously.
        /// </summary>
        /// <param name="uiAction">The UI action.</param>
        public IDispatcherOperation BeginExecuteOnUIThread(Action uiAction)
        {
            var operation = _dispatcher.BeginInvoke(uiAction);
            return new DispatcherOperationProxy(operation);
        }
    }
}
#endif