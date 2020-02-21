using Shouldly;

namespace Tests.Caliburn.RoutedUIMessaging
{
    using System.Windows;
    using Fakes.UI;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using Xunit;


    public class The_availability_effect_of : TestBase
    {
        FakeElement element;

        protected override void given_the_context_of()
        {
            element = new FakeElement();
        }

        [StaFact]
        public void disable_can_disable_an_element_if_not_available()
        {
            element.IsEnabled = true;

            AvailabilityEffect.Disable.ApplyTo(element, false);

            element.IsEnabled.ShouldBeFalse();
        }

        [StaFact]
        public void disable_can_enable_an_element_if_available()
        {
            element.IsEnabled = false;

            AvailabilityEffect.Disable.ApplyTo(element, true);

            element.IsEnabled.ShouldBeTrue();
        }

        [Fact]
        public void hide_can_hide_an_element_if_not_available()
        {
            element.Visibility = Visibility.Visible;

            AvailabilityEffect.Hide.ApplyTo(element, false);

            element.Visibility.ShouldBe(Visibility.Hidden);
        }

        [Fact]
        public void hide_can_make_visible_an_element_if_available()
        {
            element.Visibility = Visibility.Hidden;

            AvailabilityEffect.Hide.ApplyTo(element, true);

            element.Visibility.ShouldBe(Visibility.Visible);
        }

        [Fact]
        public void collapse_can_collapse_an_element_if_not_available()
        {
            element.Visibility = Visibility.Visible;

            AvailabilityEffect.Collapse.ApplyTo(element, false);

            element.Visibility.ShouldBe(Visibility.Collapsed);
        }

        [Fact]
        public void collapse_can_collapse_an_element_if_available()
        {
            element.Visibility = Visibility.Collapsed;

            AvailabilityEffect.Hide.ApplyTo(element, true);

            element.Visibility.ShouldBe(Visibility.Visible);
        }
    }
}
