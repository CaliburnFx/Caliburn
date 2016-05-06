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
    using Rhino.Mocks;

    
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

        [WpfFact]
        public void declares_the_ui_element()
        {
            node.UIElement.ShouldBe(element);
        }

        [WpfFact]
        public void can_find_parent_node()
        {
            controller.Expect(x => x.GetParent(element)).Return(parentNode);

            var found = node.FindParent();

            found.ShouldBe(parentNode);
        }

        [WpfFact]
        public void can_add_a_trigger()
        {
            var trigger = Mock<IMessageTrigger>();

            trigger.Expect(x => x.Attach(Arg<InteractionNode>.Is.Equal(node)));

            node.AddTrigger(trigger);

            node.Triggers.Contains(trigger).ShouldBeTrue();
        }

        [WpfFact]
        public void can_have_a_message_handler()
        {
            var handler = Mock<IRoutedMessageHandler>();

            node.RegisterHandler(handler);

            node.MessageHandler.ShouldBe(handler);
        }

        [WpfFact]
        public void can_determine_if_a_message_is_handled()
        {
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();

            handler.Expect(x => x.Initialize(node));
            handler.Expect(x => x.Handles(message)).Return(true);

            node.RegisterHandler(handler);

            bool result = node.Handles(message);

            result.ShouldBeTrue();
        }

        [WpfFact]
        public void can_process_message_if_node_has_handler()
        {
            var context = new object();
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();

            handler.Expect(x => x.Initialize(node));
            handler.Expect(x => x.Handles(message)).Return(true);
            handler.Expect(x => x.Process(message, context));

            node.RegisterHandler(handler);
            node.ProcessMessage(message, context);
        }

        [WpfFact]
        public void can_process_message_if_parent_node_has_handler()
        {
            var context = new object();
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();
            var parentHandler = Mock<IRoutedMessageHandler>();

            node.RegisterHandler(handler);
            parentNode.RegisterHandler(parentHandler);

            handler.Expect(x => x.Handles(message)).Return(false);
            controller.Expect(x => x.GetParent(element)).Return(parentNode);
            parentHandler.Expect(x => x.Handles(message)).Return(true);

            parentHandler.Expect(x => x.Process(message, context));

            node.ProcessMessage(message, context);
        }

        [WpfFact]
        public void will_throw_exception_if_processing_node_is_not_found()
        {
            Assert.Throws<CaliburnException>(() =>{
                var context = new object();
                var message = new FakeMessage();
                var handler = Mock<IRoutedMessageHandler>();

                node.RegisterHandler(handler);

                handler.Expect(x => x.Handles(message)).Return(false);
                controller.Expect(x => x.GetParent(element)).Return(null);

                node.ProcessMessage(message, context);
            });
        }

        [WpfFact]
        public void can_update_availability_if_node_has_handler()
        {
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();
            var trigger = Mock<IMessageTrigger>();

            handler.Expect(x => x.Initialize(node));
            trigger.Expect(x => x.Message).Return(message);
            handler.Expect(x => x.Handles(message)).Return(true);
            handler.Expect(x => x.UpdateAvailability(trigger));

            node.RegisterHandler(handler);

            node.UpdateAvailability(trigger);
        }

        [WpfFact]
        public void can_update_availability_if_parent_node_has_handler()
        {
            var message = new FakeMessage();
            var handler = Mock<IRoutedMessageHandler>();
            var parentHandler = Mock<IRoutedMessageHandler>();
            var trigger = Mock<IMessageTrigger>();

            node.RegisterHandler(handler);
            parentNode.RegisterHandler(parentHandler);

            trigger.Expect(x => x.Message).Return(message);
            handler.Expect(x => x.Handles(message)).Return(false);
            controller.Expect(x => x.GetParent(element)).Return(parentNode);
            parentHandler.Expect(x => x.Handles(message)).Return(true);
            parentHandler.Expect(x => x.UpdateAvailability(trigger));

            node.UpdateAvailability(trigger);
        }

        [WpfFact]
        public void will_throw_exception_if_trigger_update_node_is_not_found()
        {
            RaiseLoadedEvent(element);
            Assert.Throws<CaliburnException>(() =>{
                var trigger = Mock<IMessageTrigger>();
                var message = new FakeMessage();
                var handler = Mock<IRoutedMessageHandler>();

                node.RegisterHandler(handler);

                trigger.Expect(x => x.Message).Return(message);
                handler.Expect(x => x.Handles(message)).Return(false);
                controller.Expect(x => x.GetParent(element)).Return(null);

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