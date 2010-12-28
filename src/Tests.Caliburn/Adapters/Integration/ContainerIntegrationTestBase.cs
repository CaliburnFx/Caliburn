namespace Tests.Caliburn.Adapters.Integration
{
    using Components;
    using global::Caliburn.Core.InversionOfControl;
    using NUnit.Framework;

    public abstract class ContainerIntegrationTestBase : TestBase
    {
        protected IServiceLocator Locator;
        protected IRegistry Registry;

        protected override void given_the_context_of()
        {
            var container = CreateContainerAdapter();
            Locator = container;
            Registry = container;
			IoC.Initialize(Locator);
        }

        protected abstract IContainer CreateContainerAdapter();

        protected Singleton Singleton<TService, TImplementation>()
            where TImplementation : TService
        {
            return new Singleton(typeof(TService)) {
                Implementation = typeof(TImplementation)
            };
        }

        protected PerRequest PerRequest<TService, TImplementation>()
            where TImplementation : TService
        {
            return new PerRequest(typeof(TService)) {
                Implementation = typeof(TImplementation)
            };
        }

        protected Instance Instance<TService>(TService instance)
        {
            return new Instance {
                Service = typeof(TService), Implementation = instance
            };
        }

        [Test]
        public void can_register_PerRequest()
        {
            Registry.Register(new[] {
                PerRequest<ILogger, SimpleLogger>()
            });

            var instance1 = Locator.GetInstance<ILogger>();
            Assert.IsNotNull(instance1);

            var instance2 = Locator.GetInstance<ILogger>();
            Assert.IsNotNull(instance2);

            Assert.AreNotSame(instance1, instance2);
        }

        [Test]
        public void can_register_Singleton()
        {
            Registry.Register(new[] {
                Singleton<ILogger, SimpleLogger>()
            });

            var instance1 = Locator.GetInstance<ILogger>();
            Assert.IsNotNull(instance1);

            var instance2 = Locator.GetInstance<ILogger>();
            Assert.IsNotNull(instance2);

            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void can_register_Instance()
        {
            ILogger theLogger = new SimpleLogger();
            Registry.Register(new[] {
                Instance<ILogger>(theLogger)
            });

            var instance1 = Locator.GetInstance<ILogger>();
            Assert.IsNotNull(instance1);
            Assert.AreSame(instance1, theLogger);

            var instance2 = Locator.GetInstance<ILogger>();
            Assert.IsNotNull(instance2);
            Assert.AreSame(instance2, theLogger);
        }


        [Test]
        public void can_inject_dependencies_on_constructor()
        {
            Registry.Register(new[] {
                PerRequest<ILogger, SimpleLogger>(),
                PerRequest<IMailer, SimpleMailer>()
            });

            var instance = Locator.GetInstance<IMailer>() as SimpleMailer;
            Assert.IsNotNull(instance.Logger);
            Assert.IsInstanceOf<SimpleLogger>(instance.Logger);
        }

        [Test]
        public virtual void can_inject_dependencies_on_public_properties()
        {
            Registry.Register(new[] {
                PerRequest<ILogger, SimpleLogger>(),
                PerRequest<ISampleCommand, SampleCommand>()
            });

            var instance = Locator.GetInstance<ISampleCommand>() as SampleCommand;
            Assert.IsNotNull(instance.Logger);
            Assert.IsInstanceOf<SimpleLogger>(instance.Logger);
        }
    }
}