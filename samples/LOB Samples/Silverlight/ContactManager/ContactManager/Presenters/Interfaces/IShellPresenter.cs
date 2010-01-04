namespace ContactManager.Presenters.Interfaces
{
    using Caliburn.PresentationFramework.Screens;

    public interface IShellPresenter : IScreenConductor<IScreen>
    {
        void Open<T>() where T : IScreen;
        void ShowDialog<T>(T presenter) where T : IScreenEx;
    }
}