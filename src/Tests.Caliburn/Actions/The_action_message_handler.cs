using System.Collections.Generic;
using Caliburn.Core;
using Xunit;
using NSubstitute;
using Shouldly;
using Tests.Caliburn.Fakes;

namespace Tests.Caliburn.Actions
{
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.Filters;
    using global::Caliburn.PresentationFramework.RoutedMessaging;


    public class The_action_message_handler : TestBase
    {
        const string MethodName = "MyAction";

        object target;
        IActionHost host;
        ActionMessageHandler handler;

        protected override void given_the_context_of()
        {
            target = new object();
            host = Mock<IActionHost>();

            var filterManager = Mock<IFilterManager>();
            filterManager.HandlerAware
                .Returns(new IHandlerAware[] { });

            host.Filters
                .Returns(filterManager);

            host.Actions
                .Returns(new List<IAction>());

            handler = new ActionMessageHandler(host, target);
        }

        [Fact]
        public void can_get_default_data_context_value()
        {
            handler.Unwrap().ShouldBe(target);
        }

        [Fact]
        public void can_handle_an_action_message_with_approriate_name()
        {
            var message = new ActionMessage {MethodName = MethodName};

            host.GetAction(message).Returns(Mock<IAction>());

            var result = handler.Handles(message);
            result.ShouldBeTrue();
        }

        [Fact]
        public void cannot_handle_an_action_message_with_invalid_name()
        {
            var message = new ActionMessage {MethodName = MethodName};

            host.GetAction(message).Returns(null as IAction);

            var result = handler.Handles(message);
            result.ShouldBeFalse();
        }

        [Fact]
        public void cannot_handle_a_non_action_message()
        {
            var result = handler.Handles(new FakeMessage());

            result.ShouldBeFalse();
        }

        [Fact]
        public void can_process_an_action_message()
        {
            var sourceNode = Mock<IInteractionNode>();
            var context = new object();

            var message = new ActionMessage {MethodName = MethodName};
            message.Initialize(sourceNode);

            var action = Mock<IAction>();

            host.GetAction(message).Returns(action);
            handler.Process(message, context);

            action.Received().Execute(message, null, context);
        }

        [Fact]
        public void cannot_process_a_non_action_message()
        {
            Assert.Throws<CaliburnException>(() =>{
                var context = new object();
                var message = new FakeMessage();

                handler.Process(message, context);
            });
        }

        [Fact]
        public void can_update_a_trigger_if_has_trigger_effects_and_true()
        {
            var node = Mock<IInteractionNode>();
            var message = new ActionMessage { MethodName = MethodName };
            var action = Mock<IAction>();
            var trigger = Mock<IMessageTrigger>();

            var filters = Mock<IFilterManager>();
            filters.TriggerEffects.Returns(new[] {Mock<IPreProcessor>()});
            action.Filters.Returns(filters);
            trigger.Message.Returns(message);

            host.GetAction(message).Returns(action);

            action.ShouldTriggerBeAvailable(message, node).ReturnsForAnyArgs(true);

            handler.UpdateAvailability(trigger);

            trigger.Received().UpdateAvailabilty(true);
        }

        [Fact]
        public void can_update_a_trigger_if_has_trigger_effects_and_false()
        {
            var node = Mock<IInteractionNode>();
            var message = new ActionMessage { MethodName = MethodName };
            var action = Mock<IAction>();
            var trigger = Mock<IMessageTrigger>();
            var filters = Mock<IFilterManager>();
            filters.TriggerEffects.Returns(new[] { Mock<IPreProcessor>() });
            action.Filters.Returns(filters);
            trigger.Message.Returns(message);

            host.GetAction(message).Returns(action);

            action.ShouldTriggerBeAvailable(message, node).Returns(false);

            handler.UpdateAvailability(trigger);
            trigger.Received().UpdateAvailabilty(false);
        }

        [Fact]
        public void cannot_update_a_trigger_if_missing_trigger_effects()
        {
            var message = new ActionMessage { MethodName = MethodName };
            var action = Mock<IAction>();
            var trigger = Mock<IMessageTrigger>();
            var filters = Mock<IFilterManager>();
            filters.TriggerEffects.Returns(new IPreProcessor[]{});
            action.Filters.Returns(filters);
            trigger.Message.Returns(message);

            host.GetAction(message).Returns(action);

            handler.UpdateAvailability(trigger);
        }

        [Fact]
        public void cannot_update_trigger_for_a_non_action_message()
        {
            Assert.Throws<CaliburnException>(() =>{
                var message = new FakeMessage();
                var trigger = Mock<IMessageTrigger>();
                trigger.Message.Returns(message);

                handler.UpdateAvailability(trigger);
            });
        }
    }
}
