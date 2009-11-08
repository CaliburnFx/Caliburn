using System.Linq;
using System.Reflection;
using Caliburn.Core.Invocation;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Tests.Caliburn.Actions
{
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.Filters;

    [TestFixture]
    public class The_default_action_factory : TestBase
    {
        private ActionFactory _factory;
        private IMethodFactory _methodFactory;
        private IMessageBinder _messageBinder;
        private IActionHost _host;

        protected override void given_the_context_of()
        {
            _methodFactory = Mock<IMethodFactory>();
            _messageBinder = Mock<IMessageBinder>();
            _host = Mock<IActionHost>();

            _factory = new ActionFactory(_methodFactory, _messageBinder);
        }

        [Test]
        public void ignores_object_methods()
        {
            _host.Stub(x => x.TargetType).Return(typeof(object));

            var result = _factory.CreateFor(_host).ToList();

            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void builds_synchronous_actions_by_default()
        {
            ExpectActionCreated<SimpleActionTarget>(false);
            ExpectActionCreated<SimpleActionTarget>(false);

            var result = _factory.CreateFor(_host).ToList();

            Assert.That(result.Count, Is.EqualTo(2));

            foreach (var action in result)
            {
                Assert.That(action, Is.AssignableFrom(typeof(SynchronousAction)));
            }
        }

        [Test]
        public void bundles_overloads()
        {
            ExpectActionCreated<ActionTargetWithOverloads>(false);
            ExpectActionCreated<ActionTargetWithOverloads>(false);
            ExpectActionCreated<ActionTargetWithOverloads>(false);

            var result = _factory.CreateFor(_host).ToList();

            Assert.That(result.Count, Is.EqualTo(1));

            var overloadAction = result[0] as OverloadedAction;

            Assert.That(overloadAction, Is.Not.Null);

            foreach (var action in overloadAction)
            {
                Assert.That(action, Is.AssignableFrom(typeof(SynchronousAction)));
            }
        }

        [Test]
        public void determines_async_actions_from_attributes()
        {
            ExpectActionCreated<ActionTargetWithAsync>(true);

            var result = _factory.CreateFor(_host).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0], Is.InstanceOfType(typeof(AsynchronousAction)));
        }

        [Test]
        public void determines_if_blocking_should_occur_from_async_attribute()
        {
            ExpectActionCreated<ActionTargetWithAsync>(true, true);

            var result = _factory.CreateFor(_host).ToList();

            Assert.That(result.Count, Is.EqualTo(1));

            var att = result[0] as AsynchronousAction;

            Assert.That(att, Is.Not.Null);
            Assert.That(att.BlockInteraction, Is.True);
        }

        private void ExpectActionCreated<T>(bool isAsync)
        {
            ExpectActionCreated<T>(isAsync, false);
        }

        private void ExpectActionCreated<T>(bool isAsync, bool asyncBlocksInteraction) 
        {
            var method = Stub<IMethod>();

            _host.Stub(x => x.TargetType).Return(typeof(T));
            _methodFactory.Expect(x => x.CreateFrom(Arg<MethodInfo>.Is.NotNull)).Return(method);

            if(isAsync)
                method.Expect(x => x.GetMetadata<AsyncActionAttribute>()).Return(new AsyncActionAttribute{ BlockInteraction = asyncBlocksInteraction});
            else method.Expect(x => x.GetMetadata<AsyncActionAttribute>()).Return(null);

            method.Stub(x => x.Info).Return(typeof(T).GetMethods().First()).Repeat.Any();

            method.Expect(x => x.GetMatchingMetadata<PreviewAttribute>()).Return(
                new System.Collections.Generic.List<PreviewAttribute>());

            _host.Expect(x => x.GetFilterManager(method)).Return(null);
        }

        public class SimpleActionTarget
        {
            public void ProcOne() { }
            public static void ProcTwo() { }
        }

        public class ActionTargetWithOverloads
        {
            public void ProcOne() { }
            public void ProcOne(int i) { }
            public void ProcOne(int i, double j) { }
        }

        public class ActionTargetWithAsync
        {
            [AsyncAction]
            public void ProcOne() { }
        }
    }
}