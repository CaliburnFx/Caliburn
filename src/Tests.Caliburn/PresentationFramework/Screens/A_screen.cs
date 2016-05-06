using Shouldly;

namespace Tests.Caliburn.PresentationFramework.Screens
{
    using global::Caliburn.PresentationFramework.Screens;
    using global::Caliburn.Testability.Extensions;
    using Xunit;

    
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

        [Fact]
        public void can_be_initialized()
        {
            Screen.IsInitialized.ShouldBeFalse();

            bool initializedWasRaised = false;

            Screen.Activated += (s,e) => { initializedWasRaised = e.WasInitialized; };

            Screen.AssertThatChangeNotificationIsRaisedBy(x => x.IsInitialized)
                .When(() => CallProc(Screen, "Activate"));

            Screen.IsInitialized.ShouldBeTrue();
            initializedWasRaised.ShouldBeTrue();
        }

        [Fact]
        public void can_only_initialize_once()
        {
            CallProc(Screen, "Activate");

            bool initializedWasNotRaised = true;

            Screen.Activated += (s,e) => { initializedWasNotRaised = !e.WasInitialized; };

            CallProc(Screen, "Activate");

            initializedWasNotRaised.ShouldBeTrue();
        }

        [Fact]
        public void can_shutdown()
        {
            bool shutdownWasRaised = false;

            Screen.Deactivated += (s,e) => { shutdownWasRaised = e.WasClosed; };

            CallProc(Screen, "Activate");
            CallProc(Screen, "Deactivate", true);

            shutdownWasRaised.ShouldBeTrue();
        }

        [Fact]
        public void can_be_activated()
        {
            Screen.IsInitialized.ShouldBeFalse();

            bool activatedWasRaised = false;

            Screen.Activated += delegate { activatedWasRaised = true; };

            Screen.AssertThatChangeNotificationIsRaisedBy(x => x.IsActive)
                .When(() => CallProc(Screen, "Activate"));

            Screen.IsActive.ShouldBeTrue();
            activatedWasRaised.ShouldBeTrue();
        }

        [Fact]
        public void will_only_activate_if_not_already_active()
        {
            CallProc(Screen, "Activate");

            bool activateWasNotRaised = true;

            Screen.Activated += delegate { activateWasNotRaised = false; };

            CallProc(Screen, "Activate");

            activateWasNotRaised.ShouldBeTrue();
        }

        [Fact]
        public void can_be_deactivated()
        {
            CallProc(Screen, "Activate");

            bool deactivatedWasRaised = false;

            Screen.Deactivated += delegate { deactivatedWasRaised = true; };

            Screen.AssertThatChangeNotificationIsRaisedBy(x => x.IsActive)
                .When(() => CallProc(Screen, "Deactivate", false));

            Screen.IsActive.ShouldBeFalse();
            deactivatedWasRaised.ShouldBeTrue();
        }

        [Fact]
        public void will_only_deactivate_if_already_active()
        {
            bool deactivatedWasRaised = true;

            Screen.Deactivated += delegate { deactivatedWasRaised = false; };

            CallProc(Screen, "Deactivate", false);

            deactivatedWasRaised.ShouldBeTrue();
        }

        [Fact]
        public void has_type_name_as_display_name_by_default()
        {
            Screen.DisplayName.ShouldBe(Screen.GetType().FullName);
        }
    }
}