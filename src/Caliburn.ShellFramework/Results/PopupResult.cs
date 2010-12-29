namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using Core.InversionOfControl;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;
    using PresentationFramework.Views;

    /// <summary>
    /// An <see cref="IResult"/> for showing popups.
    /// </summary>
    /// <typeparam name="TPopup">The type of the popup.</typeparam>
    public class PopupResult<TPopup> : OpenResultBase<TPopup>
    {
        private readonly Func<ResultExecutionContext, TPopup> locateModal = 
            c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TPopup>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupResult&lt;TPopup&gt;"/> class.
        /// </summary>
        public PopupResult() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupResult&lt;TPopup&gt;"/> class.
        /// </summary>
        /// <param name="child">The child.</param>
        public PopupResult(TPopup child)
        {
            locateModal = c => child;
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute(ResultExecutionContext context)
        {
            var child = locateModal(context);

            if (onConfigure != null)
                onConfigure(child);

            var deactivator = child as IDeactivate;
            if (deactivator != null)
            {
                deactivator.Deactivated +=
                    (s, e) =>{
                        if(!e.WasClosed)
                            return;

                        if (onClose != null)
                            onClose(child);

                        OnCompleted(null, false);
                    };
            }

            var target = (UIElement)context.Message.Source.UIElement;
            var view = context.ServiceLocator.GetInstance<IViewLocator>().LocateForModel(child, null, null);
            var popup = view as Popup;
            
            if(popup == null)
            {
                popup = PopupConfiguration.FindOrCreatePopupFor(target);

                if(popup.Child == null)
                    popup.Child = (UIElement)view;
            }
#if !SILVERLIGHT
            else popup.PlacementTarget = target;
#endif

            context.ServiceLocator.GetInstance<IViewModelBinder>().Bind(child, popup, null);

            popup.IsOpen = true;
            popup.CaptureMouse();

            if (deactivator == null)
                OnCompleted(null, false);
        }
    }
}