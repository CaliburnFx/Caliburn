using Shouldly;

namespace Tests.Caliburn.Testability
{
    using System.Linq;
    using Fakes.Model;
    using Fakes.UI;
    using global::Caliburn.Testability;
    using Xunit;

    
    public class When_testing_priority_bindings
    {
        [WpfFact]
        public void can_detect_bad_prioritybindings_in_styles()
        {
            var validator = Validator.For<UIWithPrioritybindingToStyle, Customer>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(1);
        }

        [WpfFact]
        public void can_detect_bad_prioritybindings_in_triggers()
        {
            var validator = Validator.For<UIWithPrioritybindingStyleAndTriggers, Customer>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(1);
        }

        [WpfFact]
        public void can_detect_bad_prioritybindings_on_elements()
        {
            var validator = Validator.For<SimpleUIWithPriorityBinding, Customer>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(1);
        }
    }
}