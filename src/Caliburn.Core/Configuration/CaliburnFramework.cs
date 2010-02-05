namespace Caliburn.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using IoC;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// A gateway for configuring Caliburn.
    /// </summary>
    public class CaliburnFramework : ICaliburnFramework, IConfigurationBuilder, IModuleHook
    {
        /// <summary>
        /// Configures caliburn with the <see cref="SimpleContainer"/>.
        /// </summary>
        /// <returns></returns>
        public static IConfigurationBuilder Configure()
        {
            return Configure(new SimpleContainer());
        }

        /// <summary>
        /// Configures Caliburn with the specified container implementation.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        public static IConfigurationBuilder Configure(IContainer container)
        {
            return Configure(container, container.Register);
        }

        /// <summary>
        /// Configures Caliburn with the specified <see cref="IServiceLocator"/> and configurator <see cref="IRegistry"/>.
        /// </summary>
        /// <param name="serviceLocator">The serviceLocator.</param>
        /// <param name="registry">The configurator.</param>
        /// <returns></returns>
        public static IConfigurationBuilder Configure(IServiceLocator serviceLocator, IRegistry registry)
        {
            return Configure(serviceLocator, registry.Register);
        }

        /// <summary>
        /// Configures Caliburn with the specified <see cref="IServiceLocator"/> and configurator method.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="register">The configurator.</param>
        /// <returns></returns>
        public static IConfigurationBuilder Configure(IServiceLocator serviceLocator, Action<IEnumerable<IComponentRegistration>> register)
        {
            return new CaliburnFramework(serviceLocator, register);
        }

        private static readonly Type _moduleType = typeof(IModule);

        internal static ICaliburnFramework Instance { get; private set; }
        internal static IModuleHook ModuleHook { get; private set; }

        private readonly IServiceLocator _serviceLocator;
        private readonly Action<IEnumerable<IComponentRegistration>> _register;
        private readonly List<IModule> _modules = new List<IModule>();
        private IEnumerable<Assembly> _assembliesToInspect = new List<Assembly>();
        private bool _isStarted;

        private CaliburnFramework(IServiceLocator serviceLocator, Action<IEnumerable<IComponentRegistration>> register)
        {
            _serviceLocator = serviceLocator;
            _register = register;

            ServiceLocator.SetLocatorProvider(() => serviceLocator);
            AddModule(CaliburnModule<CoreConfiguration>.Instance);

            Instance = this;
            ModuleHook = this;
        }

        /// <summary>
        /// Allows extension of the configuration.
        /// </summary>
        /// <value>The extensibility hook.</value>
        IModuleHook IConfigurationBuilder.With
        {
            get { return this; }
        }

        /// <summary>
        /// Adds assemblies to search for types registerable in the DI container.
        /// </summary>
        /// <param name="assembliesToInspect">The assemblies to register.</param>
        /// <returns></returns>
        IConfigurationBuilder IModuleHook.Assemblies(params Assembly[] assembliesToInspect)
        {
            _assembliesToInspect = assembliesToInspect != null ? assembliesToInspect.Where(x => x != null) : new Assembly[] { };
            return this;
        }

        /// <summary>
        /// Adds the module.
        /// </summary>
        /// <typeparam name="T">The module type.</typeparam>
        /// <param name="module">The module.</param>
        /// <returns>The module.</returns>
        T IModuleHook.Module<T>(T module)
        {
            if(!_modules.Contains(module))
                _modules.Add(module);
            else return (T)_modules.First(x => x.Equals(module));

            if(_isStarted)
            {
                _register(module.GetComponents());
                module.Initialize(_serviceLocator);
            }

            return module;
        }

        /// <summary>
        /// Adds a module to the famework.
        /// </summary>
        /// <param name="module">The module.</param>
        public void AddModule(IModule module)
        {
            ((IModuleHook)this).Module(module);
        }

        /// <summary>
        /// Starts the framework.
        /// </summary>
        public void Start()
        {
            if(_isStarted)
                return;

            var registrations = new List<IComponentRegistration>();
            var modules = new List<IModule>();

            _assembliesToInspect.Apply(x => Inspect(x, registrations, modules));
            modules.Apply(AddModule);

            var components = _modules.SelectMany(x => x.GetComponents())
                .Union(registrations)
                .Union(new[]
                {
                    new Instance
                    {
                        Service = typeof(ICaliburnFramework),
                        Implementation = this
                    }
                });

            _register(components);

            var assemblySource = _serviceLocator.GetInstance<IAssemblySource>();
            _assembliesToInspect.Apply(assemblySource.Add);

            _modules.Apply(x => x.Initialize(_serviceLocator));
            assemblySource.AssemblyAdded += NewAssemblyAdded;
            _isStarted = true;

            CaliburnModule<CoreConfiguration>.Instance.ExecuteAfterStart();
        }

        private void NewAssemblyAdded(Assembly assembly)
        {
            var registrations = new List<IComponentRegistration>();
            var modules = new List<IModule>();

            Inspect(assembly, registrations, modules);

            _register(registrations);

            foreach(var module in modules)
            {
                AddModule(module);
            }
        }

        private static void Inspect(Assembly assembly, ICollection<IComponentRegistration> componentList, ICollection<IModule> modules)
        {
            var types = assembly.GetExportedTypes();

            foreach (var type in types)
            {
                foreach (var attribute in type.GetAttributes<IComponentMetadata>(true))
                    componentList.Add(attribute.GetComponentInfo(type));
            }

            foreach (var type in types)
            {
                if (!_moduleType.IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
                    continue;

                var singleton = type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                if (singleton != null)
                    modules.Add((IModule)singleton.GetValue(null, null));
                else modules.Add((IModule)Activator.CreateInstance(type));
            }
        }
    }
}