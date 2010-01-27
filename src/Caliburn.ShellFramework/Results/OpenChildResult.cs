namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    public class OpenChildResult<TChild> : OpenResultBase<TChild>
        where TChild : IScreen
    {
        private Func<ResultExecutionContext, IScreenCollection> _locateParent;

        private readonly Func<ResultExecutionContext, TChild> _locateChild = 
            c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TChild>();

        public OpenChildResult() {}

        public OpenChildResult(TChild child)
        {
            _locateChild = c => child;
        }

        public OpenChildResult<TChild> In<TParent>()
            where TParent : IScreenCollection
        {
            _locateParent = 
                c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TParent>();

            return this;
        }

        public override void Execute(ResultExecutionContext context)
        {
            if(_locateParent == null)
                _locateParent = c => (IScreenCollection)c.HandlingNode.MessageHandler.Unwrap();

            var parent = _locateParent(context);
            var child = _locateChild(context);

            if(_onConfigure != null)
                _onConfigure(child);

            parent.OpenScreen(child, success =>{
                if(success)
                {
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