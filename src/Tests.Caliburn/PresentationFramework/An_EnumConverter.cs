namespace Tests.Caliburn.PresentationFramework
{
    using Fakes;
    using global::Caliburn.PresentationFramework.Converters;
    using global::Caliburn.PresentationFramework.ViewModels;
    using NUnit.Framework;

    [TestFixture]
    public class An_EnumConverter : TestBase
    {
        EnumConverter theEnumConverter;

        protected override void given_the_context_of()
        {
            theEnumConverter = new EnumConverter();
        }

        [Test]
        public void can_convert_byte_enum_to_bindable()
        {
            var converted = theEnumConverter.Convert(ByteEnum.Byte1, typeof(object), null, null);
            Assert.That(converted, Is.InstanceOf<BindableEnum>());

            var bindable = (BindableEnum)converted;
            Assert.That(bindable.UnderlyingValue, Is.EqualTo(1));
            Assert.That(bindable.Value, Is.EqualTo(ByteEnum.Byte1));
        }

        [Test]
        public void can_convert_byte_enum_to_byte()
        {
            var converted = theEnumConverter.Convert(ByteEnum.Byte1, typeof(byte), null, null);
            Assert.That(converted, Is.InstanceOf<byte>());
            Assert.That(converted, Is.EqualTo((byte)1));
        }

        [Test]
        public void can_convert_byte_enum_to_int()
        {
            var converted = theEnumConverter.Convert(ByteEnum.Byte1, typeof(int), null, null);
            Assert.That(converted, Is.InstanceOf<int>());
            Assert.That(converted, Is.EqualTo(1));
        }

        [Test]
        public void can_convert_integer_enum_to_bindable()
        {
            var converted = theEnumConverter.Convert(IntegerEnum.Int1, typeof(object), null, null);
            Assert.That(converted, Is.InstanceOf<BindableEnum>());

            var bindable = (BindableEnum)converted;
            Assert.That(bindable.UnderlyingValue, Is.EqualTo(1));
            Assert.That(bindable.Value, Is.EqualTo(IntegerEnum.Int1));
        }

        [Test]
        public void can_convert_integer_enum_to_int()
        {
            var converted = theEnumConverter.Convert(IntegerEnum.Int1, typeof(int), null, null);
            Assert.That(converted, Is.EqualTo(1));
        }
    }
}