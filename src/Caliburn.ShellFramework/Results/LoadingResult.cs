namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework;
    using Services;

    public class LoadingResult : IResult
    {
        private readonly bool _show;
        private readonly string _message;

        public LoadingResult(bool show, string message)
        {
            _show = show;
            _message = message;
        }

        public void Execute(ResultExecutionContext context)
        {
            object viewModel = null;

            if(context.HandlingNode != null)
                viewModel = context.HandlingNode.MessageHandler.Unwrap();

            if(_show)
                context.ServiceLocator.GetInstance<ILoader>().StartLoading(viewModel, _message);
            else context.ServiceLocator.GetInstance<ILoader>().StopLoading(viewModel);

            Completed(this, new ResultCompletionEventArgs());
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}