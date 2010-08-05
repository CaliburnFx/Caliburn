using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.ComponentModel.Composition.Hosting;
using Caliburn.MEF;
using Tests.Caliburn.Adapters.Components;
using Caliburn.Core.Behaviors;
using Rhino.Mocks;
using Microsoft.Practices.ServiceLocation;

namespace Tests.Caliburn.Adapters.Integration
{
	public class The_MEF_container
	{
		[TestFixture]
		public class without_setter_injection : ContainerIntegrationTestBase
		{
			protected override global::Caliburn.Core.IoC.IContainer CreateContainerAdapter()
			{
				var container = new CompositionContainer();
				return new MEFAdapter(container);
			}

			[Test]
			[Ignore("Not applicable with setter injection turned off")]
			public override void can_inject_dependencies_on_public_properties() { }

			[Test]
			public void doesnt_inject_dependencies_on_public_properties()
			{
				registry.Register(new[] { 
				PerRequest<ILogger, SimpleLogger>(),
				PerRequest<ISampleCommand, SampleCommand>() 
			});

				var instance = locator.GetInstance<ISampleCommand>() as SampleCommand;
				Assert.IsNull(instance.Logger);
			}
		}


		[TestFixture]
		public class with_setter_injection : ContainerIntegrationTestBase
		{
			protected override global::Caliburn.Core.IoC.IContainer CreateContainerAdapter()
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
			protected override global::Caliburn.Core.IoC.IContainer CreateContainerAdapter()
			{
				var container = new CompositionContainer();
				var adapter = new MEFAdapter(container, true).WithProxyFactory<StubProxyFactory>();
				ServiceLocator.SetLocatorProvider(()=> adapter);
				return adapter;
			}

			protected override void after_each()
			{
				ServiceLocator.SetLocatorProvider(null);
			} 

			[Test]
			public override void can_inject_dependencies_on_public_properties()
			{
				registry.Register(new[] { 
					PerRequest<ILogger, SimpleLogger>(),
					PerRequest<ISampleCommand, SampleCommand>() 
				});

				var instance = locator.GetInstance<ISampleCommand>() as SampleCommand;
				Assert.IsInstanceOfType(typeof(StubProxyFactory.SampleCommandProxy), instance);
				Assert.IsNotNull(instance.Logger);
				Assert.IsInstanceOfType(typeof(SimpleLogger), instance.Logger);

			}


			private class StubProxyFactory : IProxyFactory
			{
				public object CreateProxyWithTarget(Type interfaceType, object target, IEnumerable<IBehavior> behaviors)
				{
					if (target is SampleCommand)
						return new SampleCommandProxy();
					else
						return target;
				}

				public object CreateProxy(Type type, IEnumerable<IBehavior> behaviors, IEnumerable<object> constructorArgs)
				{	
					if (type.Equals(typeof(SampleCommand)))
						return new SampleCommandProxy();
					else
						return Activator.CreateInstance(type, constructorArgs.ToArray());
				}

				public class SampleCommandProxy: SampleCommand {}
			}

		}

	}
}
