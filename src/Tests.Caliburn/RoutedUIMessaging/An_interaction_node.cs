namespace Tests.Caliburn.RoutedUIMessaging
{
    using System.Linq;
    using System.Windows.Controls;
    using Fakes;
    using global::Caliburn.Core;
    using global::Caliburn.PresentationFramework;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class An_interaction_node : TestBase
    {
        private IRoutedMessageController _controller;
        private InteractionNode _node;
        private Button _element;
        private StackPanel _parent;
        private InteractionNode _parentNode;

        protected override void given_the_context_of()
        {
            _controller = Mock<IRoutedMessageController>();
            _parent = new StackPanel();
            _element = new Button();
            _parentNode = new InteractionNode(_parent, _controller);
            _node = new InteractionNode(_element, _controller);

            _parent.Children.Add(_element);
        }

        [Test]
        public void declares_the_ui_element()
        {
            Assert.That(_node.UIElement, Is.EqualTo(_element));
        }

        [Test]
        public void can_find_parent_node()
        {
            _controller.Expect(x => x.GetParent(_element)).Return(_parentNode);

            var found = _node.FindParent();

            Assert.That(found, Is.EqualTo(_parentNode));
        }

        [Test]
        public void can_add_a_trigger()
        {
            var trigger = Mock<IMessageTrigger>();

            trigger.Expect(x => x.Attach(Arg<InteractionNode>.Is.Equal(_node)));

            _node.AddTrigger(trigger);

            Assert.That(_node.Triggers.Contains(trigger));
        }

        [Test]
        public void can_have_a_message_handler()
        {
            var handler = Mock<IRoutedMessageHandler>();

            _node.RegisterHandler(handler);

            Assert.That(_node.MessageHandler, Is.EqualTo(handler));
        }

        [Test]
        public void can_determine_if_a_message_is_handled()
        {
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();

            handler.Expect(x => x.Initialize(_node));
            handler.Expect(x => x.Handles(message)).Return(true);

            _node.RegisterHandler(handler);

            bool result = _node.Handles(message);

            Assert.That(result, Is.True);
        }

        [Test]
        public void can_process_message_if_node_has_handler()
        {
            var context = new object();
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();

            handler.Expect(x => x.Initialize(_node));
            handler.Expect(x => x.Handles(message)).Return(true);
            handler.Expect(x => x.Process(message, context));

            _node.RegisterHandler(handler);
            _node.ProcessMessage(message, context);
        }

        [Test]
        public void can_process_message_if_parent_node_has_handler()
        {
            var context = new object();
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();
            var parentHandler = Mock<IRoutedMessageHandler>();

            _node.RegisterHandler(handler);
            _parentNode.RegisterHandler(parentHandler);

            handler.Expect(x => x.Handles(message)).Return(false);
            _controller.Expect(x => x.GetParent(_element)).Return(_parentNode);
            parentHandler.Expect(x => x.Handles(message)).Return(true);

            parentHandler.Expect(x => x.Process(message, context));

            _node.ProcessMessage(message, context);
        }

        [Test]
        [ExpectedException(typeof(CaliburnException))]
        public void will_throw_exception_if_processing_node_is_not_found()
        {
            var context = new object();
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();

            _node.RegisterHandler(handler);

            handler.Expect(x => x.Handles(message)).Return(false);
            _controller.Expect(x => x.GetParent(_element)).Return(null);

            _node.ProcessMessage(message, context);
        }

        [Test]
        public void can_update_availability_if_node_has_handler()
        {
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();
            var trigger = Mock<IMessageTrigger>();

            handler.Expect(x => x.Initialize(_node));
            trigger.Expect(x => x.Message).Return(message);
            handler.Expect(x => x.Handles(message)).Return(true);
            handler.Expect(x => x.UpdateAvailability(trigger));

            _node.RegisterHandler(handler);

            _node.UpdateAvailability(trigger);
        }

        [Test]
        public void can_update_availability_if_parent_node_has_handler()
        {
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();
            var parentHandler = Mock<IRoutedMessageHandler>();
            var trigger = Mock<IMessageTrigger>();

            _node.RegisterHandler(handler);
            _parentNode.RegisterHandler(parentHandler);

            trigger.Expect(x => x.Message).Return(message);
            handler.Expect(x => x.Handles(message)).Return(false);
            _controller.Expect(x => x.GetParent(_element)).Return(_parentNode);
            parentHandler.Expect(x => x.Handles(message)).Return(true);
            parentHandler.Expect(x => x.UpdateAvailability(trigger));

            _node.UpdateAvailability(trigger);
        }

        [Test]
        [ExpectedException(typeof(CaliburnException))]
        [Ignore]
        public void will_throw_exception_if_trigger_update_node_is_not_found()
        {
            var trigger = Mock<IMessageTrigger>();
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();

            _node.RegisterHandler(handler);

            trigger.Expect(x => x.Message).Return(message);
            handler.Expect(x => x.Handles(message)).Return(false);
            _controller.Expect(x => x.GetParent(_element)).Return(null);

            _node.UpdateAvailability(trigger);
        }
    }
}