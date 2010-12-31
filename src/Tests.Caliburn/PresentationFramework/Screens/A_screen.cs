namespace Tests.Caliburn.PresentationFramework.Screens
{
    using global::Caliburn.PresentationFramework.Screens;
    using global::Caliburn.Testability.Extensions;
    using NUnit.Framework;

    [TestFixture]
    public class A_screen : TestBase
    {
        protected Screen Screen;

        protected virtual Screen CreateScreen()
        {
            return new Screen();
        }

        protected override void given_the_context_of()
        {
            Screen = CreateScreen();
        }

        [Test]
        public void can_be_initialized()
        {
            Assert.That(Screen.IsInitialized, Is.False);

            bool initializedWasRaised = false;

            Screen.Activated += (s,e) => { initializedWasRaised = e.WasInitialized; };

            Screen.AssertThatChangeNotificationIsRaisedBy(x => x.IsInitialized)
                .When(() => CallProc(Screen, "Activate"));

            Assert.That(Screen.IsInitialized, Is.True);
            Assert.That(initializedWasRaised);
        }

        [Test]
        public void can_only_initialize_once()
        {
            CallProc(Screen, "Activate");

            bool initializedWasNotRaised = true;

            Screen.Activated += (s,e) => { initializedWasNotRaised = !e.WasInitialized; };

            CallProc(Screen, "Activate");

            Assert.That(initializedWasNotRaised);
        }

        [Test]
        public void can_shutdown()
        {
            bool shutdownWasRaised = false;

            Screen.Deactivated += (s,e) => { shutdownWasRaised = e.WasClosed; };

            CallProc(Screen, "Activate");
            CallProc(Screen, "Deactivate", true);

            Assert.That(shutdownWasRaised);
        }

        [Test]
        public void can_be_activated()
        {
            Assert.That(Screen.IsActive, Is.False);

            bool activatedWasRaised = false;

            Screen.Activated += delegate { activatedWasRaised = true; };

            Screen.AssertThatChangeNotificationIsRaisedBy(x => x.IsActive)
                .When(() => CallProc(Screen, "Activate"));

            Assert.That(Screen.IsActive, Is.True);
            Assert.That(activatedWasRaised);
        }

        [Test]
        public void will_only_activate_if_not_already_active()
        {
            CallProc(Screen, "Activate");

            bool activateWasNotRaised = true;

            Screen.Activated += delegate { activateWasNotRaised = false; };

            CallProc(Screen, "Activate");

            Assert.That(activateWasNotRaised);
        }

        [Test]
        public void can_be_deactivated()
        {
            CallProc(Screen, "Activate");

            bool deactivatedWasRaised = false;

            Screen.Deactivated += delegate { deactivatedWasRaised = true; };

            Screen.AssertThatChangeNotificationIsRaisedBy(x => x.IsActive)
                .When(() => CallProc(Screen, "Deactivate", false));

            Assert.That(Screen.IsActive, Is.False);
            Assert.That(deactivatedWasRaised);
        }

        [Test]
        public void will_only_deactivate_if_already_active()
        {
            bool deactivatedWasRaised = true;

            Screen.Deactivated += delegate { deactivatedWasRaised = false; };

            CallProc(Screen, "Deactivate", false);

            Assert.That(deactivatedWasRaised);
        }

        [Test]
        public void has_type_name_as_display_name_by_default()
        {
            Assert.That(Screen.DisplayName, Is.EqualTo(Screen.GetType().FullName));
        }
    }
}