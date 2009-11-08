namespace ContactManager.Presenters.Interfaces
{
    using Caliburn.PresentationFramework.ApplicationModel;

    public interface IShellPresenter : INavigator
    {
        void Open<T>() where T : IPresenter;
        void ShowDialog<T>(T presenter) where T : IPresenter, ILifecycleNotifier;
    }
}