namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;
    using PresentationFramework.Views;

    public class PopupResult<TPopup> : OpenResultBase<TPopup>
    {
        private readonly Func<ResultExecutionContext, TPopup> _locateModal = 
            c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TPopup>();

        public PopupResult() {}

        public PopupResult(TPopup child)
        {
            _locateModal = c => child;
        }

        public override void Execute(ResultExecutionContext context)
        {
            var child = _locateModal(context);

            if (_onConfigure != null)
                _onConfigure(child);

            var deactivator = child as IDeactivate;
            if (deactivator != null)
            {
                deactivator.Deactivated +=
                    (s, e) =>{
                        if(!e.WasClosed)
                            return;

                        if (_onClose != null)
                            _onClose(child);

                        OnCompleted(null, false);
                    };
            }

            var target = (UIElement)context.Message.Source.UIElement;
            var view = context.ServiceLocator.GetInstance<IViewLocator>().Locate(child, null, null);
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