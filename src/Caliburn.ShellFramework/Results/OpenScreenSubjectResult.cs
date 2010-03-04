namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Linq;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    public class OpenScreenSubjectResult : OpenResultBase<IScreen>
    {
        private readonly IScreenSubject _screenSubject;
        private Func<ResultExecutionContext, IScreenCollection> _locateParent;

        public OpenScreenSubjectResult(IScreenSubject screenSubject)
        {
            _screenSubject = screenSubject;
        }

        public OpenScreenSubjectResult In<TParent>()
            where TParent : IScreenCollection
        {
            _locateParent =
                c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TParent>();

            return this;
        }

        public override void Execute(ResultExecutionContext context)
        {
            if (_locateParent == null)
                _locateParent = c => (IScreenCollection)c.HandlingNode.MessageHandler.Unwrap();

            var parent = _locateParent(context);

            parent.OpenScreen(_screenSubject, success =>{
                if(success)
                {
                    var child = parent.Screens.FirstOrDefault(_screenSubject.Matches);

                    if (_onConfigure != null)
                        _onConfigure(child);

                    var notifier = child as ILifecycleNotifier;
                    if(notifier != null)
                    {
                        notifier.WasShutdown += delegate{
                            if(_onShutDown != null)
                                _onShutDown(child);

                            OnCompleted(null, false);
                        };
                    }
                    else OnCompleted(null, false);
                }
                else OnCompleted(null, true);
            });
        }
    }
}