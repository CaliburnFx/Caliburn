namespace Caliburn.Silverlight.NavigationShell.Shell.ViewModels
{
    using System;
    using System.Linq;
    using Framework;
    using PresentationFramework;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.Screens;
    using ShellFramework.History;
    using ShellFramework.Questions;
    using ShellFramework.Results;

    public class ShellViewModel : ScreenConductor<IScreen>, IShell
    {
        private readonly IHistoryCoordinator _historyCoordinator;
        private readonly IObservableCollection<ITaskBarItem> _taskBarItems;

        public ShellViewModel(IHistoryCoordinator historyCoordinator, ITaskBarItem[] taskBarItems)
        {
            _historyCoordinator = historyCoordinator;
            _taskBarItems = new BindableCollection<ITaskBarItem>(taskBarItems);
        }

        public IObservableCollection<ITaskBarItem> TaskBarItems
        {
            get { return _taskBarItems; }
        }

        protected override void OnInitialize()
        {
            _historyCoordinator.Start(
                config =>{
                    config.Host = this;
                    config.HistoryKey = "Page";
                    config.ScreenNotFound = HandleScreenNotFound;
                });

            base.OnInitialize();
        }

        protected override void OnActivate()
        {
            TaskBarItems.First().Enter().Execute();
            base.OnActivate();
        }

        private void HandleScreenNotFound(string historyKey)
        {
            if(string.IsNullOrEmpty(historyKey))
                return;

            var item = TaskBarItems
                .FirstOrDefault(x => x.DisplayName == historyKey);

            if (item != null)
                item.Enter().Execute();
            else Show.MessageBox("Invalid Query String Parameter: " + historyKey).Execute();
        }

        protected override void ExecuteShutdownModel(ISubordinate model, Action completed)
        {
            model.Execute(completed);
        }
    }
}