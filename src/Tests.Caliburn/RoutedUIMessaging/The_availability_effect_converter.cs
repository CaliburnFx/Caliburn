namespace Tests.Caliburn.RoutedUIMessaging
{
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class The_availability_effect_converter : TestBase
    {
        AvailabilityEffectConverter converter;

        protected override void given_the_context_of()
        {
            converter = new AvailabilityEffectConverter();
        }

        [Test]
        public void can_convert_Collapse()
        {
            var result = converter.ConvertFrom("Collapse");

            Assert.That(result, Is.EqualTo(AvailabilityEffect.Collapse));
        }

        [Test]
        public void can_convert_Hide()
        {
            var result = converter.ConvertFrom("Hide");

            Assert.That(result, Is.EqualTo(AvailabilityEffect.Hide));
        }

        [Test]
        public void can_convert_disable()
        {
            var result = converter.ConvertFrom("Disable");

            Assert.That(result, Is.EqualTo(AvailabilityEffect.Disable));
        }

        [Test]
        public void can_convert_from_string()
        {
            Assert.That(converter.CanConvertFrom(typeof(string)));
        }

        [Test]
        public void can_convert_none()
        {
            var result = converter.ConvertFrom("None");

            Assert.That(result, Is.EqualTo(AvailabilityEffect.None));
        }

        [Test]
        public void sends_unknown_strings_to_the_container_for_resolution()
        {
            var container = Mock<IServiceLocator>();
            IoC.Initialize(container);

            var key = "unknown";
            var effect = Stub<IAvailabilityEffect>();

            container.Expect(x => x.GetInstance(null, key))
                .Return(effect);

            converter.ConvertFrom(key);
        }
    }
}