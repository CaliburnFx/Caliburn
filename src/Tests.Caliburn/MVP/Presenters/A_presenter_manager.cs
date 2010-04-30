namespace Tests.Caliburn.MVP.Presenters
{
    using Fakes;
    using global::Caliburn.PresentationFramework.Screens;
    using global::Caliburn.Testability.Extensions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class A_presenter_manager : A_screen
    {
        protected IScreenConductor<IScreen> _screenConductor;
        private IScreen _activeScreen;

        protected override ScreenBase CreateScreen()
        {
            return new ScreenConductor<IScreen>();
        }

        protected override void given_the_context_of()
        {
            base.given_the_context_of();

            _screenConductor = (IScreenConductor<IScreen>)_screen;
            _activeScreen = Mock<IScreen>();
        }

        [Test]
        public void can_shutdown_if_current_presenter_is_null()
        {
            Assert.That(_screenConductor.CanShutdown());
        }

        [Test]
        public void asks_current_presenter_if_can_shutdown()
        {
            _screenConductor.ActiveScreen = _activeScreen;

            _activeScreen.Expect(x => x.CanShutdown())
                .Return(false);

            Assert.That(_screenConductor.CanShutdown(), Is.False);
        }

        [Test]
        public void initializes_current_presenter_during_its_initialization()
        {
            _screenConductor.ActiveScreen = _activeScreen;

            _screenConductor.Initialize();

            _activeScreen.AssertWasCalled(x => x.Initialize());
        }

        [Test]
        public void shuts_down_current_presenter_during_its_shutdown()
        {
            _screenConductor.ActiveScreen = _activeScreen;

            _screenConductor.Shutdown();

            _activeScreen.AssertWasCalled(x => x.Shutdown());
        }

        [Test]
        public void activates_current_presenter_during_its_activation()
        {
            _screenConductor.ActiveScreen = _activeScreen;

            _screenConductor.Activate();

            _activeScreen.AssertWasCalled(x => x.Activate());
        }

        [Test]
        public void deactivates_current_presenter_during_its_deactivation()
        {
            _screenConductor.ActiveScreen = _activeScreen;

            _screenConductor.Activate();
            _screenConductor.Deactivate();

            _activeScreen.AssertWasCalled(x => x.Deactivate());
        }

        [Test]
        public void can_shutdown_current_if_current_is_null()
        {
            bool wasShutdown = false;

            _screenConductor.ShutdownActiveScreen(isSuccess => wasShutdown = isSuccess);

            Assert.That(wasShutdown);
        }

        [Test]
        public void cannot_shutdown_current_if_current_does_not_allow()
        {
            _screenConductor.ActiveScreen = _activeScreen;

            _activeScreen.Expect(x => x.CanShutdown())
                .Return(false);

            bool wasShutdown = false;

            _screenConductor.ShutdownActiveScreen(isSuccess => wasShutdown = isSuccess);

            Assert.That(wasShutdown, Is.False);
        }

        [Test]
        public void can_shutdown_current_if_current_allows()
        {
            _screenConductor.ActiveScreen = _activeScreen;

            _activeScreen.Expect(x => x.CanShutdown())
                .Return(true);

            _screenConductor.AssertThatChangeNotificationIsRaisedBy(x => x.ActiveScreen)
                .When(() =>{
                    bool wasShutdown = false;

                    _screenConductor.ShutdownActiveScreen(isSuccess => wasShutdown = isSuccess);

                    Assert.That(wasShutdown);
                });

            _activeScreen.AssertWasCalled(x => x.Deactivate());
            _activeScreen.AssertWasCalled(x => x.Shutdown());
        }

        [Test]
        public void can_execute_custom_shutdown_on_shutdown_current()
        {
            var presenter = new FakeScreen
            {
                CanShutdownResult = false,
                CustomCanShutdownResult = true
            };

            _screenConductor.ActiveScreen = presenter;

            bool canShutdown = false;

            _screenConductor.ShutdownActiveScreen(isSuccess => canShutdown = isSuccess);

            Assert.That(presenter.CanShutdownWasCalled);
            Assert.That(canShutdown);
        }

        [Test]
        public void can_stop_custom_shutdown_on_shutdown_current()
        {
            var presenter = new FakeScreen
            {
                CanShutdownResult = false,
                CustomCanShutdownResult = false
            };

            _screenConductor.ActiveScreen = presenter;

            bool canShutdown = false;

            _screenConductor.ShutdownActiveScreen(isSuccess => canShutdown = isSuccess);

            Assert.That(presenter.CanShutdownWasCalled);
            Assert.That(canShutdown, Is.False);
        }

        [Test]
        public void can_open_a_presenter()
        {
            _screenConductor.Initialize();
            _screenConductor.Activate();

            _screenConductor.AssertThatChangeNotificationIsRaisedBy(x => x.ActiveScreen)
                .When(() =>{
                    bool wasOpened = false;
                    _screenConductor.OpenScreen(_activeScreen, isSuccess => wasOpened = isSuccess);
                    Assert.That(wasOpened);
                });

            _activeScreen.AssertWasCalled(x => x.Initialize());
            _activeScreen.AssertWasCalled(x => x.Activate());
        }

        [Test]
        public void opens_a_presenter_when_active_and_current_is_set()
        {
            _screenConductor.Initialize();
            _screenConductor.Activate();

            _screenConductor.AssertThatChangeNotificationIsRaisedBy(x => x.ActiveScreen)
                .When(() => _screenConductor.ActiveScreen = _activeScreen);

            _activeScreen.AssertWasCalled(x => x.Initialize());
            _activeScreen.AssertWasCalled(x => x.Activate());
        }

        [Test]
        public void shuts_down_previous_when_opening_a_new_presenter()
        {
            _screenConductor.ActiveScreen = _activeScreen;

            _activeScreen.Expect(x => x.CanShutdown())
                .Return(true);

            bool wasOpened = false;

            _screenConductor.OpenScreen(Mock<IScreen>(), isSuccess => wasOpened = isSuccess);

            _activeScreen.AssertWasCalled(x => x.Deactivate());
            _activeScreen.AssertWasCalled(x => x.Shutdown());

            Assert.That(wasOpened);
        }

        [Test]
        public void can_execute_custom_shutdown_on_previous_when_opening_new()
        {
            var presenter = new FakeScreen
            {
                CanShutdownResult = false,
                CustomCanShutdownResult = true
            };

            _screenConductor.ActiveScreen = presenter;

            bool wasOpened = false;

            _screenConductor.OpenScreen(Mock<IScreen>(), isSuccess => wasOpened = isSuccess);

            Assert.That(wasOpened);

            Assert.That(presenter.CanShutdownWasCalled);
            Assert.That(wasOpened);
        }

        [Test]
        public void can_stop_custom_shutdown_on_previous_when_opening_new()
        {
            var presenter = new FakeScreen
            {
                CanShutdownResult = false,
                CustomCanShutdownResult = false
            };

            _screenConductor.ActiveScreen = presenter;

            bool wasOpened = false;

            _screenConductor.OpenScreen(Mock<IScreen>(), isSuccess => wasOpened = isSuccess);

            Assert.That(presenter.CanShutdownWasCalled);
            Assert.That(wasOpened, Is.False);
        }
    }
}