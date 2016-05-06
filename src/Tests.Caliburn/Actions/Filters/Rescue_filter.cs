namespace Tests.Caliburn.Actions.Filters
{
    using System;
    using System.Reflection;
    using Fakes.UI;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework.Filters;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using Xunit;
    using Rhino.Mocks;

    
    public class Rescue_filter : TestBase
    {
        RescueAttribute attribute;
        IMethodFactory methodFactory;
        MethodInfo info;
        IServiceLocator container;

        protected override void given_the_context_of()
        {
            methodFactory = Mock<IMethodFactory>();
            info = typeof(MethodHost).GetMethod("Rescue");
            attribute = new RescueAttribute("Rescue");
            container = Stub<IServiceLocator>();
            container.Stub(x => x.GetInstance(typeof(IMethodFactory), null)).Return(methodFactory).Repeat.Any();
        }

        internal class MethodHost
        {
            public void Rescue(Exception ex) {}
        }

        [WpfFact]
        public void can_handle_an_exception()
        {
            var method = Mock<IMethod>();
            method.Stub(x => x.Info).Return(info);

            var target = new MethodHost();
            var exception = new Exception();

            var handlingNode = new InteractionNode(
                new ControlHost(),
                Mock<IRoutedMessageController>()
                );

            handlingNode.RegisterHandler(Mock<IRoutedMessageHandler>());

            handlingNode.MessageHandler.Stub(x => x.Unwrap())
                .Return(target);

            methodFactory.Expect(x => x.CreateFrom(info))
                .Return(method);

            attribute.Initialize(typeof(MethodHost), null, container);

            attribute.Handle(null, handlingNode, exception);

            method.AssertWasCalled(x => x.Invoke(target, exception));
        }

        [Fact]
        public void initializes_its_method()
        {
            attribute.Initialize(typeof(MethodHost), null, container);

            methodFactory.AssertWasCalled(x => x.CreateFrom(info));
        }
    }
}