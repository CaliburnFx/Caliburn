using Shouldly;

namespace Tests.Caliburn.Testability
{
    using System.Linq;
    using Fakes.Model;
    using Fakes.UI;
    using global::Caliburn.Testability;
    using Xunit;


    public class When_testing_basic_binding : TestBase
    {
        [StaFact]
        public void can_confirm_bindings_on_attached_properties()
        {
            var validator = Validator.For<SimpleUIBoundToCustomerByAttachedPorperty, Customer>();
            var result = validator.Validate();

            result.WasBoundTo(x => x.IQ).ShouldBeTrue();
            result.WasNotBoundTo(x => x.Age).ShouldBeTrue();
        }

        [StaFact]
        public void can_confirm_simple_bound_properties()
        {
            var validator = Validator.For<SimpleUIBoundToCustomer, Customer>();
            var result = validator.Validate();

            result.WasBoundTo(x => x.FirstName).ShouldBeTrue();
            result.WasNotBoundTo(x => x.LastName).ShouldBeTrue();
        }

        [StaFact]
        public void can_detect_bad_attached_property_bindings()
        {
            var validator = Validator.For<SimpleUIBoundToCustomerByAttachedPorperty, Customer>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(1);
        }

        [StaFact]
        public void can_detect_bad_bindings_in_hierarchicl_paths()
        {
            var validator = Validator.For<UIWithHierarchicalPath, MyDataSource>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(0);
        }

        [StaFact]
        public void can_detect_bad_bindings_with_indexers()
        {
            var validator = Validator.For<UIBoundToCustomerIndexer, Customer>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(1);
        }

        [StaFact]
        public void can_detect_bad_property_paths()
        {
            var validator = Validator.For<SimpleUIBoundToCustomer, Customer>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(2);
        }
    }
}
