using Shouldly;

namespace Tests.Caliburn.RoutedUIMessaging
{
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using Xunit;
    using Rhino.Mocks;

    
    public class The_availability_effect_converter : TestBase
    {
        AvailabilityEffectConverter converter;

        protected override void given_the_context_of()
        {
            converter = new AvailabilityEffectConverter();
        }

        [Fact]
        public void can_convert_Collapse()
        {
            var result = converter.ConvertFrom("Collapse");

            result.ShouldBe(AvailabilityEffect.Collapse);
        }

        [Fact]
        public void can_convert_Hide()
        {
            var result = converter.ConvertFrom("Hide");

            result.ShouldBe(AvailabilityEffect.Hide);
        }

        [Fact]
        public void can_convert_disable()
        {
            var result = converter.ConvertFrom("Disable");

            result.ShouldBe(AvailabilityEffect.Disable);
        }

        [Fact]
        public void can_convert_from_string()
        {
            converter.CanConvertFrom(typeof(string)).ShouldBeTrue();
        }

        [Fact]
        public void can_convert_none()
        {
            var result = converter.ConvertFrom("None");

            result.ShouldBe(AvailabilityEffect.None);
        }

        [Fact]
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