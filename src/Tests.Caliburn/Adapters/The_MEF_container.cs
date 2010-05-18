using System.ComponentModel.Composition.Hosting;
using Caliburn.MEF;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Tests.Caliburn.Adapters.Components;

namespace Tests.Caliburn.Adapters
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

		[NotifyPropertyChanged]
		public class ClassWithBehaviour
		{ }

		[Export(typeof(ExportedClass))]
		[NotifyPropertyChanged]
		public class ExportedClass
		{ }
	}
}