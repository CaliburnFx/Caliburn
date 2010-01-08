using System;
using Caliburn.Core.Invocation;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Tests.Caliburn.Actions
{
    using System.Collections.Generic;
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.Filters;

    [TestFixture]
    public class A_synchronous_action : TestBase
    {
        private SynchronousAction _action;
        private IMethod _method;
        private IMessageBinder _messageBinder;
        private IFilterManager _filterManager;

        protected override void given_the_context_of()
        {
            _method = Mock<IMethod>();
            _method.Stub(x => x.Info).Return(typeof(object).GetMethod("ToString")).Repeat.Any();

            _messageBinder = Mock<IMessageBinder>();
            _filterManager = Stub<IFilterManager>();

            _action = new SynchronousAction(
                _method,
                _messageBinder,
                _filterManager,
                false
                );
        }

        [Test]
        public void reports_has_trigger_affects_if_has_trigger_affecting_filters()
        {
            _filterManager.Stub(x => x.TriggerEffects).Return(new[] {Stub<IPreProcessor>()});

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
            var parameters = new object[] {5, "param"};

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
            var handlingNode = Stub<IInteractionNode>();
            var sourceNode = Stub<IInteractionNode>();

            var message = new ActionMessage();
            message.Initialize(sourceNode);

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
            var handlingNode = Stub<IInteractionNode>();

            var sourceNode = Stub<IInteractionNode>();

            var message = new ActionMessage();
            message.Initialize(sourceNode);

            var target = new object();
            var parameters = new object[] { 5, "param" };
            var context = EventArgs.Empty;

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
            var handlingNode = Stub<IInteractionNode>();
            var sourceNode = Stub<IInteractionNode>();
            var result = Mock<IResult>();

            var message = new ActionMessage();
            message.Initialize(sourceNode);

            var target = new object();
            var parameters = new object[] { 5, "param" };
            var returnValue = new object();
            var context = EventArgs.Empty;

            var handler = Stub<IRoutedMessageHandler>();
            handler.Stub(x => x.Unwrap()).Return(new object());
            handlingNode.Stub(x => x.MessageHandler).Return(handler);

            _messageBinder.Expect(x => x.DetermineParameters(message, null, handlingNode, context))
                .Return(parameters);

            _filterManager.Stub(x => x.PreProcessors)
                .Return(new IPreProcessor[] { });

            _method.Expect(x => x.Invoke(target, parameters))
                .Return(returnValue);

            _filterManager.Stub(x => x.PostProcessors)
                .Return(new IPostProcessor[] { });

            _messageBinder.Expect(x => x.CreateResult(new MessageProcessingOutcome(
                    null,
                    _method.Info.ReturnType,
                    false
                    ))).IgnoreArguments().Return(result);

            result.Expect(x => x.Execute(message, handlingNode));

            _action.Execute(message, handlingNode, context);
        }

        [Test]
        public void can_throw_exception_on_execute()
        {
            var handlingNode = Stub<IInteractionNode>();
            var sourceNode = Stub<IInteractionNode>();

            var message = new ActionMessage();
            message.Initialize(sourceNode);

            var target = new object();
            var parameters = new object[] { 5, "param" };
            var returnValue = new object();
            var context = EventArgs.Empty;

            var handler = Stub<IRoutedMessageHandler>();
            handler.Stub(x => x.Unwrap()).Return(new object());
            handlingNode.Stub(x => x.MessageHandler).Return(handler);

            _messageBinder.Expect(x => x.DetermineParameters(message, null, handlingNode, context))
                .Return(parameters);

            _filterManager.Stub(x => x.PreProcessors)
                .Return(new IPreProcessor[] { });

            _method.Expect(x => x.Invoke(target, parameters))
                 .Return(returnValue);

            _filterManager.Stub(x => x.PostProcessors)
                .Return(new IPostProcessor[] { });

            _filterManager.Stub(x => x.Rescues)
                .Return(new IRescue[] { }); 

            _messageBinder.Expect(x => x.CreateResult(new MessageProcessingOutcome(
                    null,
                    _method.Info.ReturnType,
                    false
                    ))).IgnoreArguments().Throw(new InvalidOperationException("test exception"));

            try
            {
                _action.Execute(message, handlingNode, context);
            }
            catch (InvalidOperationException inex)
            {
                Assert.That(inex.StackTrace.Contains("Rhino.Mocks.Expectations.AbstractExpectation.ReturnOrThrow"));
                return;
            }

            Assert.Fail("The test exception was not fired");
        }

        [Test]
        public void alters_return_with_post_execute_filters()
        {
            var handlingNode = Stub<IInteractionNode>();
            var sourceNode = Stub<IInteractionNode>();
            var result = Mock<IResult>();

            var message = new ActionMessage();
            message.Initialize(sourceNode);

            var target = new object();
            var parameters = new object[] { 5, "param" };
            var returnValue = new object();
            var filter = MockRepository.GenerateMock<IPostProcessor>();
            var context = EventArgs.Empty;

            var handler = Stub<IRoutedMessageHandler>();
            handler.Stub(x => x.Unwrap()).Return(new object());
            handlingNode.Stub(x => x.MessageHandler).Return(handler);

            _messageBinder.Expect(x => x.DetermineParameters(message, null, handlingNode, context))
                .Return(parameters);

            _filterManager.Stub(x => x.PreProcessors)
                .Return(new IPreProcessor[] { });

            _method.Expect(x => x.Invoke(target, parameters))
                .Return(returnValue);

            _filterManager.Stub(x => x.PostProcessors)
                .Return(new [] { filter });

            var outcome = new MessageProcessingOutcome(returnValue, returnValue.GetType(), false);

            filter.Expect(x => x.Execute(message, handlingNode, outcome));

            _messageBinder.Expect(x => x.CreateResult(outcome))
                .IgnoreArguments()
                .Return(result);

            result.Expect(x => x.Execute(message, handlingNode));

            _action.Execute(message, handlingNode, context);
        }
    }
}