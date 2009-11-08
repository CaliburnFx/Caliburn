namespace Tests.Caliburn.RoutedUIMessaging
{
    using global::Caliburn.PresentationFramework;
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class The_availability_effect_converter : TestBase
    {
        private AvailabilityEffectConverter _converter;

        protected override void given_the_context_of()
        {
            _converter = new AvailabilityEffectConverter();
        }

        [Test]
        public void can_convert_from_string()
        {
            Assert.That(_converter.CanConvertFrom(typeof(string)));
        }

        [Test]
        public void can_convert_none()
        {
            var result = _converter.ConvertFrom("None");

            Assert.That(result, Is.EqualTo(AvailabilityEffect.None));
        }

        [Test]
        public void can_convert_disable()
        {
            var result = _converter.ConvertFrom("Disable");

            Assert.That(result, Is.EqualTo(AvailabilityEffect.Disable));
        }

        [Test]
        public void can_convert_Collapse()
        {
            var result = _converter.ConvertFrom("Collapse");

            Assert.That(result, Is.EqualTo(AvailabilityEffect.Collapse));
        }

        [Test]
        public void can_convert_Hide()
        {
            var result = _converter.ConvertFrom("Hide");

            Assert.That(result, Is.EqualTo(AvailabilityEffect.Hide));
        }

        [Test]
        public void sends_unknown_strings_to_the_container_for_resolution()
        {
            var container = Mock<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => container);

            string key = "unknown";
            var effect = Stub<IAvailabilityEffect>();

            container.Expect(x => x.GetInstance(null, key))
                .Return(effect);

            _converter.ConvertFrom(key);
        }
    }
}