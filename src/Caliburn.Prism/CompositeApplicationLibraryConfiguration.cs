namespace Caliburn.Prism
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Core;
    using Core.Configuration;
    using Core.IoC;
    using Microsoft.Practices.Composite;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Logging;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.Presentation.Regions;
    using Microsoft.Practices.Composite.Presentation.Regions.Behaviors;
    using Microsoft.Practices.Composite.Regions;
    using Microsoft.Practices.ServiceLocation;
    using IModule=Microsoft.Practices.Composite.Modularity.IModule;
    using System.Reflection;

    /// <summary>
    /// The configuration for the composite application library.
    /// </summary>
    public class CompositeApplicationLibraryConfiguration : CaliburnModule<CompositeApplicationLibraryConfiguration>
    {
        private Type _loggerFacade;
        private Type _moduleInitializer;
        private Type _moduleManager;
        private Type _regionManager;
        private Type _eventAggregator;
        private Type _regionViewRegistry;
        private Type _regionBehaviorFactory;

        private Func<DependencyObject> _createShell = () => null;
        private Action<IRegionBehaviorFactory> _afterConfigureBehaviors;
        private Action<IRegionBehaviorFactory> _overrideRegionBehavior;
        private Action<RegionAdapterMappings> _afterConfigureRegionAdapterMappings;
        private Action<RegionAdapterMappings> _overrideRegionAdapterMappings;

        private IModuleCatalog _moduleCatalog;
        private Type[] _moduleTypes;
        private Action _overrideModuleInit;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeApplicationLibraryConfiguration"/> class.
        /// </summary>
        public CompositeApplicationLibraryConfiguration()
        {
            UsingLoggerFacade<ConsoleLogger>();

            /* Caliburn uses a custom ModuleInitializer in order to register the component
             * add load the modules assembly */
            UsingModuleInitializer<CaliburnModuleInitializer>();

            UsingModuleManager<ModuleManager>();
            UsingRegionManager<RegionManager>();
            UsingEventAggregator<EventAggregator>();
            UsingRegionViewRegistry<RegionViewRegistry>();
            UsingRegionBehaviorFactory<RegionBehaviorFactory>();
        }

        /// <summary>
        /// Creates the shell using the provided function.
        /// </summary>
        /// <param name="createShell">The create shell function.</param>
        /// <returns></returns>
        public CompositeApplicationLibraryConfiguration CreateShellUsing(Func<DependencyObject> createShell)
        {
            _createShell = createShell;
            return this;
        }

        /// <summary>
        ///Customizes the logger facade.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CompositeApplicationLibraryConfiguration UsingLoggerFacade<T>()
            where T : ILoggerFacade
        {
            _loggerFacade = typeof(T);
            return this;
        }

        /// <summary>
        /// Customizes the module initializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CompositeApplicationLibraryConfiguration UsingModuleInitializer<T>()
            where T : IModuleInitializer
        {
            _moduleInitializer = typeof(T);
            return this;
        }

        /// <summary>
        /// Customizes the module manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CompositeApplicationLibraryConfiguration UsingModuleManager<T>()
            where T : IModuleManager
        {
            _moduleManager = typeof(T);
            return this;
        }

        /// <summary>
        /// Customizes the region manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CompositeApplicationLibraryConfiguration UsingRegionManager<T>()
            where T : IRegionManager
        {
            _regionManager = typeof(T);
            return this;
        }

        /// <summary>
        /// Customizes the event aggregator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CompositeApplicationLibraryConfiguration UsingEventAggregator<T>()
            where T : IEventAggregator
        {
            _eventAggregator = typeof(T);
            return this;
        }

        /// <summary>
        /// Customizes the region view registry.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CompositeApplicationLibraryConfiguration UsingRegionViewRegistry<T>()
            where T : IRegionViewRegistry
        {
            _regionViewRegistry = typeof(T);
            return this;
        }

        /// <summary>
        /// Customizes the region behavior factory.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CompositeApplicationLibraryConfiguration UsingRegionBehaviorFactory<T>()
            where T : IRegionBehaviorFactory
        {
            _regionBehaviorFactory = typeof(T);
            return this;
        }

        /// <summary>
        /// Customizes the module types.
        /// </summary>
        /// <param name="modules">The modules.</param>
        /// <returns></returns>
        public CompositeApplicationLibraryConfiguration WithModuleTypes(params Type[] modules)
        {
            _moduleTypes = modules;
            return this;
        }

        /// <summary>
        /// Customizes the module catalog.
        /// </summary>
        /// <param name="moduleCatalog">The module catalog.</param>
        /// <returns></returns>
        public CompositeApplicationLibraryConfiguration WithModuleCatalog(IModuleCatalog moduleCatalog)
        {
            _moduleCatalog = moduleCatalog;
            return this;
        }

        /// <summary>
        /// Overrides the module initialization.
        /// </summary>
        /// <param name="doThisInstead">The action to perform.</param>
        /// <returns></returns>
        public CompositeApplicationLibraryConfiguration OverrideModuleInitializationWith(Action doThisInstead)
        {
            _overrideModuleInit = doThisInstead;
            return this;
        }

        /// <summary>
        /// Overrides the region behaviors using the provided action.
        /// </summary>
        /// <param name="doThisInstead">The do this instead.</param>
        /// <returns></returns>
        public CompositeApplicationLibraryConfiguration OverrideRegionBehaviorsWith(Action<IRegionBehaviorFactory> doThisInstead)
        {
            _overrideRegionBehavior = doThisInstead;
            return this;
        }

        /// <summary>
        /// Executes the action after the region behaviors are configured.
        /// </summary>
        /// <param name="configureBehaviors">The configure behaviors.</param>
        /// <returns></returns>
        public CompositeApplicationLibraryConfiguration AfterConfigureRegionBehaviors(Action<IRegionBehaviorFactory> configureBehaviors)
        {
            _afterConfigureBehaviors = configureBehaviors;
            return this;
        }

        /// <summary>
        /// Overrides the region adapter mappings using the provided action.
        /// </summary>
        /// <param name="doThisInstead">The do this instead.</param>
        /// <returns></returns>
        public CompositeApplicationLibraryConfiguration OverrideRegionAdapterMappingsWith(Action<RegionAdapterMappings> doThisInstead)
        {
            _overrideRegionAdapterMappings = doThisInstead;
            return this;
        }

        /// <summary>
        /// Executes the action after configuring the region adapter mappings.
        /// </summary>
        /// <param name="configureRegionAdapterMappings">The configure region adapter mappings.</param>
        /// <returns></returns>
        public CompositeApplicationLibraryConfiguration AfterConfigureRegionAdapterMappings(Action<RegionAdapterMappings> configureRegionAdapterMappings)
        {
            _afterConfigureRegionAdapterMappings = configureRegionAdapterMappings;
            return this;
        }

        /// <summary>
        /// Gets the component information for this module.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IComponentRegistration> GetComponentsCore()
        {
            yield return Singleton(typeof(ILoggerFacade), _loggerFacade);
            yield return Singleton(typeof(IModuleInitializer), _moduleInitializer);
            yield return Singleton(typeof(IRegionManager), _regionManager);
            yield return Singleton(typeof(IEventAggregator), _eventAggregator);
            yield return Singleton(typeof(IRegionViewRegistry), _regionViewRegistry);
            yield return Singleton(typeof(IRegionBehaviorFactory), _regionBehaviorFactory);


            yield return Singleton(typeof(IModuleManager), _moduleManager);
            yield return Singleton(typeof(RegionAdapterMappings), typeof(RegionAdapterMappings));
            yield return new Instance {Service = typeof(IModuleCatalog), Implementation = _moduleCatalog  };
            foreach (var module in _moduleTypes)
            {
                yield return Singleton(module, module);
            }

            /* Patch to support Containers that require explicit Component registration */
            //register default adapters
            yield return PerRequest(typeof(SelectorRegionAdapter), typeof(SelectorRegionAdapter));
            yield return PerRequest(typeof(ItemsControlRegionAdapter), typeof(ItemsControlRegionAdapter));
            yield return PerRequest(typeof(ContentControlRegionAdapter), typeof(ContentControlRegionAdapter));
#if SILVERLIGHT
            yield return PerRequest(typeof(TabControlRegionAdapter), typeof(TabControlRegionAdapter));
#endif

            //register default behaviors
            yield return PerRequest(typeof(AutoPopulateRegionBehavior), typeof(AutoPopulateRegionBehavior));
            yield return PerRequest(typeof(BindRegionContextToDependencyObjectBehavior), typeof(BindRegionContextToDependencyObjectBehavior));
            yield return PerRequest(typeof(RegionActiveAwareBehavior), typeof(RegionActiveAwareBehavior));
            yield return PerRequest(typeof(SyncRegionContextWithHostBehavior), typeof(SyncRegionContextWithHostBehavior));
            yield return PerRequest(typeof(RegionManagerRegistrationBehavior), typeof(RegionManagerRegistrationBehavior));
            yield return PerRequest(typeof(DelayedRegionCreationBehavior), typeof(DelayedRegionCreationBehavior));
            
        }

        /// <summary>
        /// Initializes this module.
        /// </summary>
        protected override void InitializeCore(IServiceLocator serviceLocator)
        {
            ConfigureRegionAdapterMappings();
            ConfigureRegionBehaviors();
            RegisterFrameworkExceptionTypes();

            CaliburnModule<CoreConfiguration>.Instance.AfterStart(CreateShell);

            // Initialize modules after caliburn has started in order for the DefaultBinder
            // conventions to work
            CaliburnModule<CoreConfiguration>.Instance.AfterStart(InitializeModules);
        }

        private void CreateShell()
        {
            var shell = _createShell();
            if(shell == null) 
                return;

            RegionManager.SetRegionManager(
                shell,
                ServiceLocator.Current.GetInstance<IRegionManager>()
                );

            RegionManager.UpdateRegions();
        }

        private void InitializeModules()
        {
            if(_overrideModuleInit != null)
            {
                _overrideModuleInit();
                return;
            }

            if (_moduleCatalog == null && _moduleTypes == null)
                throw new InvalidOperationException(
                    string.Format(
                        "{0} {1}",
                        "Failed to initialize modules.",
                        "You must provide modules to load through either the WithModules or WithModuleCatalog methods")
                    );

            if(_moduleTypes != null)
                _moduleTypes.Apply(x =>{
                    var module = ServiceLocator.Current.GetInstance(x) as IModule;
                    if (module != null)
                    {
                        AddAssemblyIfMissing(x.Assembly);
                        module.Initialize();
                    }
                });

            if(_moduleCatalog == null) 
                return;
 
            var manager =
                ServiceLocator.Current.TryResolve<IModuleManager>();
            if (manager != null)
                manager.Run();
        }

        private void AddAssemblyIfMissing(Assembly assembly)
        {
            var assemblySource =  ServiceLocator.Current.TryResolve<IAssemblySource>();
            if(assemblySource != null)
                if (!assemblySource.Contains(assembly))
                    assemblySource.Add(assembly);
        }

        private void ConfigureRegionAdapterMappings()
        {
            var regionAdapterMappings = ServiceLocator.Current.TryResolve<RegionAdapterMappings>();
            if (regionAdapterMappings == null)
                return;

            if(_overrideRegionAdapterMappings != null)
            {
                _overrideRegionAdapterMappings(regionAdapterMappings);
                return;
            }

#if SILVERLIGHT
            regionAdapterMappings.RegisterMapping(
                typeof(TabControl), 
                ServiceLocator.Current.GetInstance<TabControlRegionAdapter>()
                );
#endif
            regionAdapterMappings.RegisterMapping(
                typeof(Selector),
                ServiceLocator.Current.GetInstance<SelectorRegionAdapter>()
                );

            regionAdapterMappings.RegisterMapping(
                typeof(ItemsControl),
                ServiceLocator.Current.GetInstance<ItemsControlRegionAdapter>()
                );

            regionAdapterMappings.RegisterMapping(
                typeof(ContentControl),
                ServiceLocator.Current.GetInstance<ContentControlRegionAdapter>()
                );

            if(_afterConfigureRegionAdapterMappings != null)
                _afterConfigureRegionAdapterMappings(regionAdapterMappings);
        }

        private void ConfigureRegionBehaviors()
        {
            var defaultRegionBehaviorTypesDictionary = ServiceLocator.Current.TryResolve<IRegionBehaviorFactory>();

            if(defaultRegionBehaviorTypesDictionary == null) 
                return;


            if (_overrideRegionBehavior != null)
            {
                _overrideRegionBehavior(defaultRegionBehaviorTypesDictionary);
                return;
            }

            defaultRegionBehaviorTypesDictionary.AddIfMissing(
                AutoPopulateRegionBehavior.BehaviorKey,
                typeof(AutoPopulateRegionBehavior)
                );

            defaultRegionBehaviorTypesDictionary.AddIfMissing(
                BindRegionContextToDependencyObjectBehavior.BehaviorKey,
                typeof(BindRegionContextToDependencyObjectBehavior)
                );

            defaultRegionBehaviorTypesDictionary.AddIfMissing(
                RegionActiveAwareBehavior.BehaviorKey,
                typeof(RegionActiveAwareBehavior)
                );

            defaultRegionBehaviorTypesDictionary.AddIfMissing(
                SyncRegionContextWithHostBehavior.BehaviorKey,
                typeof(SyncRegionContextWithHostBehavior)
                );

            defaultRegionBehaviorTypesDictionary.AddIfMissing(
                RegionManagerRegistrationBehavior.BehaviorKey,
                typeof(RegionManagerRegistrationBehavior)
                );

            if(_afterConfigureBehaviors != null)
                _afterConfigureBehaviors(defaultRegionBehaviorTypesDictionary);
        }

        /// <summary>
        /// Registers in the <see cref="ServiceLocator"/> the <see cref="Type"/> of the Exceptions
        /// that are not considered root exceptions by the <see cref="ExceptionExtensions"/>.
        /// </summary>
        private void RegisterFrameworkExceptionTypes()
        {
            ExceptionExtensions.RegisterFrameworkExceptionType(
                typeof(ActivationException)
                );
        }
    }
}