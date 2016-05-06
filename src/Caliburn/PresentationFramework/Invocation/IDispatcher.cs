namespace Caliburn.PresentationFramework.Invocation
{
    using System;
    using System.ComponentModel;
    using System.Windows.Threading;
    using Core.Invocation;

    /// <summary>
    /// Abstracts a dispatcher capable of executing code on a background or UI thread.
    /// </summary>
    public interface IDispatcher
    {
#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the default dispatcher priority.
        /// </summary>
        /// <value>The default priority.</value>
        DispatcherPriority DefaultPriority { get; set; }
#endif

        /// <summary>
        /// Executes code on the background thread.
        /// </summary>
        /// <param name="backgroundAction">The background action.</param>
        /// <param name="uiCallback">The UI callback.</param>
        /// <param name="progressChanged">The progress change callback.</param>
        IBackgroundTask ExecuteOnBackgroundThread(Action backgroundAction, RunWorkerCompletedEventHandler uiCallback, ProgressChangedEventHandler progressChanged);

        /// <summary>
        /// Executes code on the UI thread.
        /// </summary>
        /// <param name="uiAction">The UI action.</param>
        void ExecuteOnUIThread(Action uiAction);

        /// <summary>
        /// Executes code on the UI thread asynchronously.
        /// </summary>
        /// <param name="uiAction">The UI action.</param>
        IDispatcherOperation BeginExecuteOnUIThread(Action uiAction);
    }
}