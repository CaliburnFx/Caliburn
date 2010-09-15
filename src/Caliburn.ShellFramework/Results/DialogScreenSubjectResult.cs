namespace Caliburn.ShellFramework.Results
{
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    public class DialogScreenSubjectResult : OpenResultBase<object>
    {
        readonly ISubjectSpecification subjectSpecification;

#if !SILVERLIGHT
        public bool? DialogResult { get; set; }
#endif

        public DialogScreenSubjectResult(ISubjectSpecification subjectSpecification)
        {
            this.subjectSpecification = subjectSpecification;
        }

        public override void Execute(ResultExecutionContext context)
        {
            subjectSpecification.CreateSubjectHost(context.ServiceLocator.GetInstance<IViewModelFactory>(), host =>{
                if(onConfigure != null)
                    onConfigure(host);

                var deactivator = host as IDeactivate;
                if(deactivator != null)
                {
                    deactivator.Deactivated +=
                        (s, e) =>{
                            if(!e.WasClosed)
                                return;

                            if (onClose != null)
                                onClose(host);

                            OnCompleted(null, false);
                        };
                }

#if !SILVERLIGHT
                DialogResult = context.ServiceLocator.GetInstance<IWindowManager>()
                    .ShowDialog(host, null);
#else
                context.ServiceLocator.GetInstance<IWindowManager>()
                    .ShowDialog(host, null);
#endif

                if (deactivator == null)
                    OnCompleted(null, false);
            });
        }
    }
}