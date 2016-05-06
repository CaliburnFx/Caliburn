using System.Collections.Generic;
using Caliburn.Core;
using Xunit;
using Rhino.Mocks;
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

            var filterManager = Stub<IFilterManager>();
            filterManager.Stub(x => x.HandlerAware)
                .Return(new IHandlerAware[]{})
                .Repeat.Any();

            host.Stub(x => x.Filters)
                .Return(filterManager)
                .Repeat.Any();

            host.Stub(x => x.Actions)
                .Return(new List<IAction>())
                .Repeat.Any();

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

            host.Expect(x => x.GetAction(message)).Return(Stub<IAction>());

            var result = handler.Handles(message);
            result.ShouldBeTrue();
        }

        [Fact]
        public void cannot_handle_an_action_message_with_invalid_name()
        {
            var message = new ActionMessage {MethodName = MethodName};

            host.Expect(x => x.GetAction(message)).Return(null);

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
            var handlingNode = Stub<IInteractionNode>();
            var sourceNode = Stub<IInteractionNode>();
            var context = new object();

            var message = new ActionMessage {MethodName = MethodName};
            message.Initialize(sourceNode);

            var action = Mock<IAction>();

            host.Expect(x => x.GetAction(message)).Return(action);
            action.Expect(x => x.Execute(message, handlingNode, context));

            handler.Process(message, context);
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
            var node = Stub<IInteractionNode>();
            var message = new ActionMessage { MethodName = MethodName };
            var action = Mock<IAction>();
            var trigger = Mock<IMessageTrigger>();

            var filters = Stub<IFilterManager>();
            filters.Stub(x => x.TriggerEffects).Return(new[] {Stub<IPreProcessor>()}).Repeat.Any();
            action.Stub(x => x.Filters).Return(filters).Repeat.Any();
            trigger.Stub(x => x.Message).Return(message);

            host.Expect(x => x.GetAction(message)).Return(action);

            action.Expect(x => x.ShouldTriggerBeAvailable(message, node)).Return(true);

            trigger.Expect(x => x.UpdateAvailabilty(true));

            handler.UpdateAvailability(trigger);
        }

        [Fact]
        public void can_update_a_trigger_if_has_trigger_effects_and_false()
        {
            var node = Stub<IInteractionNode>();
            var message = new ActionMessage { MethodName = MethodName };
            var action = Mock<IAction>();
            var trigger = Mock<IMessageTrigger>();
            var filters = Stub<IFilterManager>();
            filters.Stub(x => x.TriggerEffects).Return(new[] { Stub<IPreProcessor>() }).Repeat.Any();
            action.Stub(x => x.Filters).Return(filters).Repeat.Any();
            trigger.Stub(x => x.Message).Return(message);

            host.Expect(x => x.GetAction(message)).Return(action);

            action.Expect(x => x.ShouldTriggerBeAvailable(message, node)).Return(false);

            trigger.Expect(x => x.UpdateAvailabilty(false));


            handler.UpdateAvailability(trigger);
        }

        [Fact]
        public void cannot_update_a_trigger_if_missing_trigger_effects()
        {
            var message = new ActionMessage { MethodName = MethodName };
            var action = Mock<IAction>();
            var trigger = Mock<IMessageTrigger>();
            var filters = Stub<IFilterManager>();
            filters.Stub(x => x.TriggerEffects).Return(new IPreProcessor[]{}).Repeat.Any();
            action.Stub(x => x.Filters).Return(filters).Repeat.Any();
            trigger.Stub(x => x.Message).Return(message);

            host.Expect(x => x.GetAction(message)).Return(action);

            handler.UpdateAvailability(trigger);
        }

        [Fact]
        public void cannot_update_trigger_for_a_non_action_message()
        {
            Assert.Throws<CaliburnException>(() =>{
                var message = new FakeMessage();
                var trigger = Mock<IMessageTrigger>();
                trigger.Stub(x => x.Message).Return(message);

                handler.UpdateAvailability(trigger);
            });
        }
    }
}