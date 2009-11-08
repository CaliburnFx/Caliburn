namespace Tests.Caliburn.MVP.Presenters
{
    using global::Caliburn.PresentationFramework.ApplicationModel;
    using global::Caliburn.Testability.Extensions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class A_presenter : TestBase
    {
        protected PresenterBase _presenter;

        protected virtual PresenterBase CreatePresenter()
        {
            return new Presenter();
        }

        protected override void given_the_context_of()
        {
            _presenter = CreatePresenter();
        }

        [Test]
        public void can_be_initialized()
        {
            Assert.That(_presenter.IsInitialized, Is.False);

            bool initializedWasRaised = false;

            _presenter.Initialized += delegate { initializedWasRaised = true; };

            _presenter.AssertThatChangeNotificationIsRaisedBy(x => x.IsInitialized)
                .When(_presenter.Initialize);

            Assert.That(_presenter.IsInitialized, Is.True);
            Assert.That(initializedWasRaised);
        }

        [Test]
        public void can_only_initialize_once()
        {
            _presenter.Initialize();

            bool initializedWasNotRaised = true;

            _presenter.Initialized += delegate { initializedWasNotRaised = false; };

            _presenter.Initialize();

            Assert.That(initializedWasNotRaised);
        }

        [Test]
        public void can_shutdown()
        {
            bool shutdownWasRaised = false;

            _presenter.WasShutdown += delegate { shutdownWasRaised = true; };

            _presenter.Shutdown();

            Assert.That(shutdownWasRaised);
        }

        [Test]
        public void can_be_activated()
        {
            Assert.That(_presenter.IsActive, Is.False);

            bool activatedWasRaised = false;

            _presenter.Activated += delegate { activatedWasRaised = true; };

            _presenter.AssertThatChangeNotificationIsRaisedBy(x => x.IsActive)
                .When(_presenter.Activate);

            Assert.That(_presenter.IsActive, Is.True);
            Assert.That(activatedWasRaised);
        }

        [Test]
        public void will_only_activate_if_not_already_active()
        {
            _presenter.Activate();

            bool activateWasNotRaised = true;

            _presenter.Activated += delegate { activateWasNotRaised = false; };

            _presenter.Activate();

            Assert.That(activateWasNotRaised);
        }

        [Test]
        public void can_be_deactivated()
        {
            _presenter.Activate();

            bool deactivatedWasRaised = false;

            _presenter.Deactivated += delegate { deactivatedWasRaised = true; };

            _presenter.AssertThatChangeNotificationIsRaisedBy(x => x.IsActive)
                .When(_presenter.Deactivate);

            Assert.That(_presenter.IsActive, Is.False);
            Assert.That(deactivatedWasRaised);
        }

        [Test]
        public void will_only_deactivate_if_already_active()
        {
            bool deactivatedWasRaised = true;

            _presenter.Deactivated += delegate { deactivatedWasRaised = false; };

            _presenter.Deactivate();

            Assert.That(deactivatedWasRaised);
        }

        [Test]
        public void has_type_name_as_display_name_by_default()
        {
            Assert.That(_presenter.DisplayName, Is.EqualTo(_presenter.GetType().Name));
        }
    }
}