namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    public class DialogScreenSubjectResult : OpenResultBase<IScreen>
    {
        private readonly IScreenSubject _screenSubject;
        private Action<ISubordinate, Action> _handleShutdown;

        public DialogScreenSubjectResult(IScreenSubject screenSubject)
        {
            _screenSubject = screenSubject;
        }

        public DialogScreenSubjectResult HandleShutdownWith(Action<ISubordinate, Action> handler)
        {
            _handleShutdown = handler;
            return this;
        }

        public override void Execute(ResultExecutionContext context)
        {
            _screenSubject.CreateScreen(context.ServiceLocator.GetInstance<IViewModelFactory>(), screen =>{
                if(_onConfigure != null)
                    _onConfigure(screen);

                var notifier = screen as ILifecycleNotifier;
                if(notifier != null)
                {
                    notifier.WasShutdown +=
                        (s, e) =>{
                            if(_onShutDown != null)
                                _onShutDown(screen);

                            OnCompleted(null, false);
                        };
                }

                context.ServiceLocator.GetInstance<IWindowManager>()
                    .ShowDialog(screen, null, _handleShutdown);

                if(notifier == null)
                    OnCompleted(null, false);
            });
        }
    }
}