namespace Tests.Caliburn.MVP.Presenters
{
    using global::Caliburn.PresentationFramework.Screens;
    using NUnit.Framework;

    [TestFixture]
    public class A_screen_conductor_with_collection_all_screens_active : A_screen
    {
        protected Conductor<IScreen>.Collection.AllActive _screenConductor;

        protected override Screen CreateScreen()
        {
            return new Conductor<IScreen>.Collection.AllActive(false);
        }

        protected override void given_the_context_of()
        {
            base.given_the_context_of();

            _screenConductor = (Conductor<IScreen>.Collection.AllActive)_screen;
        }
    }
}