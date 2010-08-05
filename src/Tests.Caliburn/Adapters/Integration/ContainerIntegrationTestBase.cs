using System.Collections;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Tests.Caliburn.Adapters.Components;

namespace Tests.Caliburn.Adapters.Integration
{
	using global::Caliburn.Core.IoC;
	using NUnit.Framework.SyntaxHelpers;

	public abstract class ContainerIntegrationTestBase : TestBase
	{
		protected IServiceLocator locator;
		protected IRegistry registry;


		protected override void given_the_context_of()
		{
			var container = CreateContainerAdapter();
			locator = container;
			registry = container;
		}


		protected abstract IContainer CreateContainerAdapter();

		protected Singleton Singleton<TService, TImplementation>()
		   where TImplementation : TService
		{
			return new Singleton(typeof(TService)) { Implementation = typeof(TImplementation) };
		}
		protected PerRequest PerRequest<TService, TImplementation>()
			where TImplementation : TService
		{
			return new PerRequest(typeof(TService)) { Implementation = typeof(TImplementation) };
		}
		protected Instance Instance<TService>(TService instance)
		{
			return new Instance { Service = typeof(TService), Implementation = instance };
		}



		[Test]
		public void can_register_PerRequest()
		{
			registry.Register(new[] { PerRequest<ILogger, SimpleLogger>() });

			ILogger instance1 = locator.GetInstance<ILogger>();
			Assert.IsNotNull(instance1);

			ILogger instance2 = locator.GetInstance<ILogger>();
			Assert.IsNotNull(instance2);

			Assert.AreNotSame(instance1, instance2);
		}

		[Test]
		public void can_register_Singleton()
		{
			registry.Register(new[] { Singleton<ILogger, SimpleLogger>() });

			ILogger instance1 = locator.GetInstance<ILogger>();
			Assert.IsNotNull(instance1);

			ILogger instance2 = locator.GetInstance<ILogger>();
			Assert.IsNotNull(instance2);

			Assert.AreSame(instance1, instance2);
		}

		[Test]
		public void can_register_Instance()
		{
			ILogger theLogger = new SimpleLogger();
			registry.Register(new[] { Instance<ILogger>(theLogger) });

			ILogger instance1 = locator.GetInstance<ILogger>();
			Assert.IsNotNull(instance1);
			Assert.AreSame(instance1, theLogger);

			ILogger instance2 = locator.GetInstance<ILogger>();
			Assert.IsNotNull(instance2);
			Assert.AreSame(instance2, theLogger);
		}


		[Test]
		public void can_inject_dependencies_on_constructor()
		{
			registry.Register(new[] { 
				PerRequest<ILogger, SimpleLogger>(),
				PerRequest<IMailer, SimpleMailer>() 
			});

			var instance = locator.GetInstance<IMailer>() as SimpleMailer;
			Assert.IsNotNull(instance.Logger);
			Assert.IsInstanceOfType(typeof(SimpleLogger), instance.Logger);
		}

		[Test]
		public virtual void can_inject_dependencies_on_public_properties()
		{
			registry.Register(new[] { 
				PerRequest<ILogger, SimpleLogger>(),
				PerRequest<ISampleCommand, SampleCommand>() 
			});

			var instance = locator.GetInstance<ISampleCommand>() as SampleCommand;
			Assert.IsNotNull(instance.Logger);
			Assert.IsInstanceOfType(typeof(SimpleLogger), instance.Logger);
		} 
	}
}