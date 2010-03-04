using System;
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

    [TestFixture]
    public class Rescue_filter : TestBase
    {
        private RescueAttribute _attribute;
        private IMethodFactory _methodFactory;
        private MethodInfo _info;
        private IServiceLocator _container;

        protected override void given_the_context_of()
        {
            _methodFactory = Mock<IMethodFactory>();
            _info = typeof(MethodHost).GetMethod("Rescue");
            _attribute = new RescueAttribute("Rescue");
            _container = Stub<IServiceLocator>();
            _container.Stub(x => x.GetInstance<IMethodFactory>()).Return(_methodFactory).Repeat.Any();
        }

        [Test]
        public void initializes_its_method()
        {
            _attribute.Initialize(typeof(MethodHost), null, _container);

            _methodFactory.AssertWasCalled(x => x.CreateFrom(_info));
        }

        [Test]
        public void can_handle_an_exception()
        {
            var method = Mock<IMethod>();
            method.Stub(x => x.Info).Return(_info);

            var target = new MethodHost();
            var exception = new Exception();

            var handlingNode = new InteractionNode(
                new ControlHost(),
                Mock<IRoutedMessageController>()
                );

            handlingNode.RegisterHandler(Mock<IRoutedMessageHandler>());

            handlingNode.MessageHandler.Stub(x => x.Unwrap())
                .Return(target);

            _methodFactory.Expect(x => x.CreateFrom(_info))
                .Return(method);

            _attribute.Initialize(typeof(MethodHost), null, _container);

            _attribute.Handle(null, handlingNode, exception);

            method.AssertWasCalled(x => x.Invoke(target, exception));
        }

        internal class MethodHost
        {
            public void Rescue(Exception ex)
            {
                
            }
        }
    }
}