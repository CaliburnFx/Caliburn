namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Windows;
    using Core.InversionOfControl;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    /// <summary>
    /// An <see cref="IResult"/> for showing popups.
    /// </summary>
    /// <typeparam name="TPopup">The type of the popup.</typeparam>
    public class PopupResult<TPopup> : OpenResultBase<TPopup>
    {
        readonly Func<ResultExecutionContext, TPopup> locateModel = 
            c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TPopup>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupResult&lt;TPopup&gt;"/> class.
        /// </summary>
        public PopupResult() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupResult&lt;TPopup&gt;"/> class.
        /// </summary>
        /// <param name="viewModel">The ViewModel.</param>
        public PopupResult(TPopup viewModel)
        {
            locateModel = c => viewModel;
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute(ResultExecutionContext context)
        {
            var viewModel = locateModel(context);

            if (onConfigure != null)
                onConfigure(viewModel);

            var deactivator = viewModel as IDeactivate;
            if (deactivator != null && onClose != null)
            {
                EventHandler<DeactivationEventArgs> handler = null;
                handler = (s2, e2) =>{
                    if(!e2.WasClosed)
                        return;

                    deactivator.Deactivated -= handler;
                    onClose(viewModel);
                };

                deactivator.Deactivated += handler;
            }

            var target = (UIElement)context.Message.Source.UIElement;
            var windowManager = context.ServiceLocator.GetInstance<IWindowManager>();

            windowManager.ShowPopup(viewModel, target);

            OnCompleted(null, false);
        }
    }
}