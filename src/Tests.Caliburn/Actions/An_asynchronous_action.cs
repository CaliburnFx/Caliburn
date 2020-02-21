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
    using NSubstitute;


    public class An_asynchronous_action : TestBase
    {
        AsynchronousAction action;
        IMethod method;
        IMessageBinder messageBinder;
        IFilterManager filterManager;

        protected override void given_the_context_of()
        {
            method = Mock<IMethod>();
            method.Info.Returns(typeof(object).GetMethod("ToString"));

            messageBinder = Mock<IMessageBinder>();
            filterManager = Mock<IFilterManager>();

            action = new AsynchronousAction(
                Mock<IServiceLocator>(),
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

            var sourceNode = Mock<IInteractionNode>();
            var message = new ActionMessage();
            message.Initialize(sourceNode);

            var handlingNode = Mock<IInteractionNode>();
            var target = new object();
            var parameters = new object[] {
                5, "param"
            };

            messageBinder.DetermineParameters(message, null, handlingNode, null)
                .Returns(parameters);

            filterManager.TriggerEffects
                .Returns(new[] {
                    filter1, filter2
                });

            filter1.Execute(message, handlingNode, parameters)
                .Returns(false);

            filter2.Execute(message, handlingNode, parameters)
                .Returns(true);

            var result = action.ShouldTriggerBeAvailable(message, handlingNode);

            result.ShouldBeFalse();
        }

        [Fact]
        public void can_determine_positive_trigger_effect()
        {
            var filter = Mock<IPreProcessor>();
            var handlingNode = Mock<IInteractionNode>();

            var sourceNode = Mock<IInteractionNode>();
            var message = new ActionMessage();
            message.Initialize(sourceNode);

            var target = new object();
            var parameters = new object[] {
                5, "param"
            };

            messageBinder.DetermineParameters(message, null, handlingNode, null)
                .Returns(parameters);

            filterManager.TriggerEffects
                .Returns(new[] {
                    filter
                });

            filter.Execute(message, handlingNode, parameters)
                .ReturnsForAnyArgs(true);

            var result = action.ShouldTriggerBeAvailable(message, handlingNode);

            result.ShouldBeTrue();
        }

        [Fact]
        public void can_execute()
        {
            var soureceNode = Mock<IInteractionNode>();
            var handlingNode = Mock<IInteractionNode>();

            var message = new ActionMessage();
            message.Initialize(soureceNode);

            var target = new object();
            var parameters = new object[] {
                5, "param"
            };
            var task = Mock<IBackgroundTask>();
            var context = EventArgs.Empty;
            var handler = Mock<IRoutedMessageHandler>();
            handler.Unwrap().Returns(target);
            handlingNode.MessageHandler.Returns(handler);

            messageBinder.DetermineParameters(message, null, handlingNode, context)
                .ReturnsForAnyArgs(parameters);

            filterManager.PreProcessors
                .Returns(new IPreProcessor[] {});

            method.CreateBackgroundTask(target, parameters)
                .Returns(task);

            action.Execute(message, handlingNode, context);

            task.ReceivedWithAnyArgs().Start(null);
        }

        [Fact]
        public void reports_has_trigger_affects_if_has_trigger_affecting_filters()
        {
            filterManager.TriggerEffects.Returns(new[] {
                Mock<IPreProcessor>()
            });

            var result = action.HasTriggerEffects();

            result.ShouldBeTrue();
        }

        [Fact]
        public void reports_not_having_trigger_affects_if_no_trigger_affecting_filters()
        {
            filterManager.TriggerEffects.Returns(new IPreProcessor[] {});

            var result = action.HasTriggerEffects();

            result.ShouldBeFalse();
        }

        [Fact]
        public void when_executing_pre_filters_can_cancel()
        {
            var filter1 = Mock<IPreProcessor>();
            var filter2 = Mock<IPreProcessor>();

            var sourceNode = Mock<IInteractionNode>();
            var message = new ActionMessage();
            message.Initialize(sourceNode);

            var handlingNode = Mock<IInteractionNode>();
            var parameters = new object[] {
                5, "param"
            };
            var context = EventArgs.Empty;
            var handler = Mock<IRoutedMessageHandler>();
            handler.Unwrap().Returns(new object());
            handlingNode.MessageHandler.Returns(handler);

            messageBinder.DetermineParameters(message, null, handlingNode, context)
                .Returns(parameters);

            filterManager.PreProcessors
                .Returns(new[] {
                    filter1, filter2
                });

            filter1.Execute(message, handlingNode, parameters)
                .Returns(false);

            filter2.Execute(message, handlingNode, parameters)
                .Returns(true);

            action.Execute(message, handlingNode, context);
        }
    }
}
