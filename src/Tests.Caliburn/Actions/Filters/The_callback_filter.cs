using System.Reflection;
using Caliburn.Core.Invocation;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Caliburn.Actions.Filters
{
    using Fakes.UI;
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.RoutedMessaging;

    [TestFixture]
    public class The_async_attribute_filter : TestBase
    {
        private AsyncActionAttribute _attribute;
        private IMethodFactory _methodFactory;
        private MethodInfo _info;
        private IServiceLocator _container;

        protected override void given_the_context_of()
        {
            _methodFactory = Mock<IMethodFactory>();
            _info = typeof(MethodHost).GetMethod("Callback");
            _attribute = new AsyncActionAttribute { Callback = "Callback" };
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
        public void can_execute_a_callback()
        {
            var method = Mock<IMethod>();
            var target = new MethodHost();
            var result = new object();

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

            method.Expect(x => x.Invoke(target, result)).Return(typeof(string));
            method.Stub(x => x.Info).Return(typeof(object).GetMethod("ToString"));

            var outcome = new MessageProcessingOutcome(result, result.GetType(), false);

            _attribute.Execute(null, handlingNode, outcome);
        }

        internal class MethodHost
        {
            public void Callback(object result)
            {

            }
        }
    }
}