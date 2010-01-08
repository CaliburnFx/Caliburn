using System;
using Caliburn.Core.Invocation;
using Caliburn.Core.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Tests.Caliburn.Actions
{
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.Filters;
    using Microsoft.Practices.ServiceLocation;

    [TestFixture]
    public class An_asynchronous_action : TestBase
    {
        private AsynchronousAction _action;
        private IMethod _method;
        private IMessageBinder _messageBinder;
        private IFilterManager _filterManager;

        protected override void given_the_context_of()
        {
            _method = Mock<IMethod>();
            _method.Stub(x => x.Info).Return(typeof(object).GetMethod("ToString")).Repeat.Any();

            _messageBinder = Mock<IMessageBinder>();
            _filterManager = Stub<IFilterManager>();

            _action = new AsynchronousAction(
                Stub<IServiceLocator>(),
                _method,
                _messageBinder,
                _filterManager,
                false
                );
        }

        [Test]
        public void reports_has_trigger_affects_if_has_trigger_affecting_filters()
        {
            _filterManager.Stub(x => x.TriggerEffects).Return(new[] { Stub<IPreProcessor>()});

            var result = _action.HasTriggerEffects();

            Assert.That(result, Is.True);
        }

        [Test]
        public void reports_not_having_trigger_affects_if_no_trigger_affecting_filters()
        {
            _filterManager.Stub(x => x.TriggerEffects).Return(new IPreProcessor[]{});

            var result = _action.HasTriggerEffects();

            Assert.That(result, Is.False);
        }

        [Test]
        public void can_determine_positive_trigger_effect()
        {
            var filter = Mock<IPreProcessor>();
            var handlingNode = Stub<IInteractionNode>();

            var sourceNode = Stub<IInteractionNode>();
            var message = new ActionMessage();
            message.Initialize(sourceNode);

            var target = new object();
            var parameters = new object[] { 5, "param" };

            _messageBinder.Expect(x => x.DetermineParameters(message, null, handlingNode, null))
                .Return(parameters);

            _filterManager.Stub(x => x.TriggerEffects)
                .Return(new[] { filter });

            filter.Expect(x => x.Execute(message, handlingNode, parameters))
                .IgnoreArguments()
                .Return(true);

            var result = _action.ShouldTriggerBeAvailable(message, handlingNode);

            Assert.That(result, Is.True);
        }

        [Test]
        public void can_determine_negative_trigger_effect()
        {
            var filter1 = Mock<IPreProcessor>();
            var filter2 = Mock<IPreProcessor>();

            var sourceNode = Stub<IInteractionNode>();
            var message = new ActionMessage();
            message.Initialize(sourceNode);

            var handlingNode = Stub<IInteractionNode>();
            var target = new object();
            var parameters = new object[] { 5, "param" };

            _messageBinder.Expect(x => x.DetermineParameters(message, null, handlingNode, null))
                .Return(parameters);

            _filterManager.Stub(x => x.TriggerEffects)
                .Return(new[] { filter1, filter2 });

            filter1.Expect(x => x.Execute(message, handlingNode, parameters))
                .Return(false);

            filter2.Expect(x => x.Execute(message, handlingNode, parameters))
                .Return(true);

            var result = _action.ShouldTriggerBeAvailable(message, handlingNode);

            Assert.That(result, Is.False);
        }

        [Test]
        public void when_executing_pre_filters_can_cancel()
        {
            var filter1 = Mock<IPreProcessor>();
            var filter2 = Mock<IPreProcessor>();

            var sourceNode = Stub<IInteractionNode>();
            var message = new ActionMessage();
            message.Initialize(sourceNode);

            var handlingNode = Stub<IInteractionNode>();
            var parameters = new object[] { 5, "param" };
            var context = EventArgs.Empty;
            var handler = Stub<IRoutedMessageHandler>();
            handler.Stub(x => x.Unwrap()).Return(new object());
            handlingNode.Stub(x => x.MessageHandler).Return(handler);

            _messageBinder.Expect(x => x.DetermineParameters(message, null, handlingNode, context))
                .Return(parameters);

            _filterManager.Stub(x => x.PreProcessors)
                .Return(new[] { filter1, filter2 });

            filter1.Expect(x => x.Execute(message, handlingNode, parameters))
                .Return(false);

            filter2.Expect(x => x.Execute(message, handlingNode, parameters))
                .Return(true);

            _action.Execute(message, handlingNode, context);
        }

        [Test]
        public void can_execute()
        {
            var soureceNode = Stub<IInteractionNode>();
            var handlingNode = Stub<IInteractionNode>();

            var message = new ActionMessage();
            message.Initialize(soureceNode);
            
            var target = new object();
            var parameters = new object[] { 5, "param" };
            var task = MockRepository.GenerateMock<IBackgroundTask>();
            var context = EventArgs.Empty;
            var handler = Stub<IRoutedMessageHandler>();
            handler.Stub(x => x.Unwrap()).Return(target);
            handlingNode.Stub(x => x.MessageHandler).Return(handler);

            _messageBinder.Expect(x => x.DetermineParameters(message, null, handlingNode, context))
                .IgnoreArguments()
                .Return(parameters);

            _filterManager.Stub(x => x.PreProcessors)
                .Return(new IPreProcessor[] { });

            _method.Stub(x => x.CreateBackgroundTask(target, parameters))
                .Return(task);

            task.Expect(x => x.Enqueue(null));

            _action.Execute(message, handlingNode, context);
        }
    }
}