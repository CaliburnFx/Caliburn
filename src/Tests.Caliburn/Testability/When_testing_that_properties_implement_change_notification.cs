namespace Tests.Caliburn.Testability
{
    using System;
    using ChangeNotificationSamples;
    using global::Caliburn.Testability.Extensions;
    using NUnit.Framework;

    [TestFixture]
    public class When_testing_that_properties_implement_change_notification
    {
        [Test]
        public void a_notification_with_the_incorrect_property_name_will_fail()
        {
            Assert.Throws(Is.InstanceOf<Exception>(), () =>{
                var sut = new NotificationWithWrongName();
                sut.AssertThatAllProperties().RaiseChangeNotification();
            });
        }

        [Test]
        public void by_default_reference_type_properties_will_be_set_to_null()
        {
            var sut = new PropertyWithReferenceType();
            sut.AssertThatAllProperties().RaiseChangeNotification();

            Assert.That(sut.SomeField, Is.Null);
        }

        [Test]
        public void can_test_a_single_property()
        {
            var sut = new NotificationOnAllProperties();

            sut.AssertThatProperty(x => x.String)
                .RaisesChangeNotification();
        }

        [Test]
        public void if_the_class_has_no_eligible_properties_the_assertion_will_fail()
        {
            Assert.Throws(Is.InstanceOf<Exception>(), () =>{
                var sut = new NoNotificationNecessary();
                sut.AssertThatAllProperties().RaiseChangeNotification();
            });
        }

        [Test]
        public void some_properties_can_be_ignored()
        {
            var sut = new PartialNotification();

            sut.AssertThatAllProperties()
                .Ignoring(x => x.NoNotification).RaiseChangeNotification();
        }

        [Test]
        public void some_properties_in_base_class_can_be_ignored()
        {
            var sut = new ChildNotification();

            sut.AssertThatAllProperties()
                .Ignoring(x => x.NoNotification)
                .RaiseChangeNotification();
        }

        [Test]
        public void specific_values_can_be_set_on_individual_properties()
        {
            var sut = new NotificationOnAllProperties();

            sut.AssertThatAllProperties()
                .SetValue(x => x.String, "some_string").RaiseChangeNotification();

            Assert.That(sut.String, Is.EqualTo("some_string"));
        }

        [Test]
        public void the_assertion_will_pass_if_notification_is_correct()
        {
            var sut = new NotificationOnAllProperties();

            sut.AssertThatAllProperties().RaiseChangeNotification();
        }
    }
}