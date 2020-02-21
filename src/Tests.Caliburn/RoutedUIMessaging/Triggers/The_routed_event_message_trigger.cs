using Shouldly;

namespace Tests.Caliburn.RoutedUIMessaging.Triggers
{
    using System;
    using System.Windows;
    using Fakes;
    using Fakes.UI;
    using global::Caliburn.Core;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using global::Caliburn.PresentationFramework.RoutedMessaging.Triggers;
    using Xunit;
    using NSubstitute;


    public class The_routed_event_message_trigger : TestBase
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
            var trigger = new AttachedEventMessageTrigger
            {
                Message = message,
                RoutedEvent = FakeElement.RoutedEvent
            };

            node.UIElement.Returns(element);

            trigger.Attach(node);

            trigger.Node.ShouldBe(node);
            message.InvalidatedHandler.ShouldNotBeNull();
            message.InitializeCalledWith.ShouldBe(node);
        }

        [Fact]
        public void throws_exception_if_attempt_to_attach_to_non_UIElement()
        {
            Assert.Throws<CaliburnException>(() =>{
                var trigger = new AttachedEventMessageTrigger {
                    Message = message,
                    RoutedEvent = FakeElement.RoutedEvent
                };

                node.UIElement.Returns(new DependencyObject());

                trigger.Attach(node);
            });
            var uiElement = node.Received(2).UIElement;
            uiElement.ShouldBeNull();
        }

        [Fact]
        public void can_trigger_message_processing()
        {
            var trigger = new AttachedEventMessageTrigger
            {
                Message = message,
                RoutedEvent = FakeElement.RoutedEvent
            };

            var args = new RoutedEventArgs(FakeElement.RoutedEvent, element);

            node.UIElement.Returns(element);

            trigger.Attach(node);
            element.RaiseEvent(args);
            node.Received().ProcessMessage(Arg.Is<IRoutedMessage>(x => x ==message),
                Arg.Is<EventArgs>(x => x == args));
        }

        [Fact]
        public void can_update_availability()
        {
            var trigger = new AttachedEventMessageTrigger
            {
                Message = message,
                RoutedEvent = FakeElement.RoutedEvent
            };

            node.UIElement.Returns(element);

            node.UIElement.Returns(element);

            trigger.Attach(node);
            trigger.UpdateAvailabilty(false);
            message.AvailabilityEffect.Received().ApplyTo(element, false);
        }
    }
}
