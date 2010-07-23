namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    public class OpenChildResult<TChild> : OpenResultBase<TChild>
    {
        private Func<ResultExecutionContext, IConductor> _locateParent;

        private readonly Func<ResultExecutionContext, TChild> _locateChild =
            c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TChild>();

        public OpenChildResult() { }

        public OpenChildResult(TChild child)
        {
            _locateChild = c => child;
        }

        public OpenChildResult<TChild> In<TParent>()
            where TParent : IConductor
        {
            _locateParent = c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TParent>();
            return this;
        }

        public OpenChildResult<TChild> In(IConductor parent)
        {
            _locateParent = c => parent;
            return this;
        }

        public override void Execute(ResultExecutionContext context)
        {
            if (_locateParent == null)
                _locateParent = c => (IConductor)c.HandlingNode.MessageHandler.Unwrap();

            var parent = _locateParent(context);
            var child = _locateChild(context);

            if (_onConfigure != null)
                _onConfigure(child);

            EventHandler<ActivationProcessedEventArgs> processed = null;
            processed = (s, e) =>{
                parent.ActivationProcessed -= processed;

                if(e.Success)
                {
                    OnOpened(parent, child);

                    var deactivator = child as IDeactivate;
                    if (deactivator != null && _onClose != null)
                    {
                        deactivator.Deactivated += (s2, e2) =>{
                            if(e2.WasClosed)
                                _onClose(child);
                        };
                    }

                    OnCompleted(null, false);
                }
                else OnCompleted(null, true);
            };

            parent.ActivationProcessed += processed;
            parent.ActivateItem(child);
        }

        protected virtual void OnOpened(IConductor parent, TChild child) { }
    }
}