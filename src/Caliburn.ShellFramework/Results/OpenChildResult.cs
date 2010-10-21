namespace Caliburn.ShellFramework.Results
{
    using System;
    using Core.InversionOfControl;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    public class OpenChildResult<TChild> : OpenResultBase<TChild>
    {
        private Func<ResultExecutionContext, IConductor> locateParent;

        private readonly Func<ResultExecutionContext, TChild> locateChild =
            c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TChild>();

        public OpenChildResult() { }

        public OpenChildResult(TChild child)
        {
            locateChild = c => child;
        }

        public OpenChildResult<TChild> In<TParent>()
            where TParent : IConductor
        {
            locateParent = c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TParent>();
            return this;
        }

        public OpenChildResult<TChild> In(IConductor parent)
        {
            locateParent = c => parent;
            return this;
        }

        public override void Execute(ResultExecutionContext context)
        {
            if (locateParent == null)
                locateParent = c => (IConductor)c.HandlingNode.MessageHandler.Unwrap();

            var parent = locateParent(context);
            var child = locateChild(context);

            if (onConfigure != null)
                onConfigure(child);

            EventHandler<ActivationProcessedEventArgs> processed = null;
            processed = (s, e) =>{
                parent.ActivationProcessed -= processed;

                if(e.Success)
                {
                    OnOpened(parent, child);

                    var deactivator = child as IDeactivate;
                    if (deactivator != null && onClose != null)
                    {
                        deactivator.Deactivated += (s2, e2) =>{
                            if(e2.WasClosed)
                                onClose(child);
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