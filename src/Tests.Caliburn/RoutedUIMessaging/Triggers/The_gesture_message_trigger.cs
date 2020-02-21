using Shouldly;

namespace Tests.Caliburn.RoutedUIMessaging.Triggers
{
    using System.Windows;
    using System.Windows.Input;
    using Fakes;
    using Fakes.UI;
    using global::Caliburn.Core;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using global::Caliburn.PresentationFramework.RoutedMessaging.Triggers;
    using global::Caliburn.PresentationFramework.RoutedMessaging.Triggers.Support;
    using Xunit;
    using NSubstitute;


    public class The_gesture_message_trigger : TestBase
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
        public void can_attach_itself_to_an_element_using_a_mouse_gesture()
        {
            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Control,
                MouseAction = MouseAction.LeftDoubleClick,
                Message = message
            };

            node.UIElement.Returns(element);


            trigger.Attach(node);

            trigger.Node.ShouldBe(node);
            message.InvalidatedHandler.ShouldNotBeNull();
            message.InitializeCalledWith.ShouldBe(node);

            var binding = element.InputBindings[0];
            var gesture = binding.Gesture as MouseGesture;

            binding.Command.ShouldBe(new GestureMessageTrigger.GestureCommand(binding.Gesture));
            gesture.MouseAction.ShouldBe(MouseAction.LeftDoubleClick);
            gesture.Modifiers.ShouldBe(ModifierKeys.Control);
        }

        [Fact]
        public void can_attach_itself_to_an_element_using_a_key_gesture()
        {
            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = message
            };

            node.UIElement.Returns(element);


            trigger.Attach(node);

            trigger.Node.ShouldBe(node);
            message.InvalidatedHandler.ShouldNotBeNull();
            message.InitializeCalledWith.ShouldBe(node);

            var binding = element.InputBindings[0];
            var gesture = binding.Gesture as UnrestrictedKeyGesture;

            binding.Command.ShouldBe(new GestureMessageTrigger.GestureCommand(binding.Gesture));
            gesture.Key.ShouldBe(Key.S);
            gesture.Modifiers.ShouldBe(ModifierKeys.Alt);
        }

        [Fact]
        public void throws_exception_if_attempt_to_attach_to_non_UIElement()
        {
            Assert.Throws<CaliburnException>(() =>{
                var trigger = new GestureMessageTrigger {
                    Modifiers = ModifierKeys.Alt,
                    Key = Key.S,
                    Message = message
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
            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = message
            };

            object parameter = new object();

            node.UIElement.Returns(element);

            trigger.Attach(node);
            element.InputBindings[0].Command.Execute(parameter);
            node.Received().ProcessMessage(Arg.Is<IRoutedMessage>(x => x == message),
                Arg.Is<object>(x => x == parameter));
        }

        [Fact]
        public void can_update_availability()
        {
            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = message
            };

            node.UIElement.Returns(element);

            node.UIElement.Returns(element);

            trigger.Attach(node);
            trigger.UpdateAvailabilty(false);
            message.AvailabilityEffect.Received().ApplyTo(element, false);
        }

        [StaFact]
        public void represents_availability_consistently_through_ICommand_for_disable_availability_when_not_available()
        {
            message = new FakeMessage {AvailabilityEffect = AvailabilityEffect.Disable};

            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = message
            };

            node.UIElement.Returns(element);
            node.UIElement.Returns(element);

            trigger.Attach(node);
            trigger.UpdateAvailabilty(false);

            element.IsEnabled.ShouldBeFalse();
        }

        [Fact]
        public void represents_availability_consistently_through_ICommand_for_disable_availability_when_available()
        {
            message = new FakeMessage {AvailabilityEffect = AvailabilityEffect.Disable};

            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = message
            };

            node.UIElement.Returns(element);
            node.UIElement.Returns(element);

            trigger.Attach(node);
            trigger.UpdateAvailabilty(true);

            element.IsEnabled.ShouldBeTrue();
        }

        [Fact]
        public void
            represents_availability_consistently_through_ICommand_for_non_disable_availability_when_not_available()
        {
            message = new FakeMessage {AvailabilityEffect = AvailabilityEffect.Hide};

            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = message
            };

            node.UIElement.Returns(element);
            node.UIElement.Returns(element);

            trigger.Attach(node);
            trigger.UpdateAvailabilty(false);

            element.IsEnabled.ShouldBeTrue();
        }

        [Fact]
        public void represents_availability_consistently_through_ICommand_for_non_disable_availability_when_available()
        {
            message = new FakeMessage {AvailabilityEffect = AvailabilityEffect.Hide};

            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = message
            };

            node.UIElement.Returns(element);
            node.UIElement.Returns(element);

            trigger.Attach(node);
            trigger.UpdateAvailabilty(true);

            element.IsEnabled.ShouldBeTrue();
        }
    }
}
