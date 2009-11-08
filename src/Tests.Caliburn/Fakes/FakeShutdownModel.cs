namespace Tests.Caliburn.Fakes
{
    using global::Caliburn.PresentationFramework.ApplicationModel;

    public class FakeShutdownModel : ISubordinate
    {
        public IPresenter Master { get; set; }
    }
}