using Shouldly;

namespace Tests.Caliburn.Core
{
    using System;
    using Fakes;
    using global::Caliburn.Core.InversionOfControl;
    using Xunit;

    
    public class The_SimpleContainer
    {
        public The_SimpleContainer()
        {
            container = new SimpleContainer();
        }

        SimpleContainer container;

        [Fact]
        public void automatically_registers_itself()
        {
            var container = this.container.GetInstance<IServiceLocator>();

            this.container.ShouldBeSameAs(container);
        }

        [Fact]
        public void can_add_a_type_handler()
        {
            container.AddHandler(typeof(ITestService), () => new TestService());

            container.IsRegistered(typeof(ITestService)).ShouldBeTrue();
        }

        [Fact]
        public void can_create_concrete_types_without_registration()
        {
            var instance = container.GetInstance<TestService>();

            instance.ShouldNotBeNull();
            instance.ShouldBeOfType<TestService>();
        }

        [Fact]
        public void can_fill_dependencies_using_the_greediest_constructor()
        {
            container.Register<ITestService, TestService>();
            container.Register<IDependentService, DependentService>();

            var instance = container.GetInstance<IDependentService>();

            instance.ShouldNotBeNull();
            instance.ShouldBeOfType<DependentService>();
            instance.Dependency.ShouldNotBeNull();
            instance.Dependency.ShouldBeOfType<TestService>();
        }

        [Fact]
        public void can_locate_a_type_handler()
        {
            Func<object> handler = () => new TestService();

            container.AddHandler(typeof(ITestService), handler);

            var found = container.GetHandler(typeof(ITestService));

            found.ShouldBe(handler);
        }

        [Fact]
        public void can_register_a_service_by_key()
        {
            container.Register("test", typeof(TestService));

            container.IsRegistered("test").ShouldBeTrue();
        }

        [Fact]
        public void can_register_a_service_by_type()
        {
            container.Register(typeof(ITestService), typeof(TestService));

            container.IsRegistered(typeof(ITestService)).ShouldBeTrue();
        }

        [Fact]
        public void can_register_a_service_using_generics()
        {
            container.Register<ITestService, TestService>();

            container.IsRegistered(typeof(ITestService)).ShouldBeTrue();
        }

        [Fact]
        public void can_register_a_service_using_key_and_generics()
        {
            container.Register<TestService>("test");

            container.IsRegistered("test").ShouldBeTrue();
        }

        [Fact]
        public void can_register_a_singleton_by_key()
        {
            container.RegisterSingleton("test", typeof(TestService));

            container.IsRegistered("test").ShouldBeTrue();
        }

        [Fact]
        public void can_register_a_singleton_by_type()
        {
            container.RegisterSingleton(typeof(ITestService), typeof(TestService));

            container.IsRegistered(typeof(ITestService)).ShouldBeTrue();
        }

        [Fact]
        public void can_register_a_singleton_using_generics()
        {
            container.RegisterSingleton<ITestService, TestService>();

            container.IsRegistered(typeof(ITestService)).ShouldBeTrue();
        }

        [Fact]
        public void can_register_a_singleton_using_key_and_generics()
        {
            container.RegisterSingleton<TestService>("test");

            container.IsRegistered("test").ShouldBeTrue();
        }

        [Fact]
        public void can_resolve_a_generic_type()
        {
            container.Register(typeof(ITestService<>), typeof(TestService<>));

            var instance = container.GetInstance<ITestService<int>>();

            instance.ShouldNotBeNull();
            instance.ShouldBeOfType<TestService<int>>();
        }

        [Fact]
        public void can_resolve_a_generic_type_by_key()
        {
            container.Register(typeof(ITestService<>), typeof(TestService<>), "test");

            var instance = container.GetInstance<ITestService<int>>("test");

            instance.ShouldNotBeNull();
            instance.ShouldBeOfType<TestService<int>>();
        }

        [Fact]
        public void can_resolve_a_service_by_key()
        {
            container.Register<TestService>("test");

            var instance = container.GetInstance("test");

            instance.ShouldNotBeNull();
            instance.ShouldBeOfType<TestService>();
        }

        [Fact]
        public void can_resolve_a_service_by_type()
        {
            container.Register<ITestService, TestService>();

            var instance = container.GetInstance(typeof(ITestService));

            instance.ShouldNotBeNull();
            instance.ShouldBeOfType<TestService>();
        }

        [Fact]
        public void can_resolve_a_service_using_generics()
        {
            container.Register<ITestService, TestService>();

            var instance = container.GetInstance<ITestService>();

            instance.ShouldNotBeNull();
            instance.ShouldBeOfType<TestService>();
        }

        [Fact]
        public void can_resolve_a_singleton_by_key()
        {
            container.RegisterSingleton<TestService>("test");

            var instance1 = container.GetInstance("test");
            var instance2 = container.GetInstance("test");

            instance1.ShouldNotBeNull();
            instance1.ShouldBeOfType<TestService>();

            instance2.ShouldNotBeNull();
            instance2.ShouldBeOfType<TestService>();

            instance1.ShouldBeSameAs(instance2);
        }

        [Fact]
        public void can_resolve_a_singleton_by_type()
        {
            container.RegisterSingleton<ITestService, TestService>();

            var instance1 = container.GetInstance(typeof(ITestService));
            var instance2 = container.GetInstance(typeof(ITestService));

            instance1.ShouldNotBeNull();
            instance1.ShouldBeOfType<TestService>();

            instance2.ShouldNotBeNull();
            instance2.ShouldBeOfType<TestService>();

            instance1.ShouldBeSameAs(instance2);
        }

        [Fact]
        public void can_resolve_a_singleton_using_generics()
        {
            container.RegisterSingleton<ITestService, TestService>();

            var instance1 = container.GetInstance<ITestService>();
            var instance2 = container.GetInstance<ITestService>();

            instance1.ShouldNotBeNull();
            instance1.ShouldBeOfType<TestService>();

            instance2.ShouldNotBeNull();
            instance2.ShouldBeOfType<TestService>();

            instance1.ShouldBeSameAs(instance2);
        }
    }
}