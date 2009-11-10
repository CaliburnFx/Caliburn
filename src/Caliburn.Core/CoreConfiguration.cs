namespace Caliburn.Core
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using Invocation;
    using IoC;
    using Threading;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The configuration for Caliburn's core.
    /// </summary>
    public class CoreConfiguration : IConfigurationHook
    {
        private static readonly Type _configType = typeof(CaliburnModule);
        private readonly IServiceLocator _serviceLocator;
        private readonly Action<IEnumerable<IComponentRegistration>> _registrar;
        private readonly List<CaliburnModule> _modules = new List<CaliburnModule>();
        private readonly List<Action> _afterStart = new List<Action>();

        private Type _threadPoolType;
        private Type _methodFactoryType;
        private Type _eventHandlerFactoryType;
        private Type _dispatcherType;
        private Type _assemblySourceType;
        private IEnumerable<Assembly> _assembliesToInspect = new List<Assembly>();
        private bool _isStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreConfiguration"/> class.
        /// </summary>
        /// <param name="serviceLocator">The container.</param>
        /// <param name="registrar">Receives the components requested for configuration.</param>
        public CoreConfiguration(IServiceLocator serviceLocator, Action<IEnumerable<IComponentRegistration>> registrar)
        {
            _serviceLocator = serviceLocator;
            _registrar = registrar;

            UsingThreadPool<DefaultThreadPool>();
            UsingMethodFactory<MethodFactory>();
            UsingEventHandlerFactory<EventHandlerFactory>();
            UsingDispatcher<Execute.SimpleDispatcher>();
            UsingAssemblySource<DefaultAssemblySource>();
        }

        /// <summary>
        /// Gets the core configuration.
        /// </summary>
        /// <value>The core configuration.</value>
        CoreConfiguration IConfigurationHook.Core
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the service locator.
        /// </summary>
        /// <value>The service locator.</value>
        public IServiceLocator ServiceLocator
        {
            get { return _serviceLocator; }
        }

        /// <summary>
        /// Customizes the thread pool used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The thread pool type.</typeparam>
        /// <returns>The configuration.</returns>
        public CoreConfiguration UsingThreadPool<T>()
            where T : IThreadPool
        {
            _threadPoolType = typeof(T);
            return this;
        }

        /// <summary>
        /// Customizes the method factory used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The method factory type.</typeparam>
        /// <returns>The configuration.</returns>
        public CoreConfiguration UsingMethodFactory<T>()
            where T : IMethodFactory
        {
            _methodFactoryType = typeof(T);
            return this;
        }

        /// <summary>
        /// Customizes the event handler factory.
        /// </summary>
        /// <typeparam name="T">The event handler factory type.</typeparam>
        /// <returns>The configuration.</returns>
        public CoreConfiguration UsingEventHandlerFactory<T>()
            where T : IEventHandlerFactory
        {
            _eventHandlerFactoryType = typeof(T);
            return this;
        }

        /// <summary>
        /// Customizes the dispatcher implementation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CoreConfiguration UsingDispatcher<T>()
            where T : IDispatcher
        {
            _dispatcherType = typeof(T);
            return this;
        }

        /// <summary>
        /// Usings the assembly source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CoreConfiguration UsingAssemblySource<T>()
            where T : IAssemblySource
        {
            _assemblySourceType = typeof(T);
            return this;
        }

        /// <summary>
        /// Adds the specified module.
        /// </summary>
        /// <param name="module">The module.</param>
        public void Add(CaliburnModule module)
        {
            if (_modules.Contains(module))
                return;

            _modules.Add(module);

            if(!_isStarted)
                return;

            var components = module.GetComponents();
            _registrar(components);
            module.Initialize();
            ExecuteCustomActions();
        }

        /// <summary>
        /// Adds assemblies to search for types registerable in the DI container.
        /// </summary>
        /// <param name="assembliesToInspect">The assemblies to register.</param>
        /// <returns></returns>
        public CoreConfiguration WithAssemblies(params Assembly[] assembliesToInspect)
        {
            _assembliesToInspect = assembliesToInspect != null ? assembliesToInspect.Where(x => x != null) : new Assembly[] {};
            return this;
        }

        /// <summary>
        /// Adds actions to execute immediately following the framework startup.
        /// </summary>
        /// <param name="doThis">The action to execute after framework startup.</param>
        /// <returns></returns>
        public CoreConfiguration AfterStart(Action doThis)
        {
            _afterStart.Add(doThis);
            return this;
        }

        /// <summary>
        /// Starts the application.
        /// </summary>
        public void Start()
        {
            if (_isStarted)
                return;

            var toRegister = SetupDefaultComponents();
            var configurationChildren = new List<Type>();

            foreach (var assembly in _assembliesToInspect)
            {
                InspectAssembly(assembly, toRegister, configurationChildren);
            }

            foreach(var type in configurationChildren)
            {
                Activator.CreateInstance(type, new object[] {this});
            }

            (from child in _modules
             from info in child.GetComponents()
             select info).Apply(toRegister.Add);

            _registrar(toRegister);

            Execute.Initialize(_serviceLocator.GetInstance<IDispatcher>());

            var assemblySource = _serviceLocator.GetInstance<IAssemblySource>();
            _assembliesToInspect.Apply(assemblySource.Add);

            _modules.Apply(x => x.Initialize());

            assemblySource.AssemblyAdded += NewAssemblyAdded;

            _isStarted = true;

            ExecuteCustomActions();
        }

        private void ExecuteCustomActions() 
        {
            _afterStart.Apply(x => x());
            _afterStart.Clear();
        }

        private void NewAssemblyAdded(Assembly assembly)
        {
            var toRegister = new List<IComponentRegistration>();
            var configurationChildren = new List<Type>();

            InspectAssembly(assembly, toRegister, configurationChildren);

            _registrar(toRegister);

            foreach (var type in configurationChildren)
            {
                Activator.CreateInstance(type, new object[] {this});
            }
        }

        private void InspectAssembly(Assembly assembly, ICollection<IComponentRegistration> componentList, ICollection<Type> configs)
        {
            var types = assembly.GetExportedTypes();

            foreach (var type in types)
            {
                foreach (var attribute in type.GetCustomAttributes(true).OfType<RegisterAttribute>())
                    componentList.Add(attribute.GetComponentInfo(type));
            }

            foreach (var type in types)
            {
                if(_configType.IsAssignableFrom(type) && !type.IsAbstract)
                    configs.Add(type);
            }
        }

        private List<IComponentRegistration> SetupDefaultComponents()
        {
            return new List<IComponentRegistration>
            {
                new Singleton
                {
                    Service = typeof(IDispatcher),
                    Implementation = _dispatcherType,
                },
                new Singleton
                {
                    Service = typeof(IThreadPool),
                    Implementation = _threadPoolType,
                },
                new Singleton
                {
                    Service = typeof(IMethodFactory),
                    Implementation = _methodFactoryType,
                },
                new Singleton
                {
                    Service = typeof(IEventHandlerFactory),
                    Implementation = _eventHandlerFactoryType,
                },
                new Singleton
                {
                    Service = typeof(IAssemblySource),
                    Implementation = _assemblySourceType,
                }
            };
        }
    }
}