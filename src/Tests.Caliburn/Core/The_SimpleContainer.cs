using System;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Tests.Caliburn.Fakes;

namespace Tests.Caliburn.Core
{
    using global::Caliburn.Core.IoC;

    [TestFixture]
    public class The_SimpleContainer
    {
        private SimpleContainer _container;

        [SetUp]
        public void SetUp()
        {
            _container = new SimpleContainer();
        }

        [Test]
        public void can_add_a_type_handler()
        {
            _container.AddHandler(typeof(ITestService), () => new TestService());

            Assert.That(_container.IsRegistered(typeof(ITestService)));
        }

        [Test]
        public void can_locate_a_type_handler()
        {
            Func<object> handler = () => new TestService();

            _container.AddHandler(typeof(ITestService), handler);

            var found = _container.GetHandler(typeof(ITestService));

            Assert.That(found, Is.EqualTo(handler));
        }

        [Test]
        public void can_register_a_service_by_type()
        {
            _container.Register(typeof(ITestService), typeof(TestService));

            Assert.That(_container.IsRegistered(typeof(ITestService)));
        }

        [Test]
        public void can_register_a_service_using_generics()
        {
            _container.Register<ITestService, TestService>();

            Assert.That(_container.IsRegistered(typeof(ITestService)));
        }

        [Test]
        public void can_register_a_singleton_by_type()
        {
            _container.RegisterSingleton(typeof(ITestService), typeof(TestService));

            Assert.That(_container.IsRegistered(typeof(ITestService)));
        }

        [Test]
        public void can_register_a_singleton_using_generics()
        {
            _container.RegisterSingleton<ITestService, TestService>();

            Assert.That(_container.IsRegistered(typeof(ITestService)));
        }

        [Test]
        public void can_resolve_a_service_by_type()
        {
            _container.Register<ITestService, TestService>();

            var instance = _container.GetInstance(typeof(ITestService));

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.AssignableFrom(typeof(TestService)));
        }

        [Test]
        public void can_resolve_a_service_using_generics()
        {
            _container.Register<ITestService, TestService>();

            var instance = _container.GetInstance<ITestService>();

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.AssignableFrom(typeof(TestService)));
        }

        [Test]
        public void can_resolve_a_singleton_by_type()
        {
            _container.RegisterSingleton<ITestService, TestService>();

            var instance1 = _container.GetInstance(typeof(ITestService));
            var instance2 = _container.GetInstance(typeof(ITestService));

            Assert.That(instance1, Is.Not.Null);
            Assert.That(instance1, Is.AssignableFrom(typeof(TestService)));

            Assert.That(instance2, Is.Not.Null);
            Assert.That(instance2, Is.AssignableFrom(typeof(TestService)));

            Assert.That(instance1, Is.EqualTo(instance2));
        }

        [Test]
        public void can_resolve_a_singleton_using_generics()
        {
            _container.RegisterSingleton<ITestService, TestService>();

            var instance1 = _container.GetInstance<ITestService>();
            var instance2 = _container.GetInstance<ITestService>();

            Assert.That(instance1, Is.Not.Null);
            Assert.That(instance1, Is.AssignableFrom(typeof(TestService)));

            Assert.That(instance2, Is.Not.Null);
            Assert.That(instance2, Is.AssignableFrom(typeof(TestService)));

            Assert.That(instance1, Is.EqualTo(instance2));
        }

        [Test]
        public void can_fill_dependencies_using_the_greediest_constructor()
        {
            _container.Register<ITestService, TestService>();
            _container.Register<IDependentService, DependentService>();

            var instance = _container.GetInstance<IDependentService>();

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.AssignableFrom(typeof(DependentService)));
            Assert.That(instance.Dependency, Is.Not.Null);
            Assert.That(instance.Dependency, Is.AssignableFrom(typeof(TestService)));
        }

        [Test]
        public void can_create_concrete_types_without_registration()
        {
            var instance = _container.GetInstance<TestService>();

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.AssignableFrom(typeof(TestService)));
        }

        [Test]
        public void automatically_registers_itself()
        {
            var container = _container.GetInstance<IServiceLocator>();

            Assert.That(_container, Is.EqualTo(container));
        }

        [Test]
        public void can_register_a_service_by_key()
        {
            _container.Register("test", typeof(TestService));

            Assert.That(_container.IsRegistered("test"));
        }

        [Test]
        public void can_register_a_service_using_key_and_generics()
        {
            _container.Register<TestService>("test");

            Assert.That(_container.IsRegistered("test"));
        }

        [Test]
        public void can_register_a_singleton_by_key()
        {
            _container.RegisterSingleton("test", typeof(TestService));

            Assert.That(_container.IsRegistered("test"));
        }

        [Test]
        public void can_register_a_singleton_using_key_and_generics()
        {
            _container.RegisterSingleton<TestService>("test");

            Assert.That(_container.IsRegistered("test"));
        }

        [Test]
        public void can_resolve_a_service_by_key()
        {
            _container.Register<TestService>("test");

            var instance = _container.GetInstance("test");

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.AssignableFrom(typeof(TestService)));
        }

        [Test]
        public void can_resolve_a_singleton_by_key()
        {
            _container.RegisterSingleton<TestService>("test");

            var instance1 = _container.GetInstance("test");
            var instance2 = _container.GetInstance("test");

            Assert.That(instance1, Is.Not.Null);
            Assert.That(instance1, Is.AssignableFrom(typeof(TestService)));

            Assert.That(instance2, Is.Not.Null);
            Assert.That(instance2, Is.AssignableFrom(typeof(TestService)));

            Assert.That(instance1, Is.EqualTo(instance2));
        }

		[Test]
		public void can_resolve_a_generic_type()
		{
			_container.Register(typeof(ITestService<>), typeof(TestService<>));

			var instance = _container.GetInstance<ITestService<int>>();

			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.InstanceOfType(typeof(TestService<int>)));
		}

		[Test]
		public void can_resolve_a_generic_type_by_key()
		{
			_container.Register(typeof(ITestService<>), typeof(TestService<>), "test");

			var instance = _container.GetInstance<ITestService<int>>("test");

			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.InstanceOfType(typeof(TestService<int>)));
		}
    }
}