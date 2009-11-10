using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Caliburn.Core;
using Caliburn.Core.Invocation;
using Caliburn.Core.Threading;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Tests.Caliburn.Fakes;

namespace Tests.Caliburn.Core
{
    using global::Caliburn.Core.IoC;

    [TestFixture]
    public class The_core_configuration : TestBase
    {
        private IServiceLocator _container;

        protected override void given_the_context_of()
        {
            _container = Mock<IServiceLocator>();
            _container.Stub(x => x.GetInstance<IDispatcher>())
                .Return(new Execute.SimpleDispatcher()).Repeat.Any();
            _container.Stub(x => x.GetInstance<IAssemblySource>())
                .Return(new DefaultAssemblySource()).Repeat.Any();
        }

        [Test]
        public void is_a_configuration_hook()
        {
            var config = new CoreConfiguration(_container, delegate { });

            Assert.That(config, Is.InstanceOfType(typeof(IConfigurationHook)));
        }

        [Test]
        public void has_a_container_reference()
        {
            var config = new CoreConfiguration(_container, delegate { });

            Assert.That(config.ServiceLocator, Is.EqualTo(_container));
        }

        [Test]
        public void when_started_configures_required_components_and_children()
        {
            bool configuratorWasCalled = false;

            var config = new CoreConfiguration(
                _container,
                infos => {
                    configuratorWasCalled = true;

                    var found = (from info in infos
                                 where info.Service == typeof(IDispatcher)
                                 select info).FirstOrDefault();

                    Assert.That(found, Is.Not.Null);

                    found = (from info in infos
                             where info.Service == typeof(IThreadPool)
                             select info).FirstOrDefault();

                    Assert.That(found, Is.Not.Null);

                    found = (from info in infos
                             where info.Service == typeof(IMethodFactory)
                             select info).FirstOrDefault();

                    Assert.That(found, Is.Not.Null);

                    found = (from info in infos
                             where info.Service == typeof(IEventHandlerFactory)
                             select info).FirstOrDefault();

                    Assert.That(found, Is.Not.Null);
                });

            var child = new FakeConfig(config);
            child.Start();

            Assert.That(configuratorWasCalled);
            Assert.That(child.GetComponentsWasCalled);
            Assert.That(child.ConfigureWasCalled);
        }

        [Test]
        public void can_provide_a_custom_dispatcher()
        {
            bool configuratorWasCalled = false;

            var config = new CoreConfiguration(
                _container,
                infos =>
                {
                    configuratorWasCalled = true;

                    var found = (from info in infos.OfType<Singleton>()
                                 where info.Service == typeof(IDispatcher)
                                 select info).FirstOrDefault();

                    Assert.That(found.Implementation, Is.EqualTo(typeof(Execute.SimpleDispatcher)));
                });

            config.UsingDispatcher<Execute.SimpleDispatcher>();
            config.Start();

            Assert.That(configuratorWasCalled);
        }

        [Test]
        public void can_provide_a_custom_thread_pool()
        {
            bool configuratorWasCalled = false;

            var config = new CoreConfiguration(
                _container,
                infos =>
                {
                    configuratorWasCalled = true;

                    var found = (from info in infos.OfType<Singleton>()
                                 where info.Service == typeof(IThreadPool)
                                 select info).FirstOrDefault();

                    Assert.That(found.Implementation, Is.EqualTo(typeof(FakeThreadPool)));
                });

            config.UsingThreadPool<FakeThreadPool>();
            config.Start();

            Assert.That(configuratorWasCalled);
        }

        [Test]
        public void can_provide_a_custom_method_factory()
        {
            bool configuratorWasCalled = false;

            var config = new CoreConfiguration(
                _container,
                infos =>
                {
                    configuratorWasCalled = true;

                    var found = (from info in infos.OfType<Singleton>()
                                 where info.Service == typeof(IMethodFactory)
                                 select info).FirstOrDefault();

                    Assert.That(found.Implementation, Is.EqualTo(typeof(FakeMethodFactory)));
                });

            config.UsingMethodFactory<FakeMethodFactory>();
            config.Start();

            Assert.That(configuratorWasCalled);
        }

        [Test]
        public void can_provide_a_custom_event_handler_factory()
        {
            bool configuratorWasCalled = false;

            var config = new CoreConfiguration(
                _container,
                infos =>
                {
                    configuratorWasCalled = true;

                    var found = (from info in infos.OfType<Singleton>()
                                 where info.Service == typeof(IEventHandlerFactory)
                                 select info).FirstOrDefault();

                    Assert.That(found.Implementation, Is.EqualTo(typeof(FakeEventHandlerFactory)));
                });

            config.UsingEventHandlerFactory<FakeEventHandlerFactory>();
            config.Start();

            Assert.That(configuratorWasCalled);
        }

        private class FakeConfig : CaliburnModule
        {
            public FakeConfig(IConfigurationHook hook) 
                : base(hook) {}

            public bool GetComponentsWasCalled { get; set; }
            public bool ConfigureWasCalled { get; set; }

            protected override IEnumerable<IComponentRegistration> GetComponents()
            {
                GetComponentsWasCalled = true;
                yield break;
            }

            protected override void Initialize()
            {
                ConfigureWasCalled = true;
            }
        }

        private class FakeMethodFactory : IMethodFactory
        {
            public IMethod CreateFrom(MethodInfo methodInfo)
            {
                throw new System.NotImplementedException();
            }
        }

        private class FakeEventHandlerFactory : IEventHandlerFactory
        {
            public IEventHandler Wire(object sender, string eventName)
            {
                throw new System.NotImplementedException();
            }

            public IEventHandler Wire(object sender, EventInfo eventInfo)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}