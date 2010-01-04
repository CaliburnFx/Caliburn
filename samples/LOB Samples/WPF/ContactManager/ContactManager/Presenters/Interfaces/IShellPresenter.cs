namespace ContactManager.Presenters.Interfaces
{
    using Caliburn.PresentationFramework.Screens;

    public interface IShellPresenter : INavigator<IScreen>
    {
        void Open<T>() where T : IScreen;
        void ShowDialog<T>(T screen) where T : IScreenEx;
    }
}