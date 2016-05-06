namespace Tests.Caliburn.PresentationFramework.Screens
{
    using global::Caliburn.PresentationFramework.Screens;
    using Xunit;

    
    public class A_screen_conductor_with_collection_all_screens_active : A_screen
    {
        protected Conductor<IScreen>.Collection.AllActive ScreenConductor;

        protected override Screen CreateScreen()
        {
            return new Conductor<IScreen>.Collection.AllActive(false);
        }

        protected override void given_the_context_of()
        {
            base.given_the_context_of();

            ScreenConductor = (Conductor<IScreen>.Collection.AllActive)Screen;
        }
    }
}