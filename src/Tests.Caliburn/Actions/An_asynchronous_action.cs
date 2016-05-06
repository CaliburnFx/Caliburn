using Shouldly;

namespace Tests.Caliburn.Actions
{
    using System;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.Filters;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using Xunit;
    using Rhino.Mocks;

    
    public class An_asynchronous_action : TestBase
    {
        AsynchronousAction action;
        IMethod method;
        IMessageBinder messageBinder;
        IFilterManager filterManager;

        protected override void given_the_context_of()
        {
            method = Mock<IMethod>();
            method.Stub(x => x.Info).Return(typeof(object).GetMethod("ToString")).Repeat.Any();

            messageBinder = Mock<IMessageBinder>();
            filterManager = Stub<IFilterManager>();

            action = new AsynchronousAction(
                Stub<IServiceLocator>(),
                method,
                messageBinder,
                filterManager,
                false
                );
        }

        [Fact]
        public void can_determine_negative_trigger_effect()
        {
            var filter1 = Mock<IPreProcessor>();
            var filter2 = Mock<IPreProcessor>();

            var sourceNode = Stub<IInteractionNode>();
            var message = new ActionMessage();
            message.Initialize(sourceNode);

            var handlingNode = Stub<IInteractionNode>();
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

            result.ShouldBeFalse();
        }

        [Fact]
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

            result.ShouldBeTrue();
        }

        [Fact]
        public void can_execute()
        {
            var soureceNode = Stub<IInteractionNode>();
            var handlingNode = Stub<IInteractionNode>();

            var message = new ActionMessage();
            message.Initialize(soureceNode);

            var target = new object();
            var parameters = new object[] {
                5, "param"
            };
            var task = MockRepository.GenerateMock<IBackgroundTask>();
            var context = EventArgs.Empty;
            var handler = Stub<IRoutedMessageHandler>();
            handler.Stub(x => x.Unwrap()).Return(target);
            handlingNode.Stub(x => x.MessageHandler).Return(handler);

            messageBinder.Expect(x => x.DetermineParameters(message, null, handlingNode, context))
                .IgnoreArguments()
                .Return(parameters);

            filterManager.Stub(x => x.PreProcessors)
                .Return(new IPreProcessor[] {});

            method.Stub(x => x.CreateBackgroundTask(target, parameters))
                .Return(task);

            task.Expect(x => x.Start(null));

            action.Execute(message, handlingNode, context);
        }

        [Fact]
        public void reports_has_trigger_affects_if_has_trigger_affecting_filters()
        {
            filterManager.Stub(x => x.TriggerEffects).Return(new[] {
                Stub<IPreProcessor>()
            });

            var result = action.HasTriggerEffects();

            result.ShouldBeTrue();
        }

        [Fact]
        public void reports_not_having_trigger_affects_if_no_trigger_affecting_filters()
        {
            filterManager.Stub(x => x.TriggerEffects).Return(new IPreProcessor[] {});

            var result = action.HasTriggerEffects();

            result.ShouldBeFalse();
        }

        [Fact]
        public void when_executing_pre_filters_can_cancel()
        {
            var filter1 = Mock<IPreProcessor>();
            var filter2 = Mock<IPreProcessor>();

            var sourceNode = Stub<IInteractionNode>();
            var message = new ActionMessage();
            message.Initialize(sourceNode);

            var handlingNode = Stub<IInteractionNode>();
            var parameters = new object[] {
                5, "param"
            };
            var context = EventArgs.Empty;
            var handler = Stub<IRoutedMessageHandler>();
            handler.Stub(x => x.Unwrap()).Return(new object());
            handlingNode.Stub(x => x.MessageHandler).Return(handler);

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