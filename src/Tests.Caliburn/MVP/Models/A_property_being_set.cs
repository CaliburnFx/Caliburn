namespace Tests.Caliburn.MVP.Models
{
    using System;
    using global::Caliburn.ModelFramework;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class A_property_being_set : TestBase
    {
        private const string _propertyName = "MyProperty";

        [Test]
        public void interrogates_with_its_change_aware_metadata()
        {
            var definition = new ModelDefinition();
            var myProperty = definition.AddProperty(_propertyName, () => 0);
            var metadata = new FakePropertyChangeAware();

            myProperty.AddMetadata(metadata);
            var instance = definition.CreateInstance();

            instance.BeginEdit();
            instance.SetValue(myProperty, 5);

            Assert.That(metadata.PassedInProperty, Is.EqualTo(instance[_propertyName]));
        }

        [Test]
        public void converts_when_set_with_an_untyped_value()
        {
            var definition = new ModelDefinition();
            var myProperty = definition.AddProperty(_propertyName, () => default(DateTime));
            var metadata = new FakePropertyValueConverter();

            myProperty.AddMetadata(metadata);

            var instance = definition.CreateInstance();
            var property = instance[_propertyName];

            property.UntypedValue = new DateTime(1979, 10, 27).ToShortDateString();

            Assert.That(metadata.ConverterWasCalled);
            Assert.That(instance.GetValue(myProperty), Is.EqualTo(new DateTime(1979, 10, 27)));
        }

        [Test]
        public void converts_back_when_got_from_the_untyped_value()
        {
            var definition = new ModelDefinition();
            var myProperty = definition.AddProperty(_propertyName, () => default(DateTime));
            var metadata = new FakePropertyValueConverter();

            myProperty.AddMetadata(metadata);

            var instance = definition.CreateInstance();
            var property = instance[_propertyName];

            property.UntypedValue = new DateTime(1979, 10, 27).ToShortDateString();

            var value = property.UntypedValue;

            Assert.That(value, Is.EqualTo(new DateTime(1979, 10, 27).ToShortDateString()));
            Assert.That(metadata.ConvertBackWasCalled);
        }

        [Test]
        public void validates_itself()
        {
            var definition = new ModelDefinition();
            var myProperty = definition.AddProperty(_propertyName, () => string.Empty);
            var metadata = new FakePropertyValidator();

            myProperty.AddMetadata(metadata);

            var instance = definition.CreateInstance();

            instance.BeginEdit();

            Assert.That(instance[_propertyName].IsValid);
            instance.SetValue(myProperty, "hello");
            Assert.That(instance[_propertyName].IsValid, Is.False);
        }

        [Test]
        public void cancels_interrogation_when_an_interrogator_returns_false()
        {
            var definition = new ModelDefinition();
            var myProperty = definition.AddProperty(_propertyName, () => string.Empty);
            var metadata1 = new FakePropertyValidator();
            var metadata2 = new FakePropertyChangeAware();

            myProperty.AddMetadata(metadata1);
            myProperty.AddMetadata(metadata2);

            var instance = definition.CreateInstance();

            instance.BeginEdit();

            instance.SetValue(myProperty, "hello");

            Assert.That(instance[_propertyName].IsValid, Is.False);
            Assert.That(metadata2.PassedInProperty, Is.Null);
        }

        public class FakePropertyChangeAware : IPropertyChangeAware<int>
        {
            public IProperty<int> PassedInProperty;

            public bool Interrogate(IProperty<int> instance)
            {
                PassedInProperty = instance;
                return true;
            }
        }

        public class FakePropertyValueConverter : IPropertyValueConverter<DateTime>
        {
            public bool ConverterWasCalled;
            public bool ConvertBackWasCalled;

            public DateTime Convert(IProperty<DateTime> property, object proposedValue)
            {
                ConverterWasCalled = true;
                return System.Convert.ToDateTime(proposedValue);
            }

            public object ConvertBack(IProperty<DateTime> property, DateTime currentValue)
            {
                ConvertBackWasCalled = true;
                return currentValue.ToShortDateString();
            }
        }

        public class FakePropertyValidator : IPropertyValidator<string>
        {
            public bool Interrogate(IProperty<string> instance)
            {
                instance.ValidationResults.Add(new SimpleValidationResult("Error"));
                return false;
            }
        }
    }
}