namespace BackgroundProcessing {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Core.InversionOfControl;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.RoutedMessaging;
    using Caliburn.PresentationFramework.Screens;
    using Framework;

    //Register this class as implementation of IRunningActionsRegistry
    [Singleton(typeof(IRunningActionsRegistry))]
    public class RunningActionsViewModel : Screen, IRunningActionsRegistry {
        readonly BindableCollection<IRunningAction> runningActions = new BindableCollection<IRunningAction>();

        public bool HasRunningActions {
            get { return RunningActions.Count() > 0; }
        }

        public IEnumerable<IRunningAction> RunningActions {
            get { return runningActions; }
        }

        public void RegisterTask(IRunningAction action) {
            if(action == null)
                throw new ArgumentNullException("action");

            if(!runningActions.Contains(action)) {
                runningActions.Insert(0, action);
                OnRunningActionRegistered(action);
                NotifyOfPropertyChange("HasRunningActions");
            }
        }

        public void UnregisterTask(IRunningAction action) {
            if(action == null)
                throw new ArgumentNullException("action");

            if(runningActions.Contains(action)) {
                runningActions.Remove(action);
                OnRunningActionUnregistered(action);
                NotifyOfPropertyChange("HasRunningActions");
            }
        }

        protected virtual void OnRunningActionRegistered(IRunningAction action) {}
        protected virtual void OnRunningActionUnregistered(IRunningAction action) {}

        public void CancelAll() {
            foreach(var action in RunningActions.ToArray()) {
                if(action.IsCancellable & !action.CancellationPending)
                    action.Cancel();
            }
        }

        public IResult ShowDetails() {
            return new ShowPopupResult();
        }
    }
}