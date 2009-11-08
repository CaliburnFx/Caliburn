using System.Linq;
using Caliburn.Testability;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Tests.Caliburn.Fakes.Model;
using Tests.Caliburn.Fakes.UI;

namespace Tests.Caliburn.Testability
{
    [TestFixture]
    public class When_testing_nested_bindings : TestBase
    {
        [Test]
        public void can_detect_bad_nested_property_paths()
        {
            var validator = Validator.For<UIBoundToCustomerWithNesting, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(2));
        }

        [Test]
        public void can_detect_bad_nested_data_contexts()
        {
            var validator = Validator.For<UIBoundToCustomerWithContextNesting, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(3));
        }

        [Test]
        public void can_confirm_bound_properties_on_nested_paths()
        {
            var validator = Validator.For<UIBoundToCustomerWithNesting, Customer>();
            var result = validator.Validate();

            Assert.That(result.WasBoundTo(x => x.MailingAddress.City));
            Assert.That(result.WasNotBoundTo(x => x.BillingAddress.Street2));
        }

        [Test]
        public void can_confirm_bound_properties_on_nested_data_contexts()
        {
            var validator = Validator.For<UIBoundToCustomerWithContextNesting, Customer>();
            var result = validator.Validate();

            Assert.That(result.WasBoundTo(x => x.MailingAddress.Street2));
            Assert.That(result.WasNotBoundTo(x => x.BillingAddress.Street2));
        }
    }
}