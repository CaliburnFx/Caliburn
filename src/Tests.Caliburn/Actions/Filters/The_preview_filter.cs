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
    using Rhino.Mocks;

    
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
            container = Stub<IServiceLocator>();
            container.Stub(x => x.GetInstance(typeof(IMethodFactory), null)).Return(methodFactory).Repeat.Any();

            routedMessageHandler = Mock<IRoutedMessageHandler>();
            var metadata = new List<object>();
            routedMessageHandler.Stub(x => x.Metadata).Return(metadata).Repeat.Any();
        }

        public void SetupForMakeAwareOf(string memberName, MethodInfo info, object handler)
        {
            var method = Mock<IMethod>();
            method.Stub(x => x.Info).Return(info).Repeat.Any();
            methodFactory.Expect(x => x.CreateFrom(info)).Return(method);

            var attribute = new PreviewAttribute(memberName);
            attribute.Initialize(handler.GetType(), info, container);

            if(routedMessageHandler.Unwrap() == null) //prevents double configuration
                routedMessageHandler.Stub(x => x.Unwrap()).Return(handler).Repeat.Any();

            var message = Mock<IRoutedMessage>();
            message.Stub(x => x.RelatesTo(info)).Return(true);
            var messageTrigger = Mock<IMessageTrigger>();
            messageTrigger.Stub(x => x.Message).Return(message).Repeat.Any();
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

        [WpfFact]
        public void can_execute_a_preview()
        {
            var method = Mock<IMethod>();
            var target = new MethodHost();
            var parameter = new object();

            methodFactory.Expect(x => x.CreateFrom(info))
                .Return(method);

            var handlingNode = new InteractionNode(
                new ControlHost(),
                Mock<IRoutedMessageController>()
                );

            handlingNode.RegisterHandler(Mock<IRoutedMessageHandler>());

            handlingNode.MessageHandler.Stub(x => x.Unwrap())
                .Return(target);

            attribute.Initialize(typeof(MethodHost), null, container);

            method.Stub(x => x.Info).Return(info);

            attribute.Execute(null, handlingNode, new[] {
                parameter
            });

            method.AssertWasCalled(x => x.Invoke(target, parameter));
        }

        [Fact]
        public void initializes_its_method()
        {
            attribute.Initialize(typeof(MethodHost), null, container);
            methodFactory.AssertWasCalled(x => x.CreateFrom(info));
        }
    }
}