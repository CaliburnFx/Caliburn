namespace Tests.Caliburn.PresentationFramework
{
    using System.Linq;
    using Fakes;
    using global::Caliburn.PresentationFramework.ViewModels;
    using NUnit.Framework;

    [TestFixture]
    public class A_BindableEnumCollection
    {
        [Test]
        public void could_be_created_against_byte_enum()
        {
            var bindable = new BindableEnumCollection<ByteEnum>();
            Assert.That(bindable, Is.Not.Null);
        }

        [Test]
        public void could_be_created_against_integer_enum()
        {
            var bindable = new BindableEnumCollection<IntegerEnum>();
            Assert.That(bindable, Is.Not.Null);
        }

        [Test]
        public void should_contain_valid_BindableEnum()
        {
            var bindable = new BindableEnumCollection<IntegerEnum>();
            Assert.That(bindable, Has.Count.EqualTo(3));
            Assert.That(bindable, Has.All.AssignableTo<BindableEnum>());
            Assert.That(bindable.Select(x => x.DisplayName).ToArray(), Is.EquivalentTo(new[] {
                "Int0", "Int1", "Int2"
            }));
            Assert.That(bindable.Select(x => x.UnderlyingValue).ToArray(), Is.EquivalentTo(new[] {
                0, 1, 2
            }));
        }
    }
}