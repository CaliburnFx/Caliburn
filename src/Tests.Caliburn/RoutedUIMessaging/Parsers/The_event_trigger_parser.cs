namespace Tests.Caliburn.RoutedUIMessaging.Parsers
{
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.PresentationFramework.Parsers;
    using global::Caliburn.PresentationFramework.Triggers;
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class The_event_trigger_parser : TestBase
    {
        private EventTriggerParser _parser;

        protected override void given_the_context_of()
        {
            _parser = new EventTriggerParser();

            var container = Stub<IServiceLocator>();
            container.Stub(x => x.GetInstance<IEventHandlerFactory>()).Return(Stub<IEventHandlerFactory>()).Repeat.Any();

            ServiceLocator.SetLocatorProvider(() => container);
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