using Shouldly;

namespace Tests.Caliburn.Testability
{
    using System.Linq;
    using Fakes.Model;
    using Fakes.UI;
    using global::Caliburn.Testability;
    using Xunit;

    
    public class When_testing_multibindings
    {
        [WpfFact]
        public void can_detect_bad_multibindings_in_styles()
        {
            var validator = Validator.For<UIWithMultibindingToStyle, Customer>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(1);
        }

        [WpfFact]
        public void can_detect_bad_multibindings_in_triggers()
        {
            var validator = Validator.For<UIWithMultibindingStyleAndTriggers, Customer>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(1);
        }

        [WpfFact]
        public void can_detect_bad_multibindings_on_elements()
        {
            var validator = Validator.For<SimpleUIWithMultibinding, Customer>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(1);
        }
    }
}