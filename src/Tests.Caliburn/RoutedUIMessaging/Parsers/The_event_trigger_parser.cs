namespace Tests.Caliburn.RoutedUIMessaging.Parsers
{
    using global::Caliburn.PresentationFramework.RoutedMessaging.Parsers;
    using global::Caliburn.PresentationFramework.RoutedMessaging.Triggers;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class The_event_trigger_parser : TestBase
    {
        private EventTriggerParser _parser;

        protected override void given_the_context_of()
        {
            _parser = new EventTriggerParser();
        }

        [Test]
        public void can_parse_a_string_into_a_trigger()
        {
            const string eventName = "Click";

            var result = _parser.Parse(null, eventName) as EventMessageTrigger;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.EventName, Is.EqualTo(eventName));
        }
    }
}