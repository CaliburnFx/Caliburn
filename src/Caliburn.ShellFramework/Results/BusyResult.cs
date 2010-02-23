namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework;
    using Services;

    public class BusyResult : IResult
    {
        private readonly bool _isBusy;
        private readonly object _busyViewModel;

        public BusyResult(bool isBusy, object busyViewModel)
        {
            _isBusy = isBusy;
            _busyViewModel = busyViewModel;
        }

        public void Execute(ResultExecutionContext context)
        {
            object sourceViewModel = null;

            if(context.HandlingNode != null)
                sourceViewModel = context.HandlingNode.MessageHandler.Unwrap();

            if(_isBusy)
                context.ServiceLocator.GetInstance<IBusyService>().MarkAsBusy(sourceViewModel, _busyViewModel);
            else context.ServiceLocator.GetInstance<IBusyService>().MarkAsNotBusy(sourceViewModel);

            Completed(this, new ResultCompletionEventArgs());
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}