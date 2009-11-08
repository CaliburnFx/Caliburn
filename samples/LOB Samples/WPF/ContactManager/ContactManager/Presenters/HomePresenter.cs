namespace ContactManager.Presenters
{
    using Caliburn.Core.Metadata;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Interfaces;

    [PerRequest(typeof(IHomePresenter))]
    public class HomePresenter : Presenter, IHomePresenter
    {
        private readonly IShellPresenter _shellPresenter;

        public HomePresenter(IShellPresenter shellPresenter)
        {
            _shellPresenter = shellPresenter;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            DisplayName = "Home";
        }

        public void GotoCustomerList()
        {
            _shellPresenter.Open<IContactListPresenter>();
        }

        public void GotoAppointments()
        {
            _shellPresenter.Open<ISchedulePresenter>();
        }

        public void GotoSettings()
        {
            _shellPresenter.Open<ISettingsPresenter>();
        }
    }
}