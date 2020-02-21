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
    using NSubstitute;


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
            container = Mock<IServiceLocator>();
            container.GetInstance(typeof(IMethodFactory), null).Returns(methodFactory);
        }

        internal class MethodHost
        {
            public void Rescue(Exception ex) {}
        }

        [StaFact]
        public void can_handle_an_exception()
        {
            var method = Mock<IMethod>();
            method.Info.Returns(info);

            var target = new MethodHost();
            var exception = new Exception();

            var handlingNode = new InteractionNode(
                new ControlHost(),
                Mock<IRoutedMessageController>()
                );

            handlingNode.RegisterHandler(Mock<IRoutedMessageHandler>());

            handlingNode.MessageHandler.Unwrap()
                .Returns(target);

            methodFactory.CreateFrom(info)
                .Returns(method);

            attribute.Initialize(typeof(MethodHost), null, container);

            attribute.Handle(null, handlingNode, exception);

            method.Received().Invoke(target, exception);
        }

        [Fact]
        public void initializes_its_method()
        {
            attribute.Initialize(typeof(MethodHost), null, container);

            methodFactory.Received().CreateFrom(info);
        }
    }
}
