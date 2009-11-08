namespace ContactManager.Presenters.Interfaces
{
    using Caliburn.PresentationFramework.ApplicationModel;
    using Model;

    public interface IContactDetailsPresenter : IPresenter, ISupportCustomShutdown
    {
        Contact Contact { get; }
        void Setup(IPresenterManager owner, Contact contact);
    }
}