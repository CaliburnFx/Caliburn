namespace Tests.Caliburn.Fakes
{
    using global::Caliburn.PresentationFramework.ApplicationModel;
    using global::Caliburn.PresentationFramework.Screens;

    public class FakeShutdownModel : ISubordinate
    {
        public IScreen Master { get; set; }
    }
}