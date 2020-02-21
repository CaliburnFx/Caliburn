namespace Tests.Caliburn.Actions.Filters
{
    using System.Reflection;
    using Fakes.UI;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using Xunit;
    using NSubstitute;


    public class The_async_attribute_filter : TestBase
    {
        AsyncActionAttribute attribute;
        IMethodFactory methodFactory;
        MethodInfo info;
        IServiceLocator container;

        protected override void given_the_context_of()
        {
            methodFactory = Mock<IMethodFactory>();
            info = typeof(MethodHost).GetMethod("Callback");
            attribute = new AsyncActionAttribute {
                Callback = "Callback"
            };
            container = Mock<IServiceLocator>();
            container.GetInstance(typeof(IMethodFactory), null).Returns(methodFactory);
        }

        internal class MethodHost
        {
            public void Callback(object result) {}
        }

        [StaFact]
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

            handlingNode.MessageHandler.Unwrap()
                .Returns(target);

            methodFactory.CreateFrom(info)
                .Returns(method);

            attribute.Initialize(typeof(MethodHost), null, container);

            method.Invoke(target, result).Returns(typeof(string));
            method.Info.Returns(typeof(object).GetMethod("ToString"));

            var outcome = new MessageProcessingOutcome(result, result.GetType(), false);

            attribute.Execute(null, handlingNode, outcome);
        }

        [Fact]
        public void initializes_its_method()
        {
            attribute.Initialize(typeof(MethodHost), null, container);
            methodFactory.Received().CreateFrom(info);
        }
    }
}
