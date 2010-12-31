namespace Tests.Caliburn.Core
{
    using System;
    using Fakes;
    using global::Caliburn.Core.InversionOfControl;
    using NUnit.Framework;

    [TestFixture]
    public class The_SimpleContainer
    {
        [SetUp]
        public void SetUp()
        {
            container = new SimpleContainer();
        }

        SimpleContainer container;

        [Test]
        public void automatically_registers_itself()
        {
            var container = this.container.GetInstance<IServiceLocator>();

            Assert.That(this.container, Is.EqualTo(container));
        }

        [Test]
        public void can_add_a_type_handler()
        {
            container.AddHandler(typeof(ITestService), () => new TestService());

            Assert.That(container.IsRegistered(typeof(ITestService)));
        }

        [Test]
        public void can_create_concrete_types_without_registration()
        {
            var instance = container.GetInstance<TestService>();

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.AssignableFrom(typeof(TestService)));
        }

        [Test]
        public void can_fill_dependencies_using_the_greediest_constructor()
        {
            container.Register<ITestService, TestService>();
            container.Register<IDependentService, DependentService>();

            var instance = container.GetInstance<IDependentService>();

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.AssignableFrom(typeof(DependentService)));
            Assert.That(instance.Dependency, Is.Not.Null);
            Assert.That(instance.Dependency, Is.AssignableFrom(typeof(TestService)));
        }

        [Test]
        public void can_locate_a_type_handler()
        {
            Func<object> handler = () => new TestService();

            container.AddHandler(typeof(ITestService), handler);

            var found = container.GetHandler(typeof(ITestService));

            Assert.That(found, Is.EqualTo(handler));
        }

        [Test]
        public void can_register_a_service_by_key()
        {
            container.Register("test", typeof(TestService));

            Assert.That(container.IsRegistered("test"));
        }

        [Test]
        public void can_register_a_service_by_type()
        {
            container.Register(typeof(ITestService), typeof(TestService));

            Assert.That(container.IsRegistered(typeof(ITestService)));
        }

        [Test]
        public void can_register_a_service_using_generics()
        {
            container.Register<ITestService, TestService>();

            Assert.That(container.IsRegistered(typeof(ITestService)));
        }

        [Test]
        public void can_register_a_service_using_key_and_generics()
        {
            container.Register<TestService>("test");

            Assert.That(container.IsRegistered("test"));
        }

        [Test]
        public void can_register_a_singleton_by_key()
        {
            container.RegisterSingleton("test", typeof(TestService));

            Assert.That(container.IsRegistered("test"));
        }

        [Test]
        public void can_register_a_singleton_by_type()
        {
            container.RegisterSingleton(typeof(ITestService), typeof(TestService));

            Assert.That(container.IsRegistered(typeof(ITestService)));
        }

        [Test]
        public void can_register_a_singleton_using_generics()
        {
            container.RegisterSingleton<ITestService, TestService>();

            Assert.That(container.IsRegistered(typeof(ITestService)));
        }

        [Test]
        public void can_register_a_singleton_using_key_and_generics()
        {
            container.RegisterSingleton<TestService>("test");

            Assert.That(container.IsRegistered("test"));
        }

        [Test]
        public void can_resolve_a_generic_type()
        {
            container.Register(typeof(ITestService<>), typeof(TestService<>));

            var instance = container.GetInstance<ITestService<int>>();

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.InstanceOf<TestService<int>>());
        }

        [Test]
        public void can_resolve_a_generic_type_by_key()
        {
            container.Register(typeof(ITestService<>), typeof(TestService<>), "test");

            var instance = container.GetInstance<ITestService<int>>("test");

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.InstanceOf<TestService<int>>());
        }

        [Test]
        public void can_resolve_a_service_by_key()
        {
            container.Register<TestService>("test");

            var instance = container.GetInstance("test");

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.AssignableFrom(typeof(TestService)));
        }

        [Test]
        public void can_resolve_a_service_by_type()
        {
            container.Register<ITestService, TestService>();

            var instance = container.GetInstance(typeof(ITestService));

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.AssignableFrom(typeof(TestService)));
        }

        [Test]
        public void can_resolve_a_service_using_generics()
        {
            container.Register<ITestService, TestService>();

            var instance = container.GetInstance<ITestService>();

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.AssignableFrom(typeof(TestService)));
        }

        [Test]
        public void can_resolve_a_singleton_by_key()
        {
            container.RegisterSingleton<TestService>("test");

            var instance1 = container.GetInstance("test");
            var instance2 = container.GetInstance("test");

            Assert.That(instance1, Is.Not.Null);
            Assert.That(instance1, Is.AssignableFrom(typeof(TestService)));

            Assert.That(instance2, Is.Not.Null);
            Assert.That(instance2, Is.AssignableFrom(typeof(TestService)));

            Assert.That(instance1, Is.EqualTo(instance2));
        }

        [Test]
        public void can_resolve_a_singleton_by_type()
        {
            container.RegisterSingleton<ITestService, TestService>();

            var instance1 = container.GetInstance(typeof(ITestService));
            var instance2 = container.GetInstance(typeof(ITestService));

            Assert.That(instance1, Is.Not.Null);
            Assert.That(instance1, Is.AssignableFrom(typeof(TestService)));

            Assert.That(instance2, Is.Not.Null);
            Assert.That(instance2, Is.AssignableFrom(typeof(TestService)));

            Assert.That(instance1, Is.EqualTo(instance2));
        }

        [Test]
        public void can_resolve_a_singleton_using_generics()
        {
            container.RegisterSingleton<ITestService, TestService>();

            var instance1 = container.GetInstance<ITestService>();
            var instance2 = container.GetInstance<ITestService>();

            Assert.That(instance1, Is.Not.Null);
            Assert.That(instance1, Is.AssignableFrom(typeof(TestService)));

            Assert.That(instance2, Is.Not.Null);
            Assert.That(instance2, Is.AssignableFrom(typeof(TestService)));

            Assert.That(instance1, Is.EqualTo(instance2));
        }
    }
}