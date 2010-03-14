namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
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

            if (notifier == null)
                OnCompleted(null, false);
        }
    }
}