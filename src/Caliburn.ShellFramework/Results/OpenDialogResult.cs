namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    public class OpenDialogResult<TDialog> : OpenResultBase<TDialog>
    {
        private readonly Func<ResultExecutionContext, TDialog> locateModal = 
            c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TDialog>();

#if !SILVERLIGHT
        public bool? DialogResult { get; set; }
#endif

        public OpenDialogResult() {}

        public OpenDialogResult(TDialog child)
        {
            locateModal = c => child;
        }

        public override void Execute(ResultExecutionContext context)
        {
            var dialogManager = context.ServiceLocator.GetInstance<IWindowManager>();
            var child = locateModal(context);

            if(onConfigure != null)
                onConfigure(child);

            var deactivator = child as IDeactivate;
            if (deactivator != null)
            {
                deactivator.Deactivated += (s, e) =>{
                    if(!e.WasClosed)
                        return;

                    if (onClose != null)
                        onClose(child);

                    OnCompleted(null, false);
                };
            }

#if !SILVERLIGHT
            DialogResult = dialogManager.ShowDialog(child, null);
#else
            dialogManager.ShowDialog(child, null);
#endif

            if (deactivator == null)
                OnCompleted(null, false);
        }
    }
}