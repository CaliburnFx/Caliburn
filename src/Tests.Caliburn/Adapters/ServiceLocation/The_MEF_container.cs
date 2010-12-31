namespace Tests.Caliburn.Adapters.ServiceLocation
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Reflection;
    using Components;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.DynamicProxy;
    using global::Caliburn.MEF;
    using global::Caliburn.PresentationFramework.Behaviors;
    using NUnit.Framework;

    [TestFixture]
    public class The_MEF_container : ServiceLocatorTests
    {
        protected override IServiceLocator CreateServiceLocator()
        {
            var catalog = new TypeCatalog(typeof(SimpleLogger), typeof(AdvancedLogger));
            var container = new CompositionContainer(catalog);
            return new MEFAdapter(container);
        }

        [NotifyPropertyChanged]
        public class ClassWithBehaviour
        {
            [Import]
            public SharedExportedClass Shared { get; set; }
        }

        [Export(typeof(ExportedClass)), NotifyPropertyChanged]
        public class ExportedClass {}

        [Export(typeof(SharedExportedClass)), NotifyPropertyChanged, PartCreationPolicy(CreationPolicy.Shared)]
        public class SharedExportedClass {}

        [Export(typeof(ExportedClassWithProperties)), NotifyPropertyChanged, PartCreationPolicy(CreationPolicy.Shared)]
        public class ExportedClassWithProperties
        {
            public ExportedClassWithProperties()
            {
                Multiple = new List<ILogger>();
            }

            [Import]
            public SharedExportedClass Shared { get; set; }

            [ImportMany]
            public IList<ILogger> Multiple { get; set; }
        }

        [Export(typeof(ExportedClassWithoutPolicy)), NotifyPropertyChanged]
        public class ExportedClassWithoutPolicy {}

        [Test]
        public void Can_ImportMany()
        {
            var catalog = new AssemblyCatalog(Assembly.GetAssembly(typeof(SharedExportedClass)));
            var container = new CompositionContainer(catalog);

            var adapter = new MEFAdapter(container);
            IoC.Initialize(adapter);

            var instance1 = adapter.GetInstance<ExportedClassWithProperties>();

            Assert.That(instance1.Multiple.Count, Is.EqualTo(2));
        }

        [Test]
        public void Can_ImportMany_with_ProxyCatalog()
        {
            var catalog = new AssemblyCatalog(Assembly.GetAssembly(typeof(SharedExportedClass)));
            var proxycatalog = new ProxyCatalog(catalog);
            var container = new CompositionContainer(proxycatalog);

            var adapter = new MEFAdapter(container).WithProxyFactory<DynamicProxyFactory>();
            IoC.Initialize(adapter);

            var instance = adapter.GetInstance<ExportedClassWithProperties>();

            Assert.That(instance.Multiple.Count, Is.EqualTo(2));
        }

        [Test]
        public void Can_use_setter_injection_with_proxycatalog_register_trough_attributes()
        {
            var catalog = new AssemblyCatalog(Assembly.GetAssembly(typeof(SharedExportedClass)));
            var proxycatalog = new ProxyCatalog(catalog);
            var container = new CompositionContainer(proxycatalog);

            var adapter = new MEFAdapter(container).WithProxyFactory<DynamicProxyFactory>();
            IoC.Initialize(adapter);

            var shared = adapter.GetInstance<SharedExportedClass>();
            var instance1 = adapter.GetInstance<ExportedClassWithProperties>();
            var instance2 = adapter.GetInstance<ExportedClassWithProperties>();

            Assert.AreSame(instance1, instance2);
            Assert.That(shared, Is.Not.Null);
            Assert.That(instance1.Shared, Is.SameAs(shared));
            Assert.That(instance2.Shared, Is.SameAs(shared));
        }

        [Test]
        public void Can_use_setter_injection_with_proxycatalog_register_trough_container()
        {
            var catalog = new AssemblyCatalog(Assembly.GetAssembly(typeof(SharedExportedClass)));
            var proxycatalog = new ProxyCatalog(catalog);
            var container = new CompositionContainer(proxycatalog);

            var adapter = new MEFAdapter(container).WithProxyFactory<DynamicProxyFactory>();
            adapter.Register(new[] {
                new Singleton {
                    Service = typeof(ClassWithBehaviour), Implementation = typeof(ClassWithBehaviour)
                }
            });
            IoC.Initialize(adapter);

            var shared = adapter.GetInstance<SharedExportedClass>();
            var instance1 = adapter.GetInstance<ClassWithBehaviour>();
            var instance2 = adapter.GetInstance<ClassWithBehaviour>();

            Assert.AreSame(instance1, instance2);
            Assert.That(shared, Is.Not.Null);
            Assert.That(instance1.Shared, Is.SameAs(shared));
            Assert.That(instance2.Shared, Is.SameAs(shared));
        }

        [Test]
        public void NOproxycatalog_Default_CreationPolicy_is_shared()
        {
            var catalog = new AssemblyCatalog(Assembly.GetAssembly(typeof(SharedExportedClass)));
            var container = new CompositionContainer(catalog);

            var adapter = new MEFAdapter(container);
            IoC.Initialize(adapter);

            var instance1 = adapter.GetInstance<ExportedClassWithoutPolicy>();
            var instance2 = adapter.GetInstance<ExportedClassWithoutPolicy>();

            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void ProxyCatalog_NotifyChanging_does_not_throw()
        {
            var aggregatecatalog = new AggregateCatalog();

            var proxycatalog = new ProxyCatalog(aggregatecatalog);

            // we didn't access proxycatalog.Parts, so _parts is null and the following throws
            aggregatecatalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
        }

        [Test]
        public void can_register_through_adapter_using_proxyfactory()
        {
            var catalog = new AssemblyCatalog(Assembly.GetAssembly(typeof(ClassWithBehaviour)));
            var proxycatalog = new ProxyCatalog(catalog);
            var container = new CompositionContainer(proxycatalog);

            var adapter = new MEFAdapter(container).WithProxyFactory<DynamicProxyFactory>();
            IoC.Initialize(adapter);

            adapter.Register(new[] {
                new PerRequest {
                    Service = typeof(ClassWithBehaviour), Implementation = typeof(ClassWithBehaviour)
                }
            });

            var vm = container.GetExportedValue<ClassWithBehaviour>();
            Assert.That(vm, Is.InstanceOf<INotifyPropertyChanged>());
        }

        [Test]
        public void proxycatalog_Default_CreationPolicy_is_shared()
        {
            var catalog = new AssemblyCatalog(Assembly.GetAssembly(typeof(SharedExportedClass)));
            var proxycatalog = new ProxyCatalog(catalog);
            var container = new CompositionContainer(proxycatalog);

            var adapter = new MEFAdapter(container).WithProxyFactory<DynamicProxyFactory>();
            IoC.Initialize(adapter);

            var instance1 = adapter.GetInstance<ExportedClassWithoutPolicy>();
            var instance2 = adapter.GetInstance<ExportedClassWithoutPolicy>();

            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void proxycatalog_should_replace_parts()
        {
            var catalog = new AssemblyCatalog(Assembly.GetAssembly(typeof(ExportedClass)));
            var proxycatalog = new ProxyCatalog(catalog);
            var container = new CompositionContainer(proxycatalog);

            var adapter = new MEFAdapter(container).WithProxyFactory<DynamicProxyFactory>();
            IoC.Initialize(adapter);

            var vm = container.GetExportedValue<ExportedClass>();
            Assert.That(vm, Is.InstanceOf<INotifyPropertyChanged>());
        }

        [Test]
        public void register_through_adapter_using_proxyfactory_can_resolve_perrequest()
        {
            var catalog = new AssemblyCatalog(Assembly.GetAssembly(typeof(ClassWithBehaviour)));
            var proxycatalog = new ProxyCatalog(catalog);
            var container = new CompositionContainer(proxycatalog);

            var adapter = new MEFAdapter(container).WithProxyFactory<DynamicProxyFactory>();
            IoC.Initialize(adapter);

            adapter.Register(new[] {
                new PerRequest {
                    Service = typeof(ClassWithBehaviour), Implementation = typeof(ClassWithBehaviour)
                }
            });

            var sl = (IServiceLocator)adapter;
            var instance1 = sl.GetInstance<ClassWithBehaviour>();
            var instance2 = sl.GetInstance<ClassWithBehaviour>();

            Assert.AreNotSame(instance1, instance2);
        }

        [Test]
        public void register_through_adapter_using_proxyfactory_can_resolve_singletons()
        {
            var catalog = new AssemblyCatalog(Assembly.GetAssembly(typeof(ClassWithBehaviour)));
            var proxycatalog = new ProxyCatalog(catalog);
            var container = new CompositionContainer(proxycatalog);

            var adapter = new MEFAdapter(container).WithProxyFactory<DynamicProxyFactory>();
            IoC.Initialize(adapter);

            adapter.Register(new[] {
                new Singleton {
                    Service = typeof(ClassWithBehaviour), Implementation = typeof(ClassWithBehaviour)
                }
            });

            var sl = (IServiceLocator)adapter;
            var instance1 = sl.GetInstance<ClassWithBehaviour>();
            var instance2 = sl.GetInstance<ClassWithBehaviour>();

            Assert.AreSame(instance1, instance2);
        }
    }
}