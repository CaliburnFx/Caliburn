namespace Tests.Caliburn.MVP.Presenters
{
    using System;
    using Fakes;
    using global::Caliburn.PresentationFramework.Screens;
    using NUnit.Framework;

    [TestFixture]
    public class A_presenter_manager : A_screen
    {
        protected Conductor<IScreen> _screenConductor;
        private FakeScreen _activeScreen;

        protected override Screen CreateScreen()
        {
            return new Conductor<IScreen>();
        }

        protected override void given_the_context_of()
        {
            base.given_the_context_of();

            _activeScreen = new FakeScreen();
            _screenConductor = (Conductor<IScreen>)_screen;
        }

        [Test]
        public void can_shutdown_if_current_presenter_is_null()
        {
            _screenConductor.CanClose(Assert.That);
        }

        [Test]
        public void asks_current_presenter_if_can_shutdown()
        {
            _screenConductor.ActiveItem = _activeScreen;
            _screenConductor.CanClose(result => { });

            Assert.IsTrue(_activeScreen.CanCloseWasCalled);
        }

        [Test]
        public void initializes_current_presenter_during_its_initialization()
        {
            _screenConductor.ActiveItem = _activeScreen;

            CallProc(_screenConductor, "Activate");

            Assert.IsTrue(_activeScreen.IsInitialized);
        }

        [Test]
        public void shuts_down_current_presenter_during_its_shutdown()
        {
            var wasClosed = false;

            _activeScreen.Deactivated += (s, e) => wasClosed = e.WasClosed;
            _screenConductor.ActiveItem = _activeScreen;
            _activeScreen.CanCloseResult = true;
            CallProc(_screenConductor, "Activate");

            CallProc(_screenConductor, "Deactivate", true);

            Assert.IsTrue(wasClosed);
        }

        [Test]
        public void activates_current_presenter_during_its_activation()
        {
            _screenConductor.ActiveItem = _activeScreen;

            CallProc(_screenConductor, "Activate");

            Assert.IsTrue(_activeScreen.IsActive);
        }

        [Test]
        public void deactivates_current_presenter_during_its_deactivation()
        {
            _screenConductor.ActiveItem = _activeScreen;

            CallProc(_screenConductor, "Activate");
            CallProc(_screenConductor, "Deactivate", false);

            Assert.IsFalse(_activeScreen.IsActive);
        }

        [Test]
        public void cannot_shutdown_current_if_current_does_not_allow()
        {
            _activeScreen.CanCloseResult = false;
            _screenConductor.ActiveItem = _activeScreen;
            CallProc(_screenConductor, "Activate");

            _screenConductor.CloseItem(_activeScreen);

            Assert.That(_activeScreen.IsActive);
        }

        [Test]
        public void can_shutdown_current_if_current_allows()
        {
            bool wasClosed = false;

            CallProc(_screenConductor, "Activate");
            _screenConductor.ActiveItem = _activeScreen;
            _activeScreen.Deactivated += (s, e) => wasClosed = e.WasClosed;
            _activeScreen.CanCloseResult = true;

            _screenConductor.CloseItem(_activeScreen);

            Assert.That(wasClosed);
        }

        [Test]
        public void can_open_a_presenter()
        {
            bool wasOpened = false;

            CallProc(_screenConductor, "Activate");
            _activeScreen.Activated += (s, e) => wasOpened = e.WasInitialized;

            _screenConductor.ActivateItem(_activeScreen);

            Assert.That(wasOpened);
        }

        [Test]
        public void opens_a_presenter_when_active_and_current_is_set()
        {
            bool wasOpened = false;

            CallProc(_screenConductor, "Activate");
            _activeScreen.Activated += (s, e) => wasOpened = e.WasInitialized;

            _screenConductor.ActiveItem = _activeScreen;

            Assert.That(wasOpened);
        }

        [Test]
        public void shuts_down_previous_when_opening_a_new_presenter()
        {
            var wasClosed = false;

            CallProc(_screenConductor, "Activate");
            _activeScreen.Deactivated += (s, e) => wasClosed = e.WasClosed;
            _screenConductor.ActiveItem = _activeScreen;
            _activeScreen.CanCloseResult = true;

            _screenConductor.ActivateItem(Mock<IScreen>());

            Assert.That(wasClosed);
        }
    }
}