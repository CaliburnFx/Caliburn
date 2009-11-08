using System.Linq;
using Caliburn.Testability;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Tests.Caliburn.Fakes.Model;
using Tests.Caliburn.Fakes.UI;

namespace Tests.Caliburn.Testability
{
    [TestFixture]
    public class When_testing_multibindings
    {
        [Test]
        public void can_detect_bad_multibindings_on_elements()
        {
            var validator = Validator.For<SimpleUIWithMultibinding, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

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
    }
}