namespace Tests.Caliburn.Actions.Filters
{
    using System.Reflection;
    using Fakes.UI;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using Xunit;
    using Rhino.Mocks;

    
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
            container = Stub<IServiceLocator>();
            container.Stub(x => x.GetInstance(typeof(IMethodFactory), null)).Return(methodFactory).Repeat.Any();
        }

        internal class MethodHost
        {
            public void Callback(object result) {}
        }

        [WpfFact]
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

            methodFactory.Expect(x => x.CreateFrom(info))
                .Return(method);

            attribute.Initialize(typeof(MethodHost), null, container);

            method.Expect(x => x.Invoke(target, result)).Return(typeof(string));
            method.Stub(x => x.Info).Return(typeof(object).GetMethod("ToString"));

            var outcome = new MessageProcessingOutcome(result, result.GetType(), false);

            attribute.Execute(null, handlingNode, outcome);
        }

        [Fact]
        public void initializes_its_method()
        {
            attribute.Initialize(typeof(MethodHost), null, container);
            methodFactory.AssertWasCalled(x => x.CreateFrom(info));
        }
    }
}