namespace Caliburn.Prism
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Core;
    using Microsoft.Practices.Composite;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Logging;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.Presentation.Regions;
    using Microsoft.Practices.Composite.Presentation.Regions.Behaviors;
    using Microsoft.Practices.Composite.Regions;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The configuration for the composite application library.
    /// </summary>
    public class CompositeApplicationLibraryModule : CaliburnModule
    {
        private Type _loggerFacade;
        private Type _moduleInitializer;
        private Type _regionManager;
        private Type _eventAggregator;
        private Type _regionViewRegistry;
        private Type _regionBehaviorFactory;

        private readonly Func<DependencyObject> _createShell;
        private Action<IRegionBehaviorFactory> _afterConfigureBehaviors;
        private Action<IRegionBehaviorFactory> _overrideRegionBehavior;
        private Action<RegionAdapterMappings> _afterConfigureRegionAdapterMappings;
        private Action<RegionAdapterMappings> _overrideRegionAdapterMappings;

        private IModuleCatalog _moduleCatalog;
        private Type[] _moduleTypes;
        private Action _overrideModuleInit;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeApplicationLibraryModule"/> class.
        /// </summary>
        /// <param name="hook">The hook.</param>
        /// <param name="createShell">The create shell.</param>
        public CompositeApplicationLibraryModule(IConfigurationHook hook, Func<DependencyObject> createShell)
            : base(hook)
        {
            UsingLoggerFacade<ConsoleLogger>();

            /* Caliburn uses a custom ModuleInitializer in order to register the component
             * add load the modules assembly */
            UsingModuleInitializer<CaliburnModuleInitializer>();

            UsingRegionManager<RegionManager>();
            UsingEventAggregator<EventAggregator>();
            UsingRegionViewRegistry<RegionViewRegistry>();
            UsingRegionBehaviorFactory<RegionBehaviorFactory>();

            _createShell = createShell;
        }

        /// <summary>
        ///Customizes the logger facade.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CompositeApplicationLibraryModule UsingLoggerFacade<T>()
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
        public CompositeApplicationLibraryModule UsingModuleInitializer<T>()
            where T : IModuleInitializer
        {
            _moduleInitializer = typeof(T);
            return this;
        }

        /// <summary>
        /// Customizes the region manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CompositeApplicationLibraryModule UsingRegionManager<T>()
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
        public CompositeApplicationLibraryModule UsingEventAggregator<T>()
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
        public CompositeApplicationLibraryModule UsingRegionViewRegistry<T>()
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
        public CompositeApplicationLibraryModule UsingRegionBehaviorFactory<T>()
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
        public CompositeApplicationLibraryModule WithModuleTypes(params Type[] modules)
        {
            _moduleTypes = modules;
            return this;
        }

        /// <summary>
        /// Customizes the module catalog.
        /// </summary>
        /// <param name="moduleCatalog">The module catalog.</param>
        /// <returns></returns>
        public CompositeApplicationLibraryModule WithModuleCatalog(IModuleCatalog moduleCatalog)
        {
            _moduleCatalog = moduleCatalog;
            return this;
        }

        /// <summary>
        /// Overrides the module initialization.
        /// </summary>
        /// <param name="doThisInstead">The action to perform.</param>
        /// <returns></returns>
        public CompositeApplicationLibraryModule OverrideModuleInitializationWith(Action doThisInstead)
        {
            _overrideModuleInit = doThisInstead;
            return this;
        }

        /// <summary>
        /// Overrides the region behaviors using the provided action.
        /// </summary>
        /// <param name="doThisInstead">The do this instead.</param>
        /// <returns></returns>
        public CompositeApplicationLibraryModule OverrideRegionBehaviorsWith(Action<IRegionBehaviorFactory> doThisInstead)
        {
            _overrideRegionBehavior = doThisInstead;
            return this;
        }

        /// <summary>
        /// Executes the action after the region behaviors are configured.
        /// </summary>
        /// <param name="configureBehaviors">The configure behaviors.</param>
        /// <returns></returns>
        public CompositeApplicationLibraryModule AfterConfigureRegionBehaviors(Action<IRegionBehaviorFactory> configureBehaviors)
        {
            _afterConfigureBehaviors = configureBehaviors;
            return this;
        }

        /// <summary>
        /// Overrides the region adapter mappings using the provided action.
        /// </summary>
        /// <param name="doThisInstead">The do this instead.</param>
        /// <returns></returns>
        public CompositeApplicationLibraryModule OverrideRegionAdapterMappingsWith(Action<RegionAdapterMappings> doThisInstead)
        {
            _overrideRegionAdapterMappings = doThisInstead;
            return this;
        }

        /// <summary>
        /// Executes the action after configuring the region adapter mappings.
        /// </summary>
        /// <param name="configureRegionAdapterMappings">The configure region adapter mappings.</param>
        /// <returns></returns>
        public CompositeApplicationLibraryModule AfterConfigureRegionAdapterMappings(Action<RegionAdapterMappings> configureRegionAdapterMappings)
        {
            _afterConfigureRegionAdapterMappings = configureRegionAdapterMappings;
            return this;
        }

        /// <summary>
        /// Gets the component information for this module.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<ComponentInfo> GetComponents()
        {
            yield return Singleton(typeof(ILoggerFacade), _loggerFacade);
            yield return Singleton(typeof(IModuleInitializer), _moduleInitializer);
            yield return Singleton(typeof(IRegionManager), _regionManager);
            yield return Singleton(typeof(IEventAggregator), _eventAggregator);
            yield return Singleton(typeof(IRegionViewRegistry), _regionViewRegistry);
            yield return Singleton(typeof(IRegionBehaviorFactory), _regionBehaviorFactory);

            //currently dependent on default module manager
            yield return Singleton(typeof(IModuleManager), typeof(ModuleManager));
            yield return Singleton(typeof(RegionAdapterMappings), typeof(RegionAdapterMappings));


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
        protected override void Initialize()
        {
            ConfigureRegionAdapterMappings();
            ConfigureRegionBehaviors();
            RegisterFrameworkExceptionTypes();

            Core.AfterStart(CreateShell);

            // Initialize modules after caliburn has started in order for the DefaultBinder
            // conventions to work
            Core.AfterStart(InitializeModules);
        }

        private void CreateShell()
        {
            var shell = _createShell();
            if(shell == null) 
                return;

            RegionManager.SetRegionManager(
                shell,
                ServiceLocator.GetInstance<IRegionManager>()
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
                    var module = ServiceLocator.GetInstance(x) as IModule;
                    if(module != null)
                        module.Initialize();
                });

            if(_moduleCatalog == null) 
                return;

            // Unable to RegisterInstance of module catalog without taking dependecy on a 
            // specific IoC container. So this implementation is currently dependent on the 
            // default prism ModuleManager.  
            var manager = new ModuleManager(
                ServiceLocator.GetInstance<IModuleInitializer>(),
                _moduleCatalog,
                ServiceLocator.GetInstance<ILoggerFacade>()
                );

            manager.Run();
        }

        private void ConfigureRegionAdapterMappings()
        {
            var regionAdapterMappings = ServiceLocator.TryResolve<RegionAdapterMappings>();
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
                ServiceLocator.GetInstance<TabControlRegionAdapter>()
                );
#endif
            regionAdapterMappings.RegisterMapping(
                typeof(Selector),
                ServiceLocator.GetInstance<SelectorRegionAdapter>()
                );

            regionAdapterMappings.RegisterMapping(
                typeof(ItemsControl),
                ServiceLocator.GetInstance<ItemsControlRegionAdapter>()
                );

            regionAdapterMappings.RegisterMapping(
                typeof(ContentControl),
                ServiceLocator.GetInstance<ContentControlRegionAdapter>()
                );

            if(_afterConfigureRegionAdapterMappings != null)
                _afterConfigureRegionAdapterMappings(regionAdapterMappings);
        }

        private void ConfigureRegionBehaviors()
        {
            var defaultRegionBehaviorTypesDictionary = ServiceLocator.TryResolve<IRegionBehaviorFactory>();

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