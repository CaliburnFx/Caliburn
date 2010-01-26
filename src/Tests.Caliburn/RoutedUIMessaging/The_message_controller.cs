namespace Tests.Caliburn.RoutedUIMessaging
{
    using System.Linq;
    using System.Windows.Controls;
    using global::Caliburn.PresentationFramework;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class The_message_controller : TestBase
    {
        private IRoutedMessageController _controller;

        protected override void given_the_context_of()
        {
            _controller = new DefaultRoutedMessageController();
        }

        [Test]
        public void can_attach_a_trigger_to_a_ui_element()
        {
            var dp = new Button();
            var trigger = Mock<IMessageTrigger>();

            trigger.Expect(x => x.Attach(Arg<InteractionNode>.Is.NotNull));

            _controller.AttachTrigger(
                dp,
                trigger
                );

            var node = dp.GetValue(DefaultRoutedMessageController.NodeProperty) as InteractionNode;

            Assert.That(node, Is.Not.Null);
            Assert.That(node.Triggers.Contains(trigger));
        }

        [Test]
        public void can_attach_a_message_handler_to_a_ui_element()
        {
            var dp = new Button();
            var handler = Mock<IRoutedMessageHandler>();

            handler.Expect(x => x.Unwrap()).Return(handler);
            handler.Expect(x => x.Initialize(Arg<IInteractionNode>.Is.NotNull));

            _controller.AddHandler(
                dp,
                handler,
                true
                );

            var node = dp.GetValue(DefaultRoutedMessageController.NodeProperty) as InteractionNode;

            Assert.That(node, Is.Not.Null);
            Assert.That(node.MessageHandler, Is.EqualTo(handler));
            Assert.That(dp.DataContext, Is.EqualTo(handler));
        }

        [Test]
        public void can_attach_a_message_handler_to_a_ui_element_without_data_context()
        {
            var dp = new Button();
            var handler = Mock<IRoutedMessageHandler>();

            _controller.AddHandler(
                dp,
                handler,
                false
                );

            var node = dp.GetValue(DefaultRoutedMessageController.NodeProperty) as InteractionNode;

            Assert.That(node, Is.Not.Null);
            Assert.That(node.MessageHandler, Is.EqualTo(handler));
            Assert.That(dp.DataContext, Is.Null);
        }

        [Test]
        public void can_find_parent_node_of_ui_element()
        {
            var panel = new StackPanel();
            var button = new Button();

            panel.Children.Add(button);
            panel.SetValue(
                DefaultRoutedMessageController.NodeProperty,
                new InteractionNode(panel, _controller)
                );

            var parent = _controller.GetParent(button);

            Assert.That(parent, Is.Not.Null);
            Assert.That(parent.UIElement, Is.EqualTo(panel));
        }
    }
}