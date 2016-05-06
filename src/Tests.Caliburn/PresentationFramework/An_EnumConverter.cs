using Shouldly;

namespace Tests.Caliburn.PresentationFramework
{
    using Fakes;
    using global::Caliburn.PresentationFramework.Converters;
    using global::Caliburn.PresentationFramework.ViewModels;
    using Xunit;

    
    public class An_EnumConverter : TestBase
    {
        EnumConverter theEnumConverter;

        protected override void given_the_context_of()
        {
            theEnumConverter = new EnumConverter();
        }

        [Fact]
        public void can_convert_byte_enum_to_bindable()
        {
            var converted = theEnumConverter.Convert(ByteEnum.Byte1, typeof(object), null, null);
            converted.ShouldBeOfType<BindableEnum>();

            var bindable = (BindableEnum)converted;
            bindable.UnderlyingValue.ShouldBe(1);
            bindable.Value.ShouldBe(ByteEnum.Byte1);
        }

        [Fact]
        public void can_convert_byte_enum_to_byte()
        {
            var converted = theEnumConverter.Convert(ByteEnum.Byte1, typeof(byte), null, null);
            converted.ShouldBeOfType<byte>();
            converted.ShouldBe((byte)1);
        }

        [Fact]
        public void can_convert_byte_enum_to_int()
        {
            var converted = theEnumConverter.Convert(ByteEnum.Byte1, typeof(int), null, null);
            converted.ShouldBeOfType<int>();
            converted.ShouldBe(1);
        }

        [Fact]
        public void can_convert_integer_enum_to_bindable()
        {
            var converted = theEnumConverter.Convert(IntegerEnum.Int1, typeof(object), null, null);
            converted.ShouldBeOfType<BindableEnum>();

            var bindable = (BindableEnum)converted;
            bindable.UnderlyingValue.ShouldBe(1);
            bindable.Value.ShouldBe(IntegerEnum.Int1);
        }

        [Fact]
        public void can_convert_integer_enum_to_int()
        {
            var converted = theEnumConverter.Convert(IntegerEnum.Int1, typeof(int), null, null);
            converted.ShouldBe(1);
        }
    }
}