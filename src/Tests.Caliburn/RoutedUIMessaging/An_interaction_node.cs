using System.Reflection;
using System.Windows;
using Shouldly;

namespace Tests.Caliburn.RoutedUIMessaging
{
    using System.Linq;
    using System.Windows.Controls;
    using Fakes;
    using global::Caliburn.Core;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using Xunit;
    using NSubstitute;

    public class An_interaction_node : TestBase
    {
        private IRoutedMessageController controller;
        private InteractionNode node;
        private Button element;
        private StackPanel parent;
        private InteractionNode parentNode;

        protected override void given_the_context_of()
        {
            controller = Mock<IRoutedMessageController>();
            parent = new StackPanel();
            element = new Button();
            element.Focus();
            parentNode = new InteractionNode(parent, controller);
            node = new InteractionNode(element, controller);

            parent.Children.Add(element);
        }

        [StaFact]
        public void declares_the_ui_element()
        {
            node.UIElement.ShouldBe(element);
        }

        [StaFact]
        public void can_find_parent_node()
        {
            controller.GetParent(element).Returns(parentNode);

            var found = node.FindParent();

            found.ShouldBe(parentNode);
        }

        [StaFact]
        public void can_add_a_trigger()
        {
            var trigger = Mock<IMessageTrigger>();

            node.AddTrigger(trigger);

            trigger.Received().Attach(Arg.Is<InteractionNode>(x => x == node));

            node.Triggers.Contains(trigger).ShouldBeTrue();
        }

        [StaFact]
        public void can_have_a_message_handler()
        {
            var handler = Mock<IRoutedMessageHandler>();

            node.RegisterHandler(handler);

            node.MessageHandler.ShouldBe(handler);
        }

        [StaFact]
        public void can_determine_if_a_message_is_handled()
        {
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();

            handler.Handles(message).Returns(true);

            node.RegisterHandler(handler);

            bool result = node.Handles(message);

            handler.Received().Initialize(node);
            result.ShouldBeTrue();
        }

        [StaFact]
        public void can_process_message_if_node_has_handler()
        {
            var context = new object();
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();

            handler.Handles(message).Returns(true);

            node.RegisterHandler(handler);
            node.ProcessMessage(message, context);
            handler.Received().Initialize(node);
            handler.Received().Process(message, context);
        }

        [StaFact]
        public void can_process_message_if_parent_node_has_handler()
        {
            var context = new object();
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();
            var parentHandler = Mock<IRoutedMessageHandler>();

            node.RegisterHandler(handler);
            parentNode.RegisterHandler(parentHandler);

            handler.Handles(message).Returns(false);
            controller.GetParent(element).Returns(parentNode);
            parentHandler.Handles(message).Returns(true);

            node.ProcessMessage(message, context);

            parentHandler.Received().Process(message, context);
        }

        [StaFact]
        public void will_throw_exception_if_processing_node_is_not_found()
        {
            Assert.Throws<CaliburnException>(() =>{
                var context = new object();
                var message = new FakeMessage();
                var handler = Mock<IRoutedMessageHandler>();

                node.RegisterHandler(handler);

                handler.Handles(message).Returns(false);
                controller.GetParent(element).Returns(null as IInteractionNode);

                node.ProcessMessage(message, context);
            });
        }

        //Due to an issue with STA Thread differences on dotnetcoreapp we'll comment out until
        //https://github.com/AArnott/Xunit.StaFact/issues/35 is resolved.
        #if NET462
        [StaFact]
        #endif
        public void can_update_availability_if_node_has_handler()
        {
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();
            var trigger = Mock<IMessageTrigger>();

            trigger.Message.Returns(message);
            handler.Handles(message).Returns(true);

            node.RegisterHandler(handler);
            node.UpdateAvailability(trigger);

            handler.Received().Initialize(node);
            handler.Received().UpdateAvailability(trigger);
        }

        #if NET462
        [StaFact]
        #endif
        public void can_update_availability_if_parent_node_has_handler()
        {
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();
            var parentHandler = Mock<IRoutedMessageHandler>();
            var trigger = Mock<IMessageTrigger>();

            node.RegisterHandler(handler);
            parentNode.RegisterHandler(parentHandler);

            trigger.Message.Returns(message);
            handler.Handles(message).Returns(false);
            controller.GetParent(element).Returns(parentNode);
            parentHandler.Handles(message).Returns(true);

            node.UpdateAvailability(trigger);
            parentHandler.Received().UpdateAvailability(trigger);
        }

        [StaFact]
        public void will_throw_exception_if_trigger_update_node_is_not_found()
        {
            RaiseLoadedEvent(element);
            Assert.Throws<CaliburnException>(() =>{
                var trigger = Mock<IMessageTrigger>();
                var message = new FakeMessage();
                var handler = Mock<IRoutedMessageHandler>();

                node.RegisterHandler(handler);

                trigger.Message.Returns(message);
                handler.Handles(message).Returns(false);
                controller.GetParent(element).Returns(null as IInteractionNode);

                node.UpdateAvailability(trigger);
            });
        }

        private static void RaiseLoadedEvent(FrameworkElement element)
        {
            var eventMethod = typeof(FrameworkElement).GetMethod("OnLoaded", BindingFlags.Instance | BindingFlags.NonPublic);
            var args = new RoutedEventArgs(FrameworkElement.LoadedEvent);
            eventMethod.Invoke(element, new object[] { args });
        }
    }
}
