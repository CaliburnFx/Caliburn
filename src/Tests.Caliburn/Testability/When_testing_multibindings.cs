namespace Tests.Caliburn.Testability
{
    using System.Linq;
    using Fakes.Model;
    using Fakes.UI;
    using global::Caliburn.Testability;
    using NUnit.Framework;

    [TestFixture]
    public class When_testing_multibindings
    {
        [Test]
        public void can_detect_bad_multibindings_in_styles()
        {
            var validator = Validator.For<UIWithMultibindingToStyle, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_detect_bad_multibindings_in_triggers()
        {
            var validator = Validator.For<UIWithMultibindingStyleAndTriggers, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_detect_bad_multibindings_on_elements()
        {
            var validator = Validator.For<SimpleUIWithMultibinding, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }
    }
}