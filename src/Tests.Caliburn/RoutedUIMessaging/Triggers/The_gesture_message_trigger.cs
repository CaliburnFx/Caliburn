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
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
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

        [Test]
        public void can_attach_itself_to_an_element_using_a_mouse_gesture()
        {
            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Control,
                MouseAction = MouseAction.LeftDoubleClick,
                Message = message
            };

            node.Expect(x => x.UIElement).Return(element);


            trigger.Attach(node);

            Assert.That(trigger.Node, Is.EqualTo(node));
            Assert.That(message.InvalidatedHandler, Is.Not.Null);
            Assert.That(message.InitializeCalledWith, Is.EqualTo(node));

            var binding = element.InputBindings[0];
            var gesture = binding.Gesture as MouseGesture;

            Assert.That(binding.Command, Is.EqualTo(new GestureMessageTrigger.GestureCommand(binding.Gesture)));
            Assert.That(gesture.MouseAction, Is.EqualTo(MouseAction.LeftDoubleClick));
            Assert.That(gesture.Modifiers, Is.EqualTo(ModifierKeys.Control));
        }

        [Test]
        public void can_attach_itself_to_an_element_using_a_key_gesture()
        {
            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = message
            };

            node.Expect(x => x.UIElement).Return(element);


            trigger.Attach(node);

            Assert.That(trigger.Node, Is.EqualTo(node));
            Assert.That(message.InvalidatedHandler, Is.Not.Null);
            Assert.That(message.InitializeCalledWith, Is.EqualTo(node));

            var binding = element.InputBindings[0];
            var gesture = binding.Gesture as UnrestrictedKeyGesture;

            Assert.That(binding.Command, Is.EqualTo(new GestureMessageTrigger.GestureCommand(binding.Gesture)));
            Assert.That(gesture.Key, Is.EqualTo(Key.S));
            Assert.That(gesture.Modifiers, Is.EqualTo(ModifierKeys.Alt));
        }

        [Test]
        public void throws_exception_if_attempt_to_attach_to_non_UIElement()
        {
            Assert.Throws<CaliburnException>(() =>{
                var trigger = new GestureMessageTrigger {
                    Modifiers = ModifierKeys.Alt,
                    Key = Key.S,
                    Message = message
                };

                node.Expect(x => x.UIElement).Return(new DependencyObject()).Repeat.Twice();


                trigger.Attach(node);
            });
        }

        [Test]
        public void can_trigger_message_processing()
        {
            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = message
            };

            object parameter = new object();

            node.Expect(x => x.UIElement).Return(element);
            node.Expect(x => x.ProcessMessage(Arg<IRoutedMessage>.Is.Equal(message), Arg<object>.Is.Equal(parameter)));


            trigger.Attach(node);
            element.InputBindings[0].Command.Execute(parameter);
        }

        [Test]
        public void can_update_availability()
        {
            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = message
            };

            node.Expect(x => x.UIElement).Return(element);

            node.Expect(x => x.UIElement).Return(element);
            message.AvailabilityEffect.Expect(x => x.ApplyTo(element, false));


            trigger.Attach(node);
            trigger.UpdateAvailabilty(false);
        }

        [Test]
        public void represents_availability_consistently_through_ICommand_for_disable_availability_when_not_available()
        {
            message = new FakeMessage {AvailabilityEffect = AvailabilityEffect.Disable};

            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = message
            };

            node.Expect(x => x.UIElement).Return(element);
            node.Expect(x => x.UIElement).Return(element);


            trigger.Attach(node);
            trigger.UpdateAvailabilty(false);

            Assert.That(element.IsEnabled, Is.False);
        }

        [Test]
        public void represents_availability_consistently_through_ICommand_for_disable_availability_when_available()
        {
            message = new FakeMessage {AvailabilityEffect = AvailabilityEffect.Disable};

            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = message
            };

            node.Expect(x => x.UIElement).Return(element);
            node.Expect(x => x.UIElement).Return(element);


            trigger.Attach(node);
            trigger.UpdateAvailabilty(true);

            Assert.That(element.IsEnabled);
        }

        [Test]
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

            node.Expect(x => x.UIElement).Return(element);
            node.Expect(x => x.UIElement).Return(element);


            trigger.Attach(node);
            trigger.UpdateAvailabilty(false);

            Assert.That(element.IsEnabled);
        }

        [Test]
        public void represents_availability_consistently_through_ICommand_for_non_disable_availability_when_available()
        {
            message = new FakeMessage {AvailabilityEffect = AvailabilityEffect.Hide};

            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = message
            };

            node.Expect(x => x.UIElement).Return(element);
            node.Expect(x => x.UIElement).Return(element);


            trigger.Attach(node);
            trigger.UpdateAvailabilty(true);

            Assert.That(element.IsEnabled);
        }
    }
}