using Shouldly;

namespace Tests.Caliburn.RoutedUIMessaging.Triggers
{
    using System;
    using Fakes;
    using Fakes.UI;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using global::Caliburn.PresentationFramework.RoutedMessaging.Triggers;
    using Xunit;
    using NSubstitute;


    public class The_event_message_trigger : TestBase
    {
        IInteractionNode node;
        FakeElement element;
        FakeMessage message;

        protected override void given_the_context_of()
        {
            node = Mock<IInteractionNode>();

            element = new FakeElement();
            message = new FakeMessage {AvailabilityEffect = Mock<IAvailabilityEffect>()};
        }

        [Fact]
        public void can_attach_itself_to_an_element()
        {
            var trigger = new EventMessageTrigger
            {
                Message = message,
                EventName = FakeElement.EventName
            };

            node.UIElement.Returns(element);

            trigger.Attach(node);

            trigger.Node.ShouldBe(node);
            message.InvalidatedHandler.ShouldNotBeNull();
            message.InitializeCalledWith.ShouldBe(node);
        }

        [Fact]
        public void can_trigger_message_processing()
        {
            var trigger = new EventMessageTrigger
            {
                Message = message,
                EventName = FakeElement.EventName
            };

            node.UIElement.Returns(element);

            trigger.Attach(node);
            element.RaiseClick();
            node.Received().ProcessMessage(Arg.Is<IRoutedMessage>(x => x == message),
                Arg.Any<EventArgs>());
        }

        [Fact]
        public void can_update_availability()
        {
            var trigger = new EventMessageTrigger
            {
                Message = message,
                EventName = FakeElement.EventName
            };

            node.UIElement.Returns(element);

            node.UIElement.Returns(element);


            trigger.Attach(node);
            trigger.UpdateAvailabilty(false);
            message.AvailabilityEffect.Received().ApplyTo(element, false);
        }
    }
}
