namespace Tests.Caliburn.Adapters.Integration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using Components;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.Core.Behaviors;
    using global::Caliburn.MEF;
    using NUnit.Framework;

    public class The_MEF_container
    {
        [TestFixture]
        public class with_setter_injection : ContainerIntegrationTestBase
        {
            protected override IContainer CreateContainerAdapter()
            {
                var container = new CompositionContainer();
                return new MEFAdapter(container, true);
            }

            [Test]
            public override void can_inject_dependencies_on_public_properties()
            {
                base.can_inject_dependencies_on_public_properties();
            }
        }


        [TestFixture]
        public class with_setter_injection_and_proxies : ContainerIntegrationTestBase
        {
            protected override IContainer CreateContainerAdapter()
            {
                var container = new CompositionContainer();
                var adapter = new MEFAdapter(container, true).WithProxyFactory<StubProxyFactory>();
                IoC.Initialize(adapter);
                return adapter;
            }

            protected override void after_each()
            {
                IoC.Initialize(null);
            }


            class StubProxyFactory : IProxyFactory
            {
                public object CreateProxyWithTarget(Type interfaceType, object target, IEnumerable<IBehavior> behaviors)
                {
                    if(target is SampleCommand)
                        return new SampleCommandProxy();
                    else
                        return target;
                }

                public object CreateProxy(Type type, IEnumerable<IBehavior> behaviors, IEnumerable<object> constructorArgs)
                {
                    if(type.Equals(typeof(SampleCommand)))
                        return new SampleCommandProxy();
                    else
                        return Activator.CreateInstance(type, constructorArgs.ToArray());
                }

                public class SampleCommandProxy : SampleCommand {}
            }

            [Test]
            public override void can_inject_dependencies_on_public_properties()
            {
                Registry.Register(new[] {
                    PerRequest<ILogger, SimpleLogger>(),
                    PerRequest<ISampleCommand, SampleCommand>()
                });

                var instance = Locator.GetInstance<ISampleCommand>() as SampleCommand;
                Assert.IsInstanceOf<StubProxyFactory.SampleCommandProxy>(instance);
                Assert.IsNotNull(instance.Logger);
                Assert.IsInstanceOf<SimpleLogger>(instance.Logger);
            }
        }

        [TestFixture]
        public class without_setter_injection : ContainerIntegrationTestBase
        {
            protected override IContainer CreateContainerAdapter()
            {
                var container = new CompositionContainer();
                return new MEFAdapter(container);
            }

            [Test, Ignore("Not applicable with setter injection turned off")]
			
            public override void can_inject_dependencies_on_public_properties() {}

            [Test]
            public void doesnt_inject_dependencies_on_public_properties()
            {
                Registry.Register(new[] {
                    PerRequest<ILogger, SimpleLogger>(),
                    PerRequest<ISampleCommand, SampleCommand>()
                });

                var instance = Locator.GetInstance<ISampleCommand>() as SampleCommand;
                Assert.IsNull(instance.Logger);
            }
        }
    }
}