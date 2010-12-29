namespace Caliburn.ShellFramework.Results
{
    using Core.InversionOfControl;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    /// <summary>
    /// An <see cref="IResult"/> for showing modal dialogs based on an <see cref="ISubjectSpecification"/>.
    /// </summary>
    public class DialogScreenSubjectResult : OpenResultBase<object>
    {
        readonly ISubjectSpecification subjectSpecification;

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the dialog result.
        /// </summary>
        /// <value>The dialog result.</value>
        public bool? DialogResult { get; set; }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogScreenSubjectResult"/> class.
        /// </summary>
        /// <param name="subjectSpecification">The subject specification.</param>
        public DialogScreenSubjectResult(ISubjectSpecification subjectSpecification)
        {
            this.subjectSpecification = subjectSpecification;
        }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
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