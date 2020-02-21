using Shouldly;

namespace Tests.Caliburn.Actions.Filters
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using Fakes.UI;
    using global::Caliburn.Core;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework.Filters;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using Xunit;
    using NSubstitute;


    public class The_preview_filter : TestBase
    {
        PreviewAttribute attribute;
        IMethodFactory methodFactory;
        MethodInfo info;
        IServiceLocator container;
        IRoutedMessageHandler routedMessageHandler;

        protected override void given_the_context_of()
        {
            methodFactory = Mock<IMethodFactory>();
            info = typeof(MethodHost).GetMethod("Preview");
            attribute = new PreviewAttribute("Preview");
            container = Mock<IServiceLocator>();
            container.GetInstance(typeof(IMethodFactory), null).Returns(methodFactory);

            routedMessageHandler = Mock<IRoutedMessageHandler>();
            var metadata = new List<object>();
            routedMessageHandler.Metadata.Returns(metadata);
        }

        public void SetupForMakeAwareOf(string memberName, MethodInfo info, object handler)
        {
            var method = Mock<IMethod>();
            method.Info.Returns(info);
            methodFactory.CreateFrom(info).Returns(method);

            var attribute = new PreviewAttribute(memberName);
            attribute.Initialize(handler.GetType(), info, container);

            if(routedMessageHandler.Unwrap() == null) //prevents double configuration
                routedMessageHandler.Unwrap().Returns(handler);

            var message = Mock<IRoutedMessage>();
            message.RelatesTo(info).Returns(true);
            var messageTrigger = Mock<IMessageTrigger>();
            messageTrigger.Message.Returns(message);
            attribute.MakeAwareOf(routedMessageHandler);
            attribute.MakeAwareOf(routedMessageHandler, messageTrigger);
        }

        internal class MethodHost
        {
            public void Preview(object parmater) {}
        }

        internal class NotifyingMethodHost : INotifyPropertyChanged
        {
            public bool AProperty
            {
                get { return false; }
            }

            public event PropertyChangedEventHandler PropertyChanged = delegate { };

            public bool AMethod()
            {
                return false;
            }
        }

        internal class MethodHostWithEvent
        {
            public bool AProperty
            {
                get { return false; }
            }

            public bool AnotherProperty
            {
                get { return false; }
            }

            public event EventHandler APropertyChanged = delegate { };
        }

        [Fact]
        public void can_be_made_aware_of_a_message_handler_with_a_method_guard()
        {
            SetupForMakeAwareOf("AMethod", typeof(NotifyingMethodHost).GetMethod("AMethod"), new NotifyingMethodHost());
            routedMessageHandler.Metadata.FirstOrDefaultOfType<DependencyObserver>().ShouldBeNull();
            routedMessageHandler.Metadata.FirstOrDefaultOfType<EventMonitor>().ShouldBeNull();
        }

        [Fact]
        public void can_be_made_aware_of_a_message_handler_with_a_property_guard()
        {
            SetupForMakeAwareOf("AProperty", typeof(NotifyingMethodHost).GetMethod("get_AProperty"), new NotifyingMethodHost());
            routedMessageHandler.Metadata.FirstOrDefaultOfType<DependencyObserver>().ShouldNotBeNull();
            routedMessageHandler.Metadata.FirstOrDefaultOfType<EventMonitor>().ShouldBeNull();
        }

        [Fact]
        public void can_be_made_aware_of_a_message_handler_with_a_property_guard_with_ChangedEvent()
        {
            SetupForMakeAwareOf("AProperty", typeof(MethodHostWithEvent).GetMethod("get_AProperty"), new MethodHostWithEvent());
            routedMessageHandler.Metadata.FirstOrDefaultOfType<DependencyObserver>().ShouldBeNull();
            routedMessageHandler.Metadata.FirstOrDefaultOfType<EventMonitor>().ShouldNotBeNull();
        }

        [Fact]
        public void can_be_made_aware_of_a_message_handler_with_a_property_guard_with_mismatching_ChangedEvent()
        {
            SetupForMakeAwareOf("AnotherProperty", typeof(MethodHostWithEvent).GetMethod("get_AnotherProperty"), new MethodHostWithEvent());
            routedMessageHandler.Metadata.FirstOrDefaultOfType<DependencyObserver>().ShouldBeNull();
            routedMessageHandler.Metadata.FirstOrDefaultOfType<EventMonitor>().ShouldBeNull();
        }

        [Fact]
        public void can_be_made_aware_of_a_message_handler_with_both_a_property_and_a_method_guard()
        {
            SetupForMakeAwareOf("AProperty", typeof(NotifyingMethodHost).GetMethod("get_AProperty"), new NotifyingMethodHost());
            SetupForMakeAwareOf("AMethod", typeof(NotifyingMethodHost).GetMethod("AMethod"), new NotifyingMethodHost());
            routedMessageHandler.Metadata.FirstOrDefaultOfType<DependencyObserver>().ShouldNotBeNull();
            routedMessageHandler.Metadata.FirstOrDefaultOfType<EventMonitor>().ShouldBeNull();
        }

        [StaFact]
        public void can_execute_a_preview()
        {
            var method = Mock<IMethod>();
            var target = new MethodHost();
            var parameter = new object();

            methodFactory.CreateFrom(info)
                .Returns(method);

            var handlingNode = new InteractionNode(
                new ControlHost(),
                Mock<IRoutedMessageController>()
                );

            handlingNode.RegisterHandler(Mock<IRoutedMessageHandler>());

            handlingNode.MessageHandler.Unwrap()
                .Returns(target);

            attribute.Initialize(typeof(MethodHost), null, container);

            method.Info.Returns(info);

            attribute.Execute(null, handlingNode, new[] {
                parameter
            });

            method.Received().Invoke(target, parameter);
        }

        [Fact]
        public void initializes_its_method()
        {
            attribute.Initialize(typeof(MethodHost), null, container);
            methodFactory.Received().CreateFrom(info);
        }
    }
}
