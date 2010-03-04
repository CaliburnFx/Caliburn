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
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class The_gesture_message_trigger : TestBase
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
        public void can_attach_itself_to_an_element_using_a_mouse_gesture()
        {
            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Control,
                MouseAction = MouseAction.LeftDoubleClick,
                Message = _message
            };

            _node.Expect(x => x.UIElement).Return(_element);


            trigger.Attach(_node);

            Assert.That(trigger.Node, Is.EqualTo(_node));
            Assert.That(_message.InvalidatedHandler, Is.Not.Null);
            Assert.That(_message.InitializeCalledWith, Is.EqualTo(_node));

            var binding = _element.InputBindings[0];
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
                Message = _message
            };

            _node.Expect(x => x.UIElement).Return(_element);


            trigger.Attach(_node);

            Assert.That(trigger.Node, Is.EqualTo(_node));
            Assert.That(_message.InvalidatedHandler, Is.Not.Null);
            Assert.That(_message.InitializeCalledWith, Is.EqualTo(_node));

            var binding = _element.InputBindings[0];
            var gesture = binding.Gesture as UnrestrictedKeyGesture;

            Assert.That(binding.Command, Is.EqualTo(new GestureMessageTrigger.GestureCommand(binding.Gesture)));
            Assert.That(gesture.Key, Is.EqualTo(Key.S));
            Assert.That(gesture.Modifiers, Is.EqualTo(ModifierKeys.Alt));
        }

        [Test]
        [ExpectedException(typeof(CaliburnException))]
        public void throws_exception_if_attempt_to_attach_to_non_UIElement()
        {
            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = _message
            };

            _node.Expect(x => x.UIElement).Return(new DependencyObject()).Repeat.Twice();


            trigger.Attach(_node);
        }

        [Test]
        public void can_trigger_message_processing()
        {
            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = _message
            };

            object parameter = new object();

            _node.Expect(x => x.UIElement).Return(_element);
            _node.Expect(x => x.ProcessMessage(Arg<IRoutedMessage>.Is.Equal(_message), Arg<object>.Is.Equal(parameter)));


            trigger.Attach(_node);
            _element.InputBindings[0].Command.Execute(parameter);
        }

        [Test]
        public void can_update_availability()
        {
            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = _message
            };

            _node.Expect(x => x.UIElement).Return(_element);

            _node.Expect(x => x.UIElement).Return(_element);
            _message.AvailabilityEffect.Expect(x => x.ApplyTo(_element, false));


            trigger.Attach(_node);
            trigger.UpdateAvailabilty(false);
        }

        [Test]
        public void represents_availability_consistently_through_ICommand_for_disable_availability_when_not_available()
        {
            _message = new FakeMessage {AvailabilityEffect = AvailabilityEffect.Disable};

            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = _message
            };

            _node.Expect(x => x.UIElement).Return(_element);
            _node.Expect(x => x.UIElement).Return(_element);


            trigger.Attach(_node);
            trigger.UpdateAvailabilty(false);

            Assert.That(_element.IsEnabled, Is.False);
        }

        [Test]
        public void represents_availability_consistently_through_ICommand_for_disable_availability_when_available()
        {
            _message = new FakeMessage {AvailabilityEffect = AvailabilityEffect.Disable};

            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = _message
            };

            _node.Expect(x => x.UIElement).Return(_element);
            _node.Expect(x => x.UIElement).Return(_element);


            trigger.Attach(_node);
            trigger.UpdateAvailabilty(true);

            Assert.That(_element.IsEnabled);
        }

        [Test]
        public void
            represents_availability_consistently_through_ICommand_for_non_disable_availability_when_not_available()
        {
            _message = new FakeMessage {AvailabilityEffect = AvailabilityEffect.Hide};

            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = _message
            };

            _node.Expect(x => x.UIElement).Return(_element);
            _node.Expect(x => x.UIElement).Return(_element);


            trigger.Attach(_node);
            trigger.UpdateAvailabilty(false);

            Assert.That(_element.IsEnabled);
        }

        [Test]
        public void represents_availability_consistently_through_ICommand_for_non_disable_availability_when_available()
        {
            _message = new FakeMessage {AvailabilityEffect = AvailabilityEffect.Hide};

            var trigger = new GestureMessageTrigger
            {
                Modifiers = ModifierKeys.Alt,
                Key = Key.S,
                Message = _message
            };

            _node.Expect(x => x.UIElement).Return(_element);
            _node.Expect(x => x.UIElement).Return(_element);


            trigger.Attach(_node);
            trigger.UpdateAvailabilty(true);

            Assert.That(_element.IsEnabled);
        }
    }
}