namespace ContactManager.Presenters
{
    using System;
    using System.Linq;
    using Caliburn.Core.Metadata;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.WPF.ApplicationFramework;
    using Interfaces;
    using Microsoft.Practices.ServiceLocation;
    using Services.Interfaces;

    [Singleton(typeof(IShellPresenter))]
    public class ShellPresenter : Navigator, IShellPresenter
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly ISettings _settings;
        private IPresenter _dialogModel;

        public ShellPresenter(IServiceLocator serviceLocator, ISettings settings)
        {
            _serviceLocator = serviceLocator;
            _settings = settings;
        }

        public IPresenter DialogModel
        {
            get { return _dialogModel; }
            set { _dialogModel = value; NotifyOfPropertyChange("DialogModel"); }
        }

        public void Open<T>() where T : IPresenter
        {
            var presenter = _serviceLocator.GetInstance<T>();
            this.Open(presenter);
        }

        public void GoHome()
        {
            Open<IHomePresenter>();
        }

        public void ShowDialog<T>(T presenter)
            where T : IPresenter, ILifecycleNotifier
        {
            presenter.WasShutdown +=
                delegate { DialogModel = null; };

            DialogModel = presenter;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _settings.Load();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Open<IHomePresenter>();
        }

        protected override void ExecuteShutdownModel(ISubordinate model, Action completed)
        {
            var dialogPresenter = _serviceLocator.GetInstance<IQuestionPresenter>();

            var composite = model as ISubordinateComposite;
            if(composite != null)
            {
                dialogPresenter.Setup(composite.GetChildren().Cast<Question>(), completed);
                ShowDialog(dialogPresenter);
            }
            else
            {
                var question = (Question)model;
                dialogPresenter.Setup(new[] {question}, completed);
                ShowDialog(dialogPresenter);
            }
        }
    }
}