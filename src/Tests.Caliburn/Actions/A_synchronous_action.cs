namespace Tests.Caliburn.Actions
{
    using System;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.Filters;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class A_synchronous_action : TestBase
    {
        SynchronousAction action;
        IMethod method;
        IMessageBinder messageBinder;
        IFilterManager filterManager;

        protected override void given_the_context_of()
        {
            method = Mock<IMethod>();
            method.Stub(x => x.Info).Return(typeof(object).GetMethod("ToString")).Repeat.Any();

            messageBinder = Mock<IMessageBinder>();
            filterManager = Stub<IFilterManager>();

            action = new SynchronousAction(
                Stub<IServiceLocator>(),
                method,
                messageBinder,
                filterManager,
                false
                );
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
            var parameters = new object[] {
                5, "param"
            };
            var returnValue = new object();
            var filter = MockRepository.GenerateMock<IPostProcessor>();
            var context = EventArgs.Empty;

            var handler = Stub<IRoutedMessageHandler>();
            handler.Stub(x => x.Unwrap()).Return(new object());
            handlingNode.Stub(x => x.MessageHandler).Return(handler);

            messageBinder.Expect(x => x.DetermineParameters(message, null, handlingNode, context))
                .Return(parameters);

            filterManager.Stub(x => x.PreProcessors)
                .Return(new IPreProcessor[] {});

            method.Expect(x => x.Invoke(target, parameters))
                .Return(returnValue);

            filterManager.Stub(x => x.PostProcessors)
                .Return(new[] {
                    filter
                });

            var outcome = new MessageProcessingOutcome(returnValue, returnValue.GetType(), false);

            filter.Expect(x => x.Execute(message, handlingNode, outcome));

            messageBinder.Expect(x => x.CreateResult(outcome))
                .IgnoreArguments()
                .Return(result);

            result.Expect(x => x.Execute(null)).IgnoreArguments();

            action.Execute(message, handlingNode, context);
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
            var parameters = new object[] {
                5, "param"
            };

            messageBinder.Expect(x => x.DetermineParameters(message, null, handlingNode, null))
                .Return(parameters);

            filterManager.Stub(x => x.TriggerEffects)
                .Return(new[] {
                    filter1, filter2
                }).Repeat.Any();

            filter1.Expect(x => x.Execute(message, handlingNode, parameters))
                .Return(false);

            filter2.Expect(x => x.Execute(message, handlingNode, parameters))
                .Return(true);

            var result = action.ShouldTriggerBeAvailable(message, handlingNode);

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
            var parameters = new object[] {
                5, "param"
            };

            messageBinder.Expect(x => x.DetermineParameters(message, null, handlingNode, null))
                .Return(parameters);

            filterManager.Stub(x => x.TriggerEffects)
                .Return(new[] {
                    filter
                }).Repeat.Any();

            filter.Expect(x => x.Execute(message, handlingNode, parameters))
                .IgnoreArguments()
                .Return(true);

            var result = action.ShouldTriggerBeAvailable(message, handlingNode);

            Assert.That(result, Is.True);
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
            var parameters = new object[] {
                5, "param"
            };
            var returnValue = new object();
            var context = EventArgs.Empty;

            var handler = Stub<IRoutedMessageHandler>();
            handler.Stub(x => x.Unwrap()).Return(new object());
            handlingNode.Stub(x => x.MessageHandler).Return(handler);

            messageBinder.Expect(x => x.DetermineParameters(message, null, handlingNode, context))
                .Return(parameters);

            filterManager.Stub(x => x.PreProcessors)
                .Return(new IPreProcessor[] {});

            method.Expect(x => x.Invoke(target, parameters))
                .Return(returnValue);

            filterManager.Stub(x => x.PostProcessors)
                .Return(new IPostProcessor[] {});

            messageBinder.Expect(x => x.CreateResult(new MessageProcessingOutcome(
                null,
                method.Info.ReturnType,
                false
                ))).IgnoreArguments().Return(result);

            result.Expect(x => x.Execute(null)).IgnoreArguments();

            action.Execute(message, handlingNode, context);
        }

        [Test]
        public void can_throw_exception_on_execute()
        {
            var handlingNode = Stub<IInteractionNode>();
            var sourceNode = Stub<IInteractionNode>();

            var message = new ActionMessage();
            message.Initialize(sourceNode);

            var target = new object();
            var parameters = new object[] {
                5, "param"
            };
            var returnValue = new object();
            var context = EventArgs.Empty;

            var handler = Stub<IRoutedMessageHandler>();
            handler.Stub(x => x.Unwrap()).Return(new object());
            handlingNode.Stub(x => x.MessageHandler).Return(handler);

            messageBinder.Expect(x => x.DetermineParameters(message, null, handlingNode, context))
                .Return(parameters);

            filterManager.Stub(x => x.PreProcessors)
                .Return(new IPreProcessor[] {});

            method.Expect(x => x.Invoke(target, parameters))
                .Return(returnValue);

            filterManager.Stub(x => x.PostProcessors)
                .Return(new IPostProcessor[] {});

            filterManager.Stub(x => x.Rescues)
                .Return(new IRescue[] {});

            messageBinder.Expect(x => x.CreateResult(new MessageProcessingOutcome(
                null,
                method.Info.ReturnType,
                false
                ))).IgnoreArguments().Throw(new InvalidOperationException("test exception"));

            try
            {
                action.Execute(message, handlingNode, context);
            }
            catch(InvalidOperationException inex)
            {
                Assert.That(inex.StackTrace.Contains("Rhino.Mocks.Expectations.AbstractExpectation.ReturnOrThrow"));
                return;
            }

            Assert.Fail("The test exception was not fired");
        }

        [Test]
        public void reports_has_trigger_affects_if_has_trigger_affecting_filters()
        {
            filterManager.Stub(x => x.TriggerEffects).Return(new[] {
                Stub<IPreProcessor>()
            });

            var result = action.HasTriggerEffects();

            Assert.That(result, Is.True);
        }

        [Test]
        public void reports_not_having_trigger_affects_if_no_trigger_affecting_filters()
        {
            filterManager.Stub(x => x.TriggerEffects).Return(new IPreProcessor[] {});

            var result = action.HasTriggerEffects();

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
            var parameters = new object[] {
                5, "param"
            };
            var context = EventArgs.Empty;

            messageBinder.Expect(x => x.DetermineParameters(message, null, handlingNode, context))
                .Return(parameters);

            filterManager.Stub(x => x.PreProcessors)
                .Return(new[] {
                    filter1, filter2
                });

            filter1.Expect(x => x.Execute(message, handlingNode, parameters))
                .Return(false);

            filter2.Expect(x => x.Execute(message, handlingNode, parameters))
                .Return(true);

            action.Execute(message, handlingNode, context);
        }
    }
}