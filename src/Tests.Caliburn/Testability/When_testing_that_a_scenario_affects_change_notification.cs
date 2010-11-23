using System;
using Caliburn.Testability.Extensions;
using NUnit.Framework;
using Tests.Caliburn.Testability.ChangeNotificationSamples;

namespace Tests.Caliburn.Testability
{
    [TestFixture]
    public class When_testing_that_a_scenario_affects_change_notification
    {
        [Test]
        public void the_assertion_fails_if_notification_is_not_raised()
        {
            Assert.Throws(Is.InstanceOf<Exception>(), () =>{
                var sut = new NoNotification();

                sut.AssertThatChangeNotificationIsRaisedBy(x => x.PropertyWithoutNotification)
                    .When(() => sut.PropertyWithoutNotification = "something");
            });
        }

        [Test]
        public void the_assertion_succeeds_if_notification_is_raised()
        {
            var sut = new NotificationOnAllProperties();

            sut.AssertThatChangeNotificationIsRaisedBy(x=>x.Int)
                .When(() => sut.Int = 99);
        }

        [Test, Ignore("My approach using the destructor doesn't quite work. Is there a way to do this?")]
        public void an_incomplete_assertion_will_fail()
        {
            Assert.Throws(Is.InstanceOf<Exception>(), () =>{
                var sut = new NotificationOnAllProperties();
                sut.AssertThatChangeNotificationIsRaisedBy(x => x.Int);
            });
        }
    }
}