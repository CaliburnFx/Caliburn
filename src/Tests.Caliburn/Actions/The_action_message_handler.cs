using System.Collections.Generic;
using Caliburn.Core;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Tests.Caliburn.Fakes;

namespace Tests.Caliburn.Actions
{
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.Filters;

    [TestFixture]
    public class The_action_message_handler : TestBase
    {
        private const string _methodName = "MyAction";

        private object _target;
        private IActionHost _host;
        private ActionMessageHandler _handler;

        protected override void given_the_context_of()
        {
            _target = new object();
            _host = Mock<IActionHost>();

            var filterManager = Stub<IFilterManager>();
            filterManager.Stub(x => x.HandlerAware)
                .Return(new IHandlerAware[]{})
                .Repeat.Any();

            _host.Stub(x => x.Filters)
                .Return(filterManager)
                .Repeat.Any();

            _host.Stub(x => x.Actions)
                .Return(new List<IAction>())
                .Repeat.Any();

            _handler = new ActionMessageHandler(_host, _target);
        }

        [Test]
        public void can_get_default_data_context_value()
        {
            Assert.That(_handler.Unwrap(), Is.EqualTo(_target));
        }

        [Test]
        public void can_handle_an_action_message_with_approriate_name()
        {
            var message = new ActionMessage {MethodName = _methodName};

            _host.Expect(x => x.GetAction(message)).Return(Stub<IAction>());

            var result = _handler.Handles(message);
            Assert.That(result, Is.True);
        }

        [Test]
        public void cannot_handle_an_action_message_with_invalid_name()
        {
            var message = new ActionMessage {MethodName = _methodName};

            _host.Expect(x => x.GetAction(message)).Return(null);

            var result = _handler.Handles(message);
            Assert.That(result, Is.False);
        }

        [Test]
        public void cannot_handle_a_non_action_message()
        {
            var result = _handler.Handles(new FakeMessage());

            Assert.That(result, Is.False);
        }

        [Test]
        public void can_process_an_action_message()
        {
            var handlingNode = Stub<IInteractionNode>();
            var sourceNode = Stub<IInteractionNode>();
            var context = new object();

            var message = new ActionMessage {MethodName = _methodName};
            message.Initialize(sourceNode);

            var action = Mock<IAction>();

            _host.Expect(x => x.GetAction(message)).Return(action);
            action.Expect(x => x.Execute(message, handlingNode, context));

            _handler.Process(message, context);
        }

        [Test]
        [ExpectedException(typeof(CaliburnException))]
        public void cannot_process_a_non_action_message()
        {
            var node = Stub<IInteractionNode>();
            var context = new object();
            var message = new FakeMessage();

            _handler.Process(message, context);
        }

        [Test]
        public void can_update_a_trigger_if_has_trigger_effects_and_true()
        {
            var node = Stub<IInteractionNode>();
            var message = new ActionMessage { MethodName = _methodName };
            var action = Mock<IAction>();
            var trigger = Mock<IMessageTrigger>();

            var filters = Stub<IFilterManager>();
            filters.Stub(x => x.TriggerEffects).Return(new[] {Stub<IPreProcessor>()}).Repeat.Any();
            action.Stub(x => x.Filters).Return(filters).Repeat.Any();
            trigger.Stub(x => x.Message).Return(message);

            _host.Expect(x => x.GetAction(message)).Return(action);

            action.Expect(x => x.ShouldTriggerBeAvailable(message, node)).Return(true);

            trigger.Expect(x => x.UpdateAvailabilty(true));

            _handler.UpdateAvailability(trigger);
        }

        [Test]
        public void can_update_a_trigger_if_has_trigger_effects_and_false()
        {
            var node = Stub<IInteractionNode>();
            var message = new ActionMessage { MethodName = _methodName };
            var action = Mock<IAction>();
            var trigger = Mock<IMessageTrigger>();
            var filters = Stub<IFilterManager>();
            filters.Stub(x => x.TriggerEffects).Return(new[] { Stub<IPreProcessor>() }).Repeat.Any();
            action.Stub(x => x.Filters).Return(filters).Repeat.Any();
            trigger.Stub(x => x.Message).Return(message);

            _host.Expect(x => x.GetAction(message)).Return(action);

            action.Expect(x => x.ShouldTriggerBeAvailable(message, node)).Return(false);

            trigger.Expect(x => x.UpdateAvailabilty(false));


            _handler.UpdateAvailability(trigger);
        }

        [Test]
        public void cannot_update_a_trigger_if_missing_trigger_effects()
        {
            var message = new ActionMessage { MethodName = _methodName };
            var action = Mock<IAction>();
            var trigger = Mock<IMessageTrigger>();
            var filters = Stub<IFilterManager>();
            filters.Stub(x => x.TriggerEffects).Return(new IPreProcessor[]{}).Repeat.Any();
            action.Stub(x => x.Filters).Return(filters).Repeat.Any();
            trigger.Stub(x => x.Message).Return(message);

            _host.Expect(x => x.GetAction(message)).Return(action);

            _handler.UpdateAvailability(trigger);
        }

        [Test]
        [ExpectedException(typeof(CaliburnException))]
        public void cannot_update_trigger_for_a_non_action_message()
        {
            var node = Stub<IInteractionNode>();
            var message = new FakeMessage();
            var trigger = Mock<IMessageTrigger>();
            trigger.Stub(x => x.Message).Return(message);

            _handler.UpdateAvailability(trigger);
        }
    }
}