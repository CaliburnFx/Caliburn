namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Linq;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    public class OpenScreenSubjectResult : OpenResultBase<object>
    {
        private readonly ISubjectSpecification subjectSpecification;
        private Func<ResultExecutionContext, IConductor> locateParent;

        public OpenScreenSubjectResult(ISubjectSpecification subjectSpecification)
        {
            this.subjectSpecification = subjectSpecification;
        }

        public OpenScreenSubjectResult In<TParent>()
            where TParent : IConductor
        {
            locateParent = c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TParent>();
            return this;
        }

        public OpenScreenSubjectResult In(IConductor parent)
        {
            locateParent = c => parent;
            return this;
        }

        public override void Execute(ResultExecutionContext context)
        {
            if (locateParent == null)
                locateParent = c => (IConductor)c.HandlingNode.MessageHandler.Unwrap();

            var parent = locateParent(context);

            parent.ActivateSubject(subjectSpecification, success =>{
                if(success)
                {
                    var child = parent
                        .GetConductedItems()
                        .OfType<IHaveSubject>()
                        .FirstOrDefault(subjectSpecification.Matches);

                    if (_onConfigure != null)
                        _onConfigure(child);

                    OnOpened(parent, child);

                    var deactivator = child as IDeactivate;
                    if(deactivator != null)
                    {
                        deactivator.Deactivated += (s, e) =>{
                            if (!e.WasClosed)
                                return;

                            if (_onClose != null)
                                _onClose(child);

                            OnCompleted(null, false);
                        };
                    }
                    else OnCompleted(null, false);
                }
                else OnCompleted(null, true);
            });
        }

        protected virtual void OnOpened(IConductor parent, object child) { }
    }
}