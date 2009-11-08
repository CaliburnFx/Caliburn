using System.Linq;
using Caliburn.Testability;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Tests.Caliburn.Fakes.Model;
using Tests.Caliburn.Fakes.UI;

namespace Tests.Caliburn.Testability
{
    [TestFixture]
    public class When_testing_basic_binding : TestBase
    {
        [Test]
        public void can_detect_bad_property_paths()
        {
            var validator = Validator.For<SimpleUIBoundToCustomer, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(2));
        }

        [Test]
        public void can_confirm_simple_bound_properties()
        {
            var validator = Validator.For<SimpleUIBoundToCustomer, Customer>();
            var result = validator.Validate();

            Assert.That(result.WasBoundTo(x => x.FirstName));
            Assert.That(result.WasNotBoundTo(x => x.LastName));
        }

        [Test]
        public void can_detect_bad_attached_property_bindings()
        {
            var validator = Validator.For<SimpleUIBoundToCustomerByAttachedPorperty, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_confirm_bindings_on_attached_properties()
        {
            var validator = Validator.For<SimpleUIBoundToCustomerByAttachedPorperty, Customer>();
            var result = validator.Validate();

            Assert.That(result.WasBoundTo(x => x.IQ));
            Assert.That(result.WasNotBoundTo(x => x.Age));
        }

        [Test]
        public void can_detect_bad_bindings_with_indexers()
        {
            var validator = Validator.For<UIBoundToCustomerIndexer, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void can_detect_bad_bindings_in_hierarchicl_paths()
        {
            var validator = Validator.For<UIWithHierarchicalPath, MyDataSource>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(0));
        }
    }
}