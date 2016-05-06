using Shouldly;

namespace Tests.Caliburn.RoutedUIMessaging.Parsers
{
    using global::Caliburn.PresentationFramework.RoutedMessaging.Parsers;
    using global::Caliburn.PresentationFramework.RoutedMessaging.Triggers;
    using Xunit;

    
    public class The_event_trigger_parser : TestBase
    {
        EventTriggerParser parser;

        protected override void given_the_context_of()
        {
            parser = new EventTriggerParser();
        }

        [Fact]
        public void can_parse_a_string_into_a_trigger()
        {
            const string eventName = "Click";

            var result = parser.Parse(null, eventName) as EventMessageTrigger;

            result.ShouldNotBeNull();
            result.EventName.ShouldBe(eventName);
        }
    }
}