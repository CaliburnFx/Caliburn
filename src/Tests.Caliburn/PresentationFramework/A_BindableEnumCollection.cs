using Shouldly;

namespace Tests.Caliburn.PresentationFramework
{
    using System.Linq;
    using Fakes;
    using global::Caliburn.PresentationFramework.ViewModels;
    using Xunit;

    
    public class A_BindableEnumCollection
    {
        [Fact]
        public void could_be_created_against_byte_enum()
        {
            var bindable = new BindableEnumCollection<ByteEnum>();
            bindable.ShouldNotBeNull();
        }

        [Fact]
        public void could_be_created_against_integer_enum()
        {
            var bindable = new BindableEnumCollection<IntegerEnum>();
            bindable.ShouldNotBeNull();
        }

        [Fact]
        public void should_contain_valid_BindableEnum()
        {
            var bindable = new BindableEnumCollection<IntegerEnum>();
            bindable.Count.ShouldBe(3);
            bindable.ShouldAllBe(x => x is BindableEnum);
            bindable.Select(x => x.DisplayName).SequenceEqual(new []{"Int0", "Int1", "Int2"}).ShouldBeTrue();
            bindable.Select(x => x.UnderlyingValue).SequenceEqual(new []{0, 1, 2}).ShouldBeTrue();
        }
    }
}