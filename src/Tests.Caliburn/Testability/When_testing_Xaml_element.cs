namespace Tests.Caliburn.Testability
{
    using System.Linq;
    using Fakes.Model;
    using Fakes.UI;
    using global::Caliburn.Testability;
    using NUnit.Framework;

    [TestFixture]
    public class When_testing_Xaml_element : TestBase
    {
        [Test]
        public void Simple_Xaml_is_loaded_correctly()
        {
            var validator = Validator.For<SimpleUIBuiltWithXaml, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Xaml_with_markup_extensions_is_loaded_correctly()
        {
            var validator = Validator.For<UIWithMarkupExtensionsBuiltWithXaml, Customer>();
            var result = validator.Validate();

            Assert.That(result.Errors.Count(), Is.EqualTo(0));
        }
    }
}