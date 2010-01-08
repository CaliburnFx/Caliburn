namespace Tests.Caliburn.Actions
{
    using System.Linq;
    using global::Caliburn.Core;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.Core.Threading;
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.Filters;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class An_overloaded_action : TestBase
    {
        private OverloadedAction _action;

        protected override void given_the_context_of()
        {
            var methodFactory = new DefaultMethodFactory(
                new DefaultThreadPool()
                );

            _action = new OverloadedAction("Test");

            var infos = typeof(MethodHost)
                .GetMethods()
                .Where(x => x.Name == "Test");

            foreach(var info in infos)
            {
                _action.AddOverload(
                    new SynchronousAction(
                        methodFactory.CreateFrom(info),
                        Stub<IMessageBinder>(),
                        Stub<IFilterManager>(),
                        false
                        )
                    );
            }
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