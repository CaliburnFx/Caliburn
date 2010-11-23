namespace Tests.Caliburn.RoutedUIMessaging.Triggers
{
    using System;
    using Fakes;
    using Fakes.UI;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using global::Caliburn.PresentationFramework.RoutedMessaging.Triggers;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class The_event_message_trigger : TestBase
    {
        private IInteractionNode _node;
        private FakeElement _element;
        private FakeMessage _message;

        protected override void given_the_context_of()
        {
            _node = Mock<IInteractionNode>();

            _element = new FakeElement();
            _message = new FakeMessage {AvailabilityEffect = Mock<IAvailabilityEffect>()};
        }

        [Test]
        public void can_attach_itself_to_an_element()
        {
            var trigger = new EventMessageTrigger
            {
                Message = _message,
                EventName = FakeElement.EventName
            };

            _node.Expect(x => x.UIElement).Return(_element).Repeat.Any();

            trigger.Attach(_node);

            Assert.That(trigger.Node, Is.EqualTo(_node));
            Assert.That(_message.InvalidatedHandler, Is.Not.Null);
            Assert.That(_message.InitializeCalledWith, Is.EqualTo(_node));
        }

        [Test]
        public void can_trigger_message_processing()
        {
            var trigger = new EventMessageTrigger
            {
                Message = _message,
                EventName = FakeElement.EventName
            };

            _node.Expect(x => x.UIElement).Return(_element).Repeat.Any();
            _node.Expect(x => x.ProcessMessage(Arg<IRoutedMessage>.Is.Equal(_message), Arg<EventArgs>.Is.TypeOf));


            trigger.Attach(_node);
            _element.RaiseClick();
        }

        [Test]
        public void can_update_availability()
        {
            var trigger = new EventMessageTrigger
            {
                Message = _message,
                EventName = FakeElement.EventName
            };

            _node.Expect(x => x.UIElement).Return(_element);

            _node.Expect(x => x.UIElement).Return(_element);
            _message.AvailabilityEffect.Expect(x => x.ApplyTo(_element, false));


            trigger.Attach(_node);
            trigger.UpdateAvailabilty(false);
        }
    }
}