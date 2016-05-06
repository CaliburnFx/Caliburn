using Shouldly;

namespace Tests.Caliburn.RoutedUIMessaging.Parsers
{
    using System.Windows.Input;
    using global::Caliburn.PresentationFramework.RoutedMessaging.Parsers;
    using global::Caliburn.PresentationFramework.RoutedMessaging.Triggers;
    using Xunit;

    
    public class The_gesture_trigger_parser : TestBase
    {
        GestureTriggerParser parser;

        protected override void given_the_context_of()
        {
            parser = new GestureTriggerParser();
        }

        [Fact]
        public void can_parse_a_key_combination()
        {
            var result = parser.Parse(null, "Key: S, Modifiers: Control") as GestureMessageTrigger;

            result.ShouldNotBeNull();
            result.Key.ShouldBe(Key.S);
            result.Modifiers.ShouldBe(ModifierKeys.Control);
        }

        [Fact]
        public void can_parse_a_mouse_action()
        {
            var result = parser.Parse(null, "MouseAction: LeftDoubleClick, Modifiers: Alt") as GestureMessageTrigger;

            result.ShouldNotBeNull();
            result.MouseAction.ShouldBe(MouseAction.LeftDoubleClick);
            result.Modifiers.ShouldBe(ModifierKeys.Alt);
        }
    }
}