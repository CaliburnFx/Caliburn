namespace ContactManager.Presenters
{
    using System;
    using Caliburn.Core.IoC;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Interfaces;
    using Services.Interfaces;

    [PerRequest(typeof(ISettingsPresenter))]
    public class SettingsPresenter : Presenter, ISettingsPresenter
    {
        private readonly IShellPresenter _shellPresenter;
        private readonly ISettings _settings;

        private TimeSpan _earliestAppointment;
        private TimeSpan _latestAppointment;

        public SettingsPresenter(IShellPresenter shellPresenter, ISettings settings)
        {
            _shellPresenter = shellPresenter;
            _settings = settings;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            DisplayName = "Settings";
            EarliestAppointment = _settings.EarliestAppointment;
            LatestAppointment = _settings.LatestAppointment;
        }

        public TimeSpan EarliestAppointment
        {
            get { return _earliestAppointment; }
            set
            {
                _earliestAppointment = value;
                NotifyOfPropertyChange(() => EarliestAppointment);
            }
        }

        public TimeSpan LatestAppointment
        {
            get { return _latestAppointment; }
            set
            {
                _latestAppointment = value;
                NotifyOfPropertyChange(() => LatestAppointment);
            }
        }

        public void Save()
        {
            _settings.EarliestAppointment = EarliestAppointment;
            _settings.LatestAppointment = LatestAppointment;

            _settings.Save();

            _shellPresenter.Open<IHomePresenter>();
        }

        public void Cancel()
        {
            _shellPresenter.Open<IHomePresenter>();
        }
    }
}