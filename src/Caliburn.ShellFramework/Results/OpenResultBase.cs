namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework.RoutedMessaging;

    public abstract class OpenResultBase<TTarget> : IOpenResult<TTarget>
    {
        protected Action<TTarget> _onConfigure;
        protected Action<TTarget> _onClose;

        Action<TTarget> IOpenResult<TTarget>.OnConfigure
        {
            get { return _onConfigure; }
            set { _onConfigure = value; }
        }

        Action<TTarget> IOpenResult<TTarget>.OnClose
        {
            get { return _onClose; }
            set { _onClose = value; }
        }

        public abstract void Execute(ResultExecutionContext context);

        protected virtual void OnCompleted(Exception exception, bool wasCancelled)
        {
            Completed(this, new ResultCompletionEventArgs{ Error = exception, WasCancelled = wasCancelled});
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}