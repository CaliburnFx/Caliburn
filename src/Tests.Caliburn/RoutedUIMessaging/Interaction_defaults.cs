namespace Tests.Caliburn.RoutedUIMessaging
{
    using System.Windows.Controls;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.PresentationFramework.Conventions;
    using global::Caliburn.PresentationFramework.Triggers;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class Interaction_defaults : TestBase
    {
        private DefaultElementConvention<Button> _defaults;

        protected override void given_the_context_of()
        {
            _defaults = new DefaultElementConvention<Button>(
                Mock<IEventHandlerFactory>(),
                "Click",
                Button.ContentProperty,
                (c, v) => c.DataContext = v,
                c => c.DataContext
                );
        }

        [Test]
        public void declare_an_element_type()
        {
            Assert.That(_defaults.Type, Is.EqualTo(typeof(Button)));
        }

        [Test]
        public void can_provide_a_default_trigger_as_an_event_trigger()
        {
            var trigger = _defaults.CreateTrigger();

            Assert.That(trigger, Is.Not.Null);
            Assert.That(trigger, Is.InstanceOfType(typeof(EventMessageTrigger)));
        }

        [Test]
        public void can_get_a_default_value()
        {
            var value = new object();
            var button = new Button {DataContext = value};

            var result = _defaults.GetValue(button);

            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void can_set_a_default_value()
        {
            var value = new object();
            var button = new Button();

            _defaults.SetValue(button, value);

            Assert.That(button.DataContext, Is.EqualTo(value));
        }
    }
}