namespace Tests.Caliburn.PresentationFramework.Screens
{
    using global::Caliburn.PresentationFramework.Screens;
    using Fakes;
    using NUnit.Framework;

    [TestFixture]
    public class A_screen_conductor_with_collection_one_screen_active : A_screen
    {
        protected Conductor<IScreen>.Collection.OneActive ScreenConductor;
        private FakeScreen activeScreen;

        protected override Screen CreateScreen()
        {
            return new Conductor<IScreen>.Collection.OneActive();
        }

        protected override void given_the_context_of()
        {
            base.given_the_context_of();

            ScreenConductor = (Conductor<IScreen>.Collection.OneActive)Screen;
            activeScreen = new FakeScreen();
        }

        [Test]
        public void can_shutdown_if_current_item_is_null()
        {
            ScreenConductor.CanClose(Assert.That);
        }

        [Test]
        public void asks_current_item_if_can_shutdown()
        {
            ScreenConductor.ActiveItem = activeScreen;
            ScreenConductor.CanClose(result => { });

            Assert.IsTrue(activeScreen.CanCloseWasCalled);
        }

        [Test]
        public void initializes_current_item_during_its_initialization()
        {
            ScreenConductor.ActiveItem = activeScreen;

            CallProc(ScreenConductor, "Activate");

            Assert.IsTrue(activeScreen.IsInitialized);
        }

        [Test]
        public void shuts_down_current_item_during_its_shutdown()
        {
            var wasClosed = false;

            activeScreen.CanCloseResult = true;
            activeScreen.Deactivated += (s, e) => wasClosed = e.WasClosed;
            ScreenConductor.ActiveItem = activeScreen;
            CallProc(ScreenConductor, "Activate");

            CallProc(ScreenConductor, "Deactivate", true);

            Assert.IsTrue(wasClosed);
        }

        [Test]
        public void activates_current_item_during_its_activation()
        {
            ScreenConductor.ActiveItem = activeScreen;

            CallProc(ScreenConductor, "Activate");

            Assert.IsTrue(activeScreen.IsActive);
        }

        [Test]
        public void deactivates_current_item_during_its_deactivation()
        {
            ScreenConductor.ActiveItem = activeScreen;

            CallProc(ScreenConductor, "Activate");
            CallProc(ScreenConductor, "Deactivate", false);

            Assert.IsFalse(activeScreen.IsActive);
        }

        [Test]
        public void cannot_shutdown_current_if_current_does_not_allow()
        {
            activeScreen.CanCloseResult = false;
            ScreenConductor.ActiveItem = activeScreen;
            CallProc(ScreenConductor, "Activate");

            ScreenConductor.CloseItem(activeScreen);

            Assert.That(activeScreen.IsActive);
        }

        [Test]
        public void can_shutdown_current_if_current_allows()
        {
            bool wasClosed = false;
            activeScreen.CanCloseResult = true;

            CallProc(ScreenConductor, "Activate");
            ScreenConductor.ActiveItem = activeScreen;
            activeScreen.Deactivated += (s, e) =>{
                wasClosed = e.WasClosed;
            };

            ScreenConductor.CloseItem(activeScreen);

            Assert.That(wasClosed);
        }

        [Test]
        public void can_open_an_item()
        {
            bool wasOpened = false;

            CallProc(ScreenConductor, "Activate");
            activeScreen.Activated += (s, e) => wasOpened = e.WasInitialized;

            ScreenConductor.ActivateItem(activeScreen);

            Assert.That(wasOpened);
        }

        [Test]
        public void opens_an_item_when_active_and_current_is_set()
        {
            bool wasOpened = false;

            CallProc(ScreenConductor, "Activate");
            activeScreen.Activated += (s, e) => wasOpened = e.WasInitialized;

            ScreenConductor.ActiveItem = activeScreen;

            Assert.That(wasOpened);
        }
    }
}