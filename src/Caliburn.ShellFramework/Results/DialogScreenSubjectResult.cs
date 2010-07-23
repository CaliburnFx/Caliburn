namespace Caliburn.ShellFramework.Results
{
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    public class DialogScreenSubjectResult : OpenResultBase<object>
    {
        readonly ISubjectSpecification subjectSpecification;

        public DialogScreenSubjectResult(ISubjectSpecification subjectSpecification)
        {
            this.subjectSpecification = subjectSpecification;
        }

        public override void Execute(ResultExecutionContext context)
        {
            subjectSpecification.CreateSubjectHost(context.ServiceLocator.GetInstance<IViewModelFactory>(), host =>{
                if(_onConfigure != null)
                    _onConfigure(host);

                var deactivator = host as IDeactivate;
                if(deactivator != null)
                {
                    deactivator.Deactivated +=
                        (s, e) =>{
                            if(!e.WasClosed)
                                return;

                            if (_onClose != null)
                                _onClose(host);

                            OnCompleted(null, false);
                        };
                }

                context.ServiceLocator.GetInstance<IWindowManager>()
                    .ShowDialog(host, null);

                if(deactivator == null)
                    OnCompleted(null, false);
            });
        }
    }
}