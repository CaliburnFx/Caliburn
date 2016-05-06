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
    using Rhino.Mocks;

    
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

            node.Expect(x => x.UIElement).Return(element);


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

                node.Expect(x => x.UIElement).Return(new DependencyObject()).Repeat.Twice();


                trigger.Attach(node);
            });
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

            node.Expect(x => x.UIElement).Return(element);
            node.Expect(x => x.ProcessMessage(Arg<IRoutedMessage>.Is.Equal(message), Arg<EventArgs>.Is.Equal(args)));


            trigger.Attach(node);
            element.RaiseEvent(args);
        }

        [Fact]
        public void can_update_availability()
        {
            var trigger = new AttachedEventMessageTrigger
            {
                Message = message,
                RoutedEvent = FakeElement.RoutedEvent
            };

            node.Expect(x => x.UIElement).Return(element);

            node.Expect(x => x.UIElement).Return(element);
            message.AvailabilityEffect.Expect(x => x.ApplyTo(element, false));


            trigger.Attach(node);
            trigger.UpdateAvailabilty(false);
        }
    }
}