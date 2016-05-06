using Shouldly;

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
    using Xunit;
    using Rhino.Mocks;

    
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

        [Fact]
        public void builds_synchronous_actions_by_default()
        {
            ExpectActionCreated<SimpleActionTarget>();
            ExpectActionCreated<SimpleActionTarget>();

            var result = locator.Locate(CreateContext(typeof(SimpleActionTarget)))
                .ToList();

            result.Count.ShouldBe(2);

            foreach(var action in result)
            {
                action.ShouldBeAssignableTo<SynchronousAction>();
            }
        }

        [Fact]
        public void bundles_overloads()
        {
            ExpectActionCreated<ActionTargetWithOverloads>();
            ExpectActionCreated<ActionTargetWithOverloads>();
            ExpectActionCreated<ActionTargetWithOverloads>();

            var result = locator.Locate(CreateContext(typeof(ActionTargetWithOverloads)))
                .ToList();

            result.Count.ShouldBe(1);

            var overloadAction = result[0] as OverloadedAction;

            overloadAction.ShouldNotBeNull();

            foreach(var action in overloadAction)
            {
                action.ShouldBeAssignableTo<SynchronousAction>();
            }
        }

        [Fact]
        public void determines_async_actions_from_attributes()
        {
            ExpectActionCreated<ActionTargetWithAsync>();

            var result = locator.Locate(CreateContext(typeof(ActionTargetWithAsync)))
                .ToList();

            result.Count.ShouldBe(1);
            result[0].ShouldBeOfType<AsynchronousAction>();
        }

        [Fact]
        public void ignores_object_methods()
        {
            var actions = locator.Locate(CreateContext(typeof(object)))
                .ToList();

            actions.Count.ShouldBe(0);
        }
    }
}