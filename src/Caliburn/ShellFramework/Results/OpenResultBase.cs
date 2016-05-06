namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework.RoutedMessaging;

    /// <summary>
    /// A base class for <see cref="IResult"/> instances that open view models.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    public abstract class OpenResultBase<TTarget> : IOpenResult<TTarget>
    {
        /// <summary>
        /// The configuration delegate.
        /// </summary>
        protected Action<TTarget> onConfigure;

        /// <summary>
        /// The close delegate.
        /// </summary>
        protected Action<TTarget> onClose;

        /// <summary>
        /// Gets or sets the configuration delegate.
        /// </summary>
        /// <value>The on configure.</value>
        Action<TTarget> IOpenResult<TTarget>.OnConfigure
        {
            get { return onConfigure; }
            set { onConfigure = value; }
        }

        /// <summary>
        /// Gets or sets the close delegate.
        /// </summary>
        /// <value>The on close.</value>
        Action<TTarget> IOpenResult<TTarget>.OnClose
        {
            get { return onClose; }
            set { onClose = value; }
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public abstract void Execute(ResultExecutionContext context);

        /// <summary>
        /// Called when completed.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="wasCancelled">if set to <c>true</c> indicates the result was cancelled.</param>
        protected virtual void OnCompleted(Exception exception, bool wasCancelled)
        {
            Completed(this, new ResultCompletionEventArgs{ Error = exception, WasCancelled = wasCancelled});
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}