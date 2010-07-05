namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    public class OpenDialogResult<TDialog> : OpenResultBase<TDialog>
        where TDialog : IScreen
    {
        private readonly Func<ResultExecutionContext, TDialog> _locateModal = 
            c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TDialog>();

        private Action<ISubordinate, Action> _handleShutdown;

        public OpenDialogResult() {}

        public OpenDialogResult(TDialog child)
        {
            _locateModal = c => child;
        }

        public OpenDialogResult<TDialog> HandleShutdownWith(Action<ISubordinate, Action> handler)
        {
            _handleShutdown = handler;
            return this;
        }

        public override void Execute(ResultExecutionContext context)
        {
            var dialogManager = context.ServiceLocator.GetInstance<IWindowManager>();
            var child = _locateModal(context);

            if(_onConfigure != null)
                _onConfigure(child);

            var notifier = child as ILifecycleNotifier;
            if (notifier != null)
            {
                notifier.WasShutdown +=
                    (s, e) =>{
                        if(_onShutDown != null)
                            _onShutDown(child);

                        OnCompleted(null, false);
                    };
            }

            dialogManager.ShowDialog(child, null, _handleShutdown);

            if(notifier == null)
                OnCompleted(null, false);
        }
    }
}