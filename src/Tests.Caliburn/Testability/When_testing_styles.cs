using Shouldly;

namespace Tests.Caliburn.Testability
{
    using System.Linq;
    using Fakes.Model;
    using Fakes.UI;
    using global::Caliburn.Testability;
    using Xunit;


    public class When_testing_styles : TestBase
    {
        [StaFact]
        public void can_locate_errors_in_container_styles()
        {
            var validator = Validator.For<UIWithItemsControlContainerStyle, Customer>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(1);
        }

        [StaFact]
        public void can_locate_errors_in_group_styles()
        {
            var validator = Validator.For<UIWithItemsControlGroupStyle, Customer>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(1);
        }

        [StaFact]
        public void can_locate_errors_in_multi_triggers()
        {
            var validator = Validator.For<UIBoundToCustomerWithStyleAndMultiTriggers, Customer>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(1);
        }

        [StaFact]
        public void can_locate_errors_in_setters()
        {
            var validator = Validator.For<UIBoundToCustomerWithStyle, Customer>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(1);
        }

        [StaFact]
        public void can_locate_errors_in_triggers()
        {
            var validator = Validator.For<UIBoundToCustomerWithStyleAndTriggers, Customer>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(1);
        }
    }
}
