using System.ComponentModel.Composition.Hosting;
using Caliburn.MEF;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Tests.Caliburn.Adapters.Components;

namespace Tests.Caliburn.Adapters.ServiceLocation
{
	using System.ComponentModel;
	using System.ComponentModel.Composition;
	using System.Reflection;
	using global::Caliburn.Core.IoC;
	using global::Caliburn.DynamicProxy;
	using global::Caliburn.PresentationFramework.Behaviors;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
    public class The_MEF_container : ServiceLocatorTests
    {
        protected override IServiceLocator CreateServiceLocator()
        {
            var catalog = new TypeCatalog(typeof(SimpleLogger), typeof(AdvancedLogger));
            var container = new CompositionContainer(catalog);
            return new MEFAdapter(container);
        }

		[Test]
		public void proxycatalog_should_replace_parts()
		{
			var catalog = new AssemblyCatalog(Assembly.GetAssembly(typeof(ExportedClass)));
			var proxycatalog = new ProxyCatalog(catalog);
			var container = new CompositionContainer(proxycatalog);

			var adapter = new MEFAdapter(container).WithProxyFactory<DynamicProxyFactory>();
			ServiceLocator.SetLocatorProvider(() => adapter);

			var vm = container.GetExportedValue<ExportedClass>();
			Assert.That(vm, Is.InstanceOfType(typeof(INotifyPropertyChanged)));
		}

		[Test]
		public void can_register_through_adapter_using_proxyfactory()
		{
			var catalog = new AssemblyCatalog(Assembly.GetAssembly(typeof(ClassWithBehaviour)));
			var proxycatalog = new ProxyCatalog(catalog);
			var container = new CompositionContainer(proxycatalog);

			var adapter = new MEFAdapter(container).WithProxyFactory<DynamicProxyFactory>();
			ServiceLocator.SetLocatorProvider(() => adapter);

			adapter.Register(new[] { new PerRequest { Service = typeof(ClassWithBehaviour), Implementation = typeof(ClassWithBehaviour) } });

			var vm = container.GetExportedValue<ClassWithBehaviour>();
			Assert.That(vm, Is.InstanceOfType(typeof(INotifyPropertyChanged)));
		}

		[Test]
		public void register_through_adapter_using_proxyfactory_can_resolve_singletons()
		{
			var catalog = new AssemblyCatalog(Assembly.GetAssembly(typeof(ClassWithBehaviour)));
			var proxycatalog = new ProxyCatalog(catalog);
			var container = new CompositionContainer(proxycatalog);

			var adapter = new MEFAdapter(container).WithProxyFactory<DynamicProxyFactory>();
			ServiceLocator.SetLocatorProvider(() => adapter);

			adapter.Register(new[] { new Singleton() { Service = typeof(ClassWithBehaviour), Implementation = typeof(ClassWithBehaviour) } });

			var sl = (IServiceLocator) adapter;
			var instance1 = sl.GetInstance<ClassWithBehaviour>();
			var instance2 = sl.GetInstance<ClassWithBehaviour>();

			Assert.AreSame(instance1, instance2);
		}

		[Test]
		public void register_through_adapter_using_proxyfactory_can_resolve_perrequest()
		{
			var catalog = new AssemblyCatalog(Assembly.GetAssembly(typeof(ClassWithBehaviour)));
			var proxycatalog = new ProxyCatalog(catalog);
			var container = new CompositionContainer(proxycatalog);

			var adapter = new MEFAdapter(container).WithProxyFactory<DynamicProxyFactory>();
			ServiceLocator.SetLocatorProvider(() => adapter);

			adapter.Register(new[] { new PerRequest() { Service = typeof(ClassWithBehaviour), Implementation = typeof(ClassWithBehaviour) } });

			var sl = (IServiceLocator)adapter;
			var instance1 = sl.GetInstance<ClassWithBehaviour>();
			var instance2 = sl.GetInstance<ClassWithBehaviour>();

			Assert.AreNotSame(instance1, instance2);
		}

		[Test]
		public void ProxyCatalog_NotifyChanging_does_not_throw()
		{
			var aggregatecatalog = new AggregateCatalog();

			var proxycatalog = new ProxyCatalog(aggregatecatalog);
			
			// we didn't access proxycatalog.Parts, so _parts is null and the following throws
			aggregatecatalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));			
		}

		[NotifyPropertyChanged]
		public class ClassWithBehaviour
		{ }

		[Export(typeof(ExportedClass))]
		[NotifyPropertyChanged]
		public class ExportedClass
		{ }
	}
}