using System.Reflection;
using Caliburn.Core.Invocation;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Caliburn.Actions.Filters
{
    using Fakes.UI;
    using global::Caliburn.PresentationFramework.Filters;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
	using System.ComponentModel;
	using System.Collections.Generic;
	using global::Caliburn.Core;

    [TestFixture]
    public class The_preview_filter : TestBase
    {
        private PreviewAttribute _attribute;
        private IMethodFactory _methodFactory;
        private MethodInfo _info;
        private IServiceLocator _container;
		private IRoutedMessageHandler _routedMessageHandler;

        protected override void given_the_context_of()
        {
            _methodFactory = Mock<IMethodFactory>();
            _info = typeof(MethodHost).GetMethod("Preview");
            _attribute = new PreviewAttribute("Preview");
            _container = Stub<IServiceLocator>();
            _container.Stub(x => x.GetInstance<IMethodFactory>()).Return(_methodFactory).Repeat.Any();

			_routedMessageHandler = Mock<IRoutedMessageHandler>();
			var metadata = new List<object>();
			_routedMessageHandler.Stub(x => x.Metadata).Return(metadata).Repeat.Any();

        }

        [Test]
        public void initializes_its_method()
        {
            _attribute.Initialize(typeof(MethodHost), null, _container);
            _methodFactory.AssertWasCalled(x => x.CreateFrom(_info));
        }

		[Test]
		public void can_be_made_aware_of_a_message_handler_with_a_property_guard()
		{
			SetupForMakeAwareOf("AProperty", typeof(NotifyingMethodHost).GetMethod("get_AProperty"), new NotifyingMethodHost());
			Assert.IsNotNull(_routedMessageHandler.Metadata.FirstOrDefaultOfType<DependencyObserver>());
			Assert.IsNull(_routedMessageHandler.Metadata.FirstOrDefaultOfType<EventMonitor>());
		}

		[Test]
		public void can_be_made_aware_of_a_message_handler_with_a_method_guard()
		{
			SetupForMakeAwareOf("AMethod", typeof(NotifyingMethodHost).GetMethod("AMethod"), new NotifyingMethodHost());
			Assert.IsNull(_routedMessageHandler.Metadata.FirstOrDefaultOfType<DependencyObserver>());
			Assert.IsNull(_routedMessageHandler.Metadata.FirstOrDefaultOfType<EventMonitor>());
		}

		[Test]
		public void can_be_made_aware_of_a_message_handler_with_both_a_property_and_a_method_guard()
		{
			SetupForMakeAwareOf("AProperty", typeof(NotifyingMethodHost).GetMethod("get_AProperty"), new NotifyingMethodHost());
			SetupForMakeAwareOf("AMethod", typeof(NotifyingMethodHost).GetMethod("AMethod"), new NotifyingMethodHost());
			Assert.IsNotNull(_routedMessageHandler.Metadata.FirstOrDefaultOfType<DependencyObserver>());
			Assert.IsNull(_routedMessageHandler.Metadata.FirstOrDefaultOfType<EventMonitor>());
		}

		[Test]
		public void can_be_made_aware_of_a_message_handler_with_a_property_guard_with_ChangedEvent()
		{
			SetupForMakeAwareOf("AProperty", typeof(MethodHostWithEvent).GetMethod("get_AProperty"), new MethodHostWithEvent());
			Assert.IsNull(_routedMessageHandler.Metadata.FirstOrDefaultOfType<DependencyObserver>());
			Assert.IsNotNull(_routedMessageHandler.Metadata.FirstOrDefaultOfType<EventMonitor>());
		}
		[Test]
		public void can_be_made_aware_of_a_message_handler_with_a_property_guard_with_mismatching_ChangedEvent()
		{
			SetupForMakeAwareOf("AnotherProperty", typeof(MethodHostWithEvent).GetMethod("get_AnotherProperty"), new MethodHostWithEvent());
			Assert.IsNull(_routedMessageHandler.Metadata.FirstOrDefaultOfType<DependencyObserver>());
			Assert.IsNull(_routedMessageHandler.Metadata.FirstOrDefaultOfType<EventMonitor>());
		}

		public void SetupForMakeAwareOf(string memberName, MethodInfo info, object handler)
		{
			var method = Mock<IMethod>();
			method.Stub(x => x.Info).Return(info).Repeat.Any();
			_methodFactory.Expect(x => x.CreateFrom(info)).Return(method);

			var attribute = new PreviewAttribute(memberName);
			attribute.Initialize(handler.GetType(), info, _container);

			if (_routedMessageHandler.Unwrap() == null) //prevents double configuration
				_routedMessageHandler.Stub(x => x.Unwrap()).Return(handler).Repeat.Any();
			
			var message = Mock<IRoutedMessage>();
			message.Stub(x=>x.RelatesTo(info)).Return(true);
			var messageTrigger = Mock<IMessageTrigger>();
			messageTrigger.Stub(x => x.Message).Return(message).Repeat.Any();
			attribute.MakeAwareOf(_routedMessageHandler);
			attribute.MakeAwareOf(_routedMessageHandler, messageTrigger);
		}
		 

        [Test]
        public void can_execute_a_preview()
        {
            var method = Mock<IMethod>();
            var target = new MethodHost();
            var parameter = new object();

            _methodFactory.Expect(x => x.CreateFrom(_info))
                .Return(method);

            var handlingNode = new InteractionNode(
                new ControlHost(),
                Mock<IRoutedMessageController>()
                );

            handlingNode.RegisterHandler(Mock<IRoutedMessageHandler>());

            handlingNode.MessageHandler.Stub(x => x.Unwrap())
                .Return(target);

            _attribute.Initialize(typeof(MethodHost), null, _container);

            method.Stub(x => x.Info).Return(_info);

            _attribute.Execute(null, handlingNode,  new[] {parameter});

            method.AssertWasCalled(x => x.Invoke(target, parameter));
        }

        internal class MethodHost
        {
            public void Preview(object parmater)
            {

            }
        }

		internal class NotifyingMethodHost: INotifyPropertyChanged
		{
			public bool AProperty { get { return false; } }
			public bool AMethod() { return false; }

			public event PropertyChangedEventHandler PropertyChanged = delegate { };
		}
		internal class MethodHostWithEvent
		{
			public bool AProperty { get { return false; } }
			public bool AnotherProperty { get { return false; } }
			public event System.EventHandler APropertyChanged = delegate { };
		}
		
    }
}