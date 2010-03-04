namespace Tests.Caliburn.RoutedUIMessaging
{
    using System.Windows;
    using Fakes.UI;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class The_availability_effect_of : TestBase
    {
        private FakeElement _element;

        protected override void given_the_context_of()
        {
            _element = new FakeElement();
        }

        [Test]
        public void disable_can_disable_an_element_if_not_available()
        {
            _element.IsEnabled = true;

            AvailabilityEffect.Disable.ApplyTo(_element, false);

            Assert.That(_element.IsEnabled, Is.False);
        }

        [Test]
        public void disable_can_enable_an_element_if_available()
        {
            _element.IsEnabled = false;

            AvailabilityEffect.Disable.ApplyTo(_element, true);

            Assert.That(_element.IsEnabled);
        }

        [Test]
        public void hide_can_hide_an_element_if_not_available()
        {
            _element.Visibility = Visibility.Visible;

            AvailabilityEffect.Hide.ApplyTo(_element, false);

            Assert.That(_element.Visibility, Is.EqualTo(Visibility.Hidden));
        }

        [Test]
        public void hide_can_make_visible_an_element_if_available()
        {
            _element.Visibility = Visibility.Hidden;

            AvailabilityEffect.Hide.ApplyTo(_element, true);

            Assert.That(_element.Visibility, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void collapse_can_collapse_an_element_if_not_available()
        {
            _element.Visibility = Visibility.Visible;

            AvailabilityEffect.Collapse.ApplyTo(_element, false);

            Assert.That(_element.Visibility, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void collapse_can_collapse_an_element_if_available()
        {
            _element.Visibility = Visibility.Collapsed;

            AvailabilityEffect.Hide.ApplyTo(_element, true);

            Assert.That(_element.Visibility, Is.EqualTo(Visibility.Visible));
        }
    }
}