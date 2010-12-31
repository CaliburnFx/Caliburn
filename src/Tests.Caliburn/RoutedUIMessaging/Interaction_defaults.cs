namespace Tests.Caliburn.RoutedUIMessaging
{
    using System.Windows.Controls;
    using global::Caliburn.PresentationFramework.Conventions;
    using global::Caliburn.PresentationFramework.RoutedMessaging.Triggers;
    using NUnit.Framework;

    [TestFixture]
    public class Interaction_defaults : TestBase
    {
        DefaultElementConvention<Button> defaults;

        protected override void given_the_context_of()
        {
            defaults = new DefaultElementConvention<Button>(
                "Click",
                Button.ContentProperty,
                (c, v) => c.DataContext = v,
                c => c.DataContext,
                null
                );
        }

        [Test]
        public void declare_an_element_type()
        {
            Assert.That(defaults.Type, Is.EqualTo(typeof(Button)));
        }

        [Test]
        public void can_provide_a_default_trigger_as_an_event_trigger()
        {
            var trigger = defaults.CreateTrigger();

            Assert.That(trigger, Is.Not.Null);
            Assert.That(trigger, Is.InstanceOf<EventMessageTrigger>());
        }

        [Test]
        public void can_get_a_default_value()
        {
            var value = new object();
            var button = new Button {DataContext = value};

            var result = defaults.GetValue(button);

            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void can_set_a_default_value()
        {
            var value = new object();
            var button = new Button();

            defaults.SetValue(button, value);

            Assert.That(button.DataContext, Is.EqualTo(value));
        }
    }
}