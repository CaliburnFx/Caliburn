using System.Linq;
using Caliburn.Testability;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Tests.Caliburn.Fakes.Model;
using Tests.Caliburn.Fakes.UI;

namespace Tests.Caliburn.Testability
{
    [TestFixture]
    public class When_testing_styles : TestBase
    {
        [Test]
        public void can_locate_errors_in_setters()
        {
            var validator = Validator.For<UIBoundToCustomerWithStyle, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_locate_errors_in_triggers()
        {
            var validator = Validator.For<UIBoundToCustomerWithStyleAndTriggers, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_locate_errors_in_multi_triggers()
        {
            var validator = Validator.For<UIBoundToCustomerWithStyleAndMultiTriggers, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_locate_errors_in_container_styles()
        {
            var validator = Validator.For<UIWithItemsControlContainerStyle, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_locate_errors_in_group_styles()
        {
            var validator = Validator.For<UIWithItemsControlGroupStyle, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }
    }
}