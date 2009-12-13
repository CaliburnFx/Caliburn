namespace BackgroundProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Core.IoC;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Framework;

    //Register this class as implementation of IRunningActionsRegistry
    [Singleton(typeof(IRunningActionsRegistry))]
    //Binds the view for this presenter
    [View(typeof(RunningActionsView))]
    public class RunningActionsPresenter : Presenter, IRunningActionsRegistry
    {
        private readonly BindableCollection<IRunningAction> _runningActions = new BindableCollection<IRunningAction>();

        public bool HasRunningActions
        {
            get
            {
                return RunningActions.Count() > 0;    
            }
        }

        public IEnumerable<IRunningAction> RunningActions
        {
            get { return _runningActions; }
        }

        protected virtual void OnRunningActionRegistered(IRunningAction action) { }
        protected virtual void OnRunningActionUnregistered(IRunningAction action) { }

        public void RegisterTask(IRunningAction action)
        {
            if(action == null) 
                throw new ArgumentNullException("action");

            if(!_runningActions.Contains(action))
            {
                _runningActions.Insert(0, action);
                OnRunningActionRegistered(action);
                NotifyOfPropertyChange("HasRunningActions");
            }
        }

        public void UnregisterTask(IRunningAction action)
        {
            if(action == null) 
                throw new ArgumentNullException("action");

            if(_runningActions.Contains(action))
            {
                _runningActions.Remove(action);
                OnRunningActionUnregistered(action);
                NotifyOfPropertyChange("HasRunningActions");
            }
        }

        public void CancelAll()
        {
            foreach(var action in RunningActions.ToArray())
                if(action.IsCancellable & !action.CancellationPending)
                    action.Cancel();
        }

        public IResult ShowDetails()
        {
            return new ShowPopupResult();
        }
    }
}