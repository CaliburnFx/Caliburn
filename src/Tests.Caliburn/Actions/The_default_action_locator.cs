namespace Tests.Caliburn.Actions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.Conventions;
    using global::Caliburn.PresentationFramework.Filters;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class The_default_action_locator : TestBase
    {
        private DefaultActionLocator _locator;
        private IMethodFactory _methodFactory;
        private IMessageBinder _messageBinder;
        private IServiceLocator _serviceLocator;
        private IConventionManager _conventionManager;

        protected override void given_the_context_of()
        {
            _methodFactory = Mock<IMethodFactory>();
            _messageBinder = Mock<IMessageBinder>();
            _serviceLocator = Mock<IServiceLocator>();
            _conventionManager = Mock<IConventionManager>();

            _locator = new DefaultActionLocator(_serviceLocator, _methodFactory, _messageBinder, _conventionManager);
        }

        [Test]
        public void ignores_object_methods()
        {
            var actions = _locator.Locate(CreateContext(typeof(object)))
                .ToList();

            Assert.That(actions.Count, Is.EqualTo(0));
        }

        [Test]
        public void builds_synchronous_actions_by_default()
        {
            ExpectActionCreated<SimpleActionTarget>();
            ExpectActionCreated<SimpleActionTarget>();

            var result = _locator.Locate(CreateContext(typeof(SimpleActionTarget)))
                .ToList();

            Assert.That(result.Count, Is.EqualTo(2));

            foreach(var action in result)
            {
                Assert.That(action, Is.AssignableFrom(typeof(SynchronousAction)));
            }
        }

        [Test]
        public void bundles_overloads()
        {
            ExpectActionCreated<ActionTargetWithOverloads>();
            ExpectActionCreated<ActionTargetWithOverloads>();
            ExpectActionCreated<ActionTargetWithOverloads>();

            var result = _locator.Locate(CreateContext(typeof(ActionTargetWithOverloads)))
                .ToList();

            Assert.That(result.Count, Is.EqualTo(1));

            var overloadAction = result[0] as OverloadedAction;

            Assert.That(overloadAction, Is.Not.Null);

            foreach(var action in overloadAction)
            {
                Assert.That(action, Is.AssignableFrom(typeof(SynchronousAction)));
            }
        }

        [Test]
        public void determines_async_actions_from_attributes()
        {
            ExpectActionCreated<ActionTargetWithAsync>();

            var result = _locator.Locate(CreateContext(typeof(ActionTargetWithAsync)))
                .ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0], Is.InstanceOfType(typeof(AsynchronousAction)));
        }

        private ActionLocationContext CreateContext(Type type)
        {
            return new ActionLocationContext(
                _serviceLocator,
                type,
                new FilterManager(type, type, _serviceLocator)
                );
        }

        private void ExpectActionCreated<T>()
        {
            var method = Stub<IMethod>();

            _methodFactory.Expect(x => x.CreateFrom(Arg<MethodInfo>.Is.NotNull))
                .Return(method);

            method.Stub(x => x.Info).Return(typeof(T).GetMethods().First()).Repeat.Any();
        }

        public class SimpleActionTarget
        {
            public void ProcOne() {}
            public static void ProcTwo() {}
        }

        public class ActionTargetWithOverloads
        {
            public void ProcOne() {}
            public void ProcOne(int i) {}
            public void ProcOne(int i, double j) {}
        }

        public class ActionTargetWithAsync
        {
            [AsyncAction]
            public void ProcOne() {}
        }
    }
}