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
    public class The_preview_filter : TestBase
    {
        private PreviewAttribute _attribute;
        private IMethodFactory _methodFactory;
        private MethodInfo _info;
        private IServiceLocator _container;

        protected override void given_the_context_of()
        {
            _methodFactory = Mock<IMethodFactory>();
            _info = typeof(MethodHost).GetMethod("Preview");
            _attribute = new PreviewAttribute("Preview");
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
    }
}