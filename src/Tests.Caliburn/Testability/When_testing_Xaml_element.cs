using Shouldly;

namespace Tests.Caliburn.Testability
{
    using System.Linq;
    using Fakes.Model;
    using Controls;
    using global::Caliburn.Testability;
    using Xunit;


    public class When_testing_Xaml_element : TestBase
    {
        [StaFact]
        public void Simple_Xaml_is_loaded_correctly()
        {
            var validator = Validator.For<SimpleUIBuiltWithXaml, Customer>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(0);
        }

        [StaFact]
        public void Xaml_with_markup_extensions_is_loaded_correctly()
        {
            var validator = Validator.For<UIWithMarkupExtensionsBuiltWithXaml, Customer>();
            var result = validator.Validate();

            result.Errors.Count().ShouldBe(0);
        }
    }
}
