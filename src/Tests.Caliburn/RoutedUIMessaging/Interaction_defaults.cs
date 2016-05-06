using Shouldly;

namespace Tests.Caliburn.RoutedUIMessaging
{
    using System.Windows.Controls;
    using global::Caliburn.PresentationFramework.Conventions;
    using global::Caliburn.PresentationFramework.RoutedMessaging.Triggers;
    using Xunit;

    
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

        [Fact]
        public void declare_an_element_type()
        {
            defaults.Type.ShouldBe(typeof(Button));
        }

        [Fact]
        public void can_provide_a_default_trigger_as_an_event_trigger()
        {
            var trigger = defaults.CreateTrigger();

            trigger.ShouldNotBeNull();
            trigger.ShouldBeOfType<EventMessageTrigger>();
        }

        [WpfFact]
        public void can_get_a_default_value()
        {
            var value = new object();
            var button = new Button {DataContext = value};

            var result = defaults.GetValue(button);

            result.ShouldBe(value);
        }

        [WpfFact]
        public void can_set_a_default_value()
        {
            var value = new object();
            var button = new Button();

            defaults.SetValue(button, value);

            button.DataContext.ShouldBe(value);
        }
    }
}