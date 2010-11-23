namespace Tests.Caliburn.MVP.Presenters
{
    using global::Caliburn.PresentationFramework.Screens;
    using global::Caliburn.Testability.Extensions;
    using NUnit.Framework;

    [TestFixture]
    public class A_screen : TestBase
    {
        protected Screen _screen;

        protected virtual Screen CreateScreen()
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

            _screen.Activated += (s,e) => { initializedWasRaised = e.WasInitialized; };

            _screen.AssertThatChangeNotificationIsRaisedBy(x => x.IsInitialized)
                .When(() => CallProc(_screen, "Activate"));

            Assert.That(_screen.IsInitialized, Is.True);
            Assert.That(initializedWasRaised);
        }

        

        [Test]
        public void can_only_initialize_once()
        {
            CallProc(_screen, "Activate");

            bool initializedWasNotRaised = true;

            _screen.Activated += (s,e) => { initializedWasNotRaised = !e.WasInitialized; };

            CallProc(_screen, "Activate");

            Assert.That(initializedWasNotRaised);
        }

        [Test]
        public void can_shutdown()
        {
            bool shutdownWasRaised = false;

            _screen.Deactivated += (s,e) => { shutdownWasRaised = e.WasClosed; };

            CallProc(_screen, "Activate");
            CallProc(_screen, "Deactivate", true);

            Assert.That(shutdownWasRaised);
        }

        [Test]
        public void can_be_activated()
        {
            Assert.That(_screen.IsActive, Is.False);

            bool activatedWasRaised = false;

            _screen.Activated += delegate { activatedWasRaised = true; };

            _screen.AssertThatChangeNotificationIsRaisedBy(x => x.IsActive)
                .When(() => CallProc(_screen, "Activate"));

            Assert.That(_screen.IsActive, Is.True);
            Assert.That(activatedWasRaised);
        }

        [Test]
        public void will_only_activate_if_not_already_active()
        {
            CallProc(_screen, "Activate");

            bool activateWasNotRaised = true;

            _screen.Activated += delegate { activateWasNotRaised = false; };

            CallProc(_screen, "Activate");

            Assert.That(activateWasNotRaised);
        }

        [Test]
        public void can_be_deactivated()
        {
            CallProc(_screen, "Activate");

            bool deactivatedWasRaised = false;

            _screen.Deactivated += delegate { deactivatedWasRaised = true; };

            _screen.AssertThatChangeNotificationIsRaisedBy(x => x.IsActive)
                .When(() => CallProc(_screen, "Deactivate", false));

            Assert.That(_screen.IsActive, Is.False);
            Assert.That(deactivatedWasRaised);
        }

        [Test]
        public void will_only_deactivate_if_already_active()
        {
            bool deactivatedWasRaised = true;

            _screen.Deactivated += delegate { deactivatedWasRaised = false; };

            CallProc(_screen, "Deactivate", false);

            Assert.That(deactivatedWasRaised);
        }

        [Test]
        public void has_type_name_as_display_name_by_default()
        {
            Assert.That(_screen.DisplayName, Is.EqualTo(_screen.GetType().FullName));
        }
    }
}