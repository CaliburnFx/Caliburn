namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    public class OpenDialogResult<TDialog> : OpenResultBase<TDialog>
    {
        private readonly Func<ResultExecutionContext, TDialog> _locateModal = 
            c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TDialog>();

        public OpenDialogResult() {}

        public OpenDialogResult(TDialog child)
        {
            _locateModal = c => child;
        }

        public override void Execute(ResultExecutionContext context)
        {
            var dialogManager = context.ServiceLocator.GetInstance<IWindowManager>();
            var child = _locateModal(context);

            if(_onConfigure != null)
                _onConfigure(child);

            var deactivator = child as IDeactivate;
            if (deactivator != null)
            {
                deactivator.Deactivated += (s, e) =>{
                    if(!e.WasClosed)
                        return;

                    if (_onClose != null)
                        _onClose(child);

                    OnCompleted(null, false);
                };
            }

            dialogManager.ShowDialog(child, null);

            if(deactivator == null)
                OnCompleted(null, false);
        }
    }
}