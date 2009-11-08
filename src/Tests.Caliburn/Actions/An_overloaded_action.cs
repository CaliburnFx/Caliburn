using Caliburn.Core;
using Caliburn.Core.Invocation;
using Caliburn.Core.Threading;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Tests.Caliburn.Actions
{
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Actions;

    [TestFixture]
    public class An_overloaded_action : TestBase
    {
        private OverloadedAction _action;
        private IServiceLocator _container;

        protected override void given_the_context_of()
        {
            var methodFactory = new MethodFactory(
                new DefaultThreadPool()
                );

            var actionFactory = new ActionFactory(
                methodFactory,
                new MessageBinder(
                    new RoutedMessageController()
                    )
                );

            _container = Stub<IServiceLocator>();
            _container.Stub(x => x.GetInstance<IMethodFactory>()).Return(methodFactory).Repeat.Any();

            var host = new ActionHost(typeof(MethodHost), actionFactory, _container);

            _action = host.GetAction(new ActionMessage { MethodName = "Test" }) as OverloadedAction;
        }

        [Test]
        public void can_determine_overload()
        {
            var message = new ActionMessage();

            message.Parameters.Add(new Parameter(5));
            var found = _action.DetermineOverloadOrFail(message);

            Assert.That(found, Is.Not.Null);

            message.Parameters.Add(new Parameter("hello"));
            found = _action.DetermineOverloadOrFail(message);

            Assert.That(found, Is.Not.Null);

            message.Parameters.Add(new Parameter(5d));
            found = _action.DetermineOverloadOrFail(message);

            Assert.That(found, Is.Not.Null);
        }

        [Test]
        [ExpectedException(typeof(CaliburnException))]
        public void fails_if_no_match_is_found()
        {
            var message = new ActionMessage();
            _action.DetermineOverloadOrFail(message);
        }

        public class MethodHost
        {
            public void Test(int number) {}
            public void Test(int number, string text) {}
            public void Test(int number, string text, double value) {}
        }
    }
}