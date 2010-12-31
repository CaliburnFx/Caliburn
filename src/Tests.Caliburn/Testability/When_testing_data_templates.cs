namespace Tests.Caliburn.Testability
{
    using System.Linq;
    using Fakes.Model;
    using Fakes.UI;
    using global::Caliburn.Testability;
    using NUnit.Framework;

    [TestFixture]
    public class When_testing_data_templates : TestBase
    {
        [Test]
        public void can_locate_errors_in_content_control_templates()
        {
            var validator = Validator.For<UIBoundToCustomerWithContentControl, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_locate_errors_in_group_styles()
        {
            var validator = Validator.For<UIWithItemsControlGroupStyleTemplate, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_locate_errors_in_items_control_templates()
        {
            var validator = Validator.For<UIBoundToCustomerWithItemsControl, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(3));
        }

        [Test]
        public void can_locate_errors_in_multi_triggers()
        {
            var validator = Validator.For<UIBoundToCustomerWithContentControlAndMultiTriggers, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_locate_errors_in_triggers()
        {
            var validator = Validator.For<UIBoundToCustomerWithContentControlAndTriggers, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }
    }
}