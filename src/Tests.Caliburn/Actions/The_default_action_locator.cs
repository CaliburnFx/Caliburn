namespace Tests.Caliburn.Actions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.Conventions;
    using global::Caliburn.PresentationFramework.Filters;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class The_default_action_locator : TestBase
    {
        DefaultActionLocator locator;
        IMethodFactory methodFactory;
        IMessageBinder messageBinder;
        IServiceLocator serviceLocator;
        IConventionManager conventionManager;

        protected override void given_the_context_of()
        {
            methodFactory = Mock<IMethodFactory>();
            messageBinder = Mock<IMessageBinder>();
            serviceLocator = Mock<IServiceLocator>();
            conventionManager = Mock<IConventionManager>();

            locator = new DefaultActionLocator(serviceLocator, methodFactory, messageBinder, conventionManager);
        }

        ActionLocationContext CreateContext(Type type)
        {
            return new ActionLocationContext(
                serviceLocator,
                type,
                new FilterManager(type, type, serviceLocator)
                );
        }

        void ExpectActionCreated<T>()
        {
            var method = Stub<IMethod>();

            methodFactory.Expect(x => x.CreateFrom(Arg<MethodInfo>.Is.NotNull))
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

        [Test]
        public void builds_synchronous_actions_by_default()
        {
            ExpectActionCreated<SimpleActionTarget>();
            ExpectActionCreated<SimpleActionTarget>();

            var result = locator.Locate(CreateContext(typeof(SimpleActionTarget)))
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

            var result = locator.Locate(CreateContext(typeof(ActionTargetWithOverloads)))
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

            var result = locator.Locate(CreateContext(typeof(ActionTargetWithAsync)))
                .ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0], Is.InstanceOf<AsynchronousAction>());
        }

        [Test]
        public void ignores_object_methods()
        {
            var actions = locator.Locate(CreateContext(typeof(object)))
                .ToList();

            Assert.That(actions.Count, Is.EqualTo(0));
        }
    }
}