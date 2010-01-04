namespace Tests.Caliburn.MVP.Presenters
{
    using global::Caliburn.PresentationFramework.Screens;
    using NUnit.Framework;

    [TestFixture]
    public class A_screen_conductor_with_collection_all_screens_active : A_screen
    {
        protected ScreenConductor<IScreen>.WithCollection.AllScreensActive _screenConductor;

        protected override ScreenBase CreateScreen()
        {
            return new ScreenConductor<IScreen>.WithCollection.AllScreensActive();
        }

        protected override void given_the_context_of()
        {
            base.given_the_context_of();

            _screenConductor = (ScreenConductor<IScreen>.WithCollection.AllScreensActive)_screen;
        }
    }
}