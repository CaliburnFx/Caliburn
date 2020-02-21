using Shouldly;

namespace Tests.Caliburn.RoutedUIMessaging
{
    using System.Linq;
    using System.Windows.Controls;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using Xunit;
    using NSubstitute;


    public class The_message_controller : TestBase
    {
        IRoutedMessageController controller;

        protected override void given_the_context_of()
        {
            controller = new DefaultRoutedMessageController();
        }

        [StaFact]
        public void can_attach_a_trigger_to_a_ui_element()
        {
            var dp = new Button();
            var trigger = Mock<IMessageTrigger>();

            controller.AttachTrigger(
                dp,
                trigger
                );

            var node = dp.GetValue(DefaultRoutedMessageController.NodeProperty) as InteractionNode;

            node.ShouldNotBeNull();
            node.Triggers.Contains(trigger).ShouldBeTrue();

            trigger.Received().Attach(Arg.Is<InteractionNode>(x => x != null));
        }

        [StaFact]
        public void can_attach_a_message_handler_to_a_ui_element()
        {
            var dp = new Button();
            var handler = Mock<IRoutedMessageHandler>();

            handler.Unwrap().Returns(handler);
            controller.AddHandler(
                dp,
                handler,
                true
                );

            var node = dp.GetValue(DefaultRoutedMessageController.NodeProperty) as InteractionNode;

            node.ShouldNotBeNull();
            node.MessageHandler.ShouldBe(handler);
            dp.DataContext.ShouldBe(handler);
            handler.Received().Initialize(Arg.Is<IInteractionNode>(x => x != null));
        }

        [StaFact]
        public void can_attach_a_message_handler_to_a_ui_element_without_data_context()
        {
            var dp = new Button();
            var handler = Mock<IRoutedMessageHandler>();

            controller.AddHandler(
                dp,
                handler,
                false
                );

            var node = dp.GetValue(DefaultRoutedMessageController.NodeProperty) as InteractionNode;

            node.ShouldNotBeNull();
            node.MessageHandler.ShouldBe(handler);
            dp.DataContext.ShouldBeNull();
        }

        [StaFact]
        public void can_find_parent_node_of_ui_element()
        {
            var panel = new StackPanel();
            var button = new Button();

            panel.Children.Add(button);
            panel.SetValue(
                DefaultRoutedMessageController.NodeProperty,
                new InteractionNode(panel, controller)
                );

            var parent = controller.GetParent(button);

            parent.ShouldNotBeNull();
            parent.UIElement.ShouldBe(panel);
        }
    }
}
