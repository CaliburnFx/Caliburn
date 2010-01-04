namespace Tests.Caliburn.MVP.Presenters
{
    using global::Caliburn.PresentationFramework.Screens;
    using global::Caliburn.Testability.Extensions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class A_screen : TestBase
    {
        protected ScreenBase _screen;

        protected virtual ScreenBase CreateScreen()
        {
            return new Screen();
        }

        protected override void given_the_context_of()
        {
            _screen = CreateScreen();
        }

        [Test]
        public void can_be_initialized()
        {
            Assert.That(_screen.IsInitialized, Is.False);

            bool initializedWasRaised = false;

            _screen.Initialized += delegate { initializedWasRaised = true; };

            _screen.AssertThatChangeNotificationIsRaisedBy(x => x.IsInitialized)
                .When(_screen.Initialize);

            Assert.That(_screen.IsInitialized, Is.True);
            Assert.That(initializedWasRaised);
        }

        [Test]
        public void can_only_initialize_once()
        {
            _screen.Initialize();

            bool initializedWasNotRaised = true;

            _screen.Initialized += delegate { initializedWasNotRaised = false; };

            _screen.Initialize();

            Assert.That(initializedWasNotRaised);
        }

        [Test]
        public void can_shutdown()
        {
            bool shutdownWasRaised = false;

            _screen.WasShutdown += delegate { shutdownWasRaised = true; };

            _screen.Shutdown();

            Assert.That(shutdownWasRaised);
        }

        [Test]
        public void can_be_activated()
        {
            Assert.That(_screen.IsActive, Is.False);

            bool activatedWasRaised = false;

            _screen.Activated += delegate { activatedWasRaised = true; };

            _screen.AssertThatChangeNotificationIsRaisedBy(x => x.IsActive)
                .When(_screen.Activate);

            Assert.That(_screen.IsActive, Is.True);
            Assert.That(activatedWasRaised);
        }

        [Test]
        public void will_only_activate_if_not_already_active()
        {
            _screen.Activate();

            bool activateWasNotRaised = true;

            _screen.Activated += delegate { activateWasNotRaised = false; };

            _screen.Activate();

            Assert.That(activateWasNotRaised);
        }

        [Test]
        public void can_be_deactivated()
        {
            _screen.Activate();

            bool deactivatedWasRaised = false;

            _screen.Deactivated += delegate { deactivatedWasRaised = true; };

            _screen.AssertThatChangeNotificationIsRaisedBy(x => x.IsActive)
                .When(_screen.Deactivate);

            Assert.That(_screen.IsActive, Is.False);
            Assert.That(deactivatedWasRaised);
        }

        [Test]
        public void will_only_deactivate_if_already_active()
        {
            bool deactivatedWasRaised = true;

            _screen.Deactivated += delegate { deactivatedWasRaised = false; };

            _screen.Deactivate();

            Assert.That(deactivatedWasRaised);
        }

        [Test]
        public void has_type_name_as_display_name_by_default()
        {
            Assert.That(_screen.DisplayName, Is.EqualTo(_screen.GetType().Name));
        }
    }
}