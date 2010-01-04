namespace ContactManager.Presenters
{
    using Caliburn.Core.IoC;
    using Caliburn.PresentationFramework.Screens;
    using Caliburn.WPF.ApplicationFramework;
    using Interfaces;

    [Singleton(typeof(ILoadScreen))]
    public class LoadScreenPresenter : Screen, ILoadScreen
    {
        private readonly IShellPresenter _shellPresenter;

        public LoadScreenPresenter(IShellPresenter shellPresenter)
        {
            _shellPresenter = shellPresenter;
        }

        public void StartLoading()
        {
            _shellPresenter.ShowDialog(this);
        }

        public void StopLoading()
        {
            Shutdown();
        }
    }
}