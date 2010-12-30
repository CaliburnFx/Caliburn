namespace Caliburn.ShellFramework.Results
{
    using System;
    using Core.InversionOfControl;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    /// <summary>
    /// Opens a view model as a dialog.
    /// </summary>
    /// <typeparam name="TDialog">The type of the dialog.</typeparam>
    public class OpenDialogResult<TDialog> : OpenResultBase<TDialog>
    {
        readonly Func<ResultExecutionContext, TDialog> locateModal = 
            c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TDialog>();

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the dialog result.
        /// </summary>
        /// <value>The dialog result.</value>
        public bool? DialogResult { get; set; }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenDialogResult&lt;TDialog&gt;"/> class.
        /// </summary>
        public OpenDialogResult() {}

        /// <summary>
        /// Initializes a new instance with the dialog view model to open.
        /// </summary>
        /// <param name="child">The child.</param>
        public OpenDialogResult(TDialog child)
        {
            locateModal = c => child;
        }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute(ResultExecutionContext context)
        {
            var child = locateModal(context);

            if(onConfigure != null)
                onConfigure(child);

            var deactivator = child as IDeactivate;
            if (deactivator != null)
            {
                EventHandler<DeactivationEventArgs> handler = null;
                handler = (s, e) =>{
                    if(!e.WasClosed)
                        return;

                    deactivator.Deactivated -= handler;

                    if(onClose != null)
                        onClose(child);

                    OnCompleted(null, false);
                };

                deactivator.Deactivated += handler;
            }

            var dialogManager = context.ServiceLocator.GetInstance<IWindowManager>();

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