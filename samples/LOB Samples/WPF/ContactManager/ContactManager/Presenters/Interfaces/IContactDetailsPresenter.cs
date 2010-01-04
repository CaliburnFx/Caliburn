namespace ContactManager.Presenters.Interfaces
{
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Model;

    public interface IContactDetailsPresenter : IScreen, ISupportCustomShutdown
    {
        Contact Contact { get; }
        void Setup(Contact contact);
    }
}