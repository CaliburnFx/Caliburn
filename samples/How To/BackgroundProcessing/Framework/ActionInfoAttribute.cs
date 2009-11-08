namespace BackgroundProcessing.Framework
{
    using System;
    using Caliburn.Core.Metadata;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.Actions;
    using Caliburn.PresentationFramework.Filters;
    using Microsoft.Practices.ServiceLocation;

    public class ActionInfoAttribute : Attribute, IInitializable, IPreProcessor
    {
        private IRunningActionsRegistry _runningActionPresenter;
        public ActionInfoAttribute()
        {
            Title = "Working";
            IsIndeterminate = true;
            IsCancellable = false;
        }

        public ActionInfoAttribute(string title)
            : this()
        {
            Title = title;
        }

        public string Title { get; set; }
        public bool IsIndeterminate { get; set; }
        public bool IsCancellable { get; set; }
        public int Priority { get; set; }

        void IInitializable.Initialize(Type targetType, IMetadataContainer metadataContainer, IServiceLocator serviceLocator)
        {
            _runningActionPresenter = serviceLocator.GetInstance<IRunningActionsRegistry>();
        }

        bool IPreProcessor.AffectsTriggers
        {
            get { return false; }
        }

		bool IPreProcessor.Execute(IRoutedMessage message, IInteractionNode handlingNode, object[] parameters)
        {
            //TODO: support of syncronous tasks
            var bgTask = AsynchronousAction.CurrentTask;
            if(bgTask == null)
                throw new InvalidOperationException("ActionInfo attribute can only be used on asynchronous actions");

            var runningAction = new RunningAction(bgTask, IsIndeterminate, IsCancellable)
            {
                Title = Title
            };

            _runningActionPresenter.RegisterTask(runningAction);
            bgTask.Completed += (o, e) => _runningActionPresenter.UnregisterTask(runningAction);

            return true;
        }
    }
}