namespace Caliburn.Core.Configuration
{
    using System;
    using System.Linq.Expressions;
    using IoC;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The default implemenation of <see cref="IServiceConfiguration"/>.
    /// </summary>
    /// <typeparam name="TModule">The type of the module.</typeparam>
    /// <typeparam name="TServicesDescription">The type of the services description.</typeparam>
    /// <typeparam name="TRegistration">The type of the registration.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
    public class ServiceConfiguration<TModule, TServicesDescription, TRegistration, TImplementation> : IConfigurationBuilder, IServiceConfiguration
        where TModule : ConventionalModule<TModule, TServicesDescription>, new()
        where TRegistration : ComponentRegistrationBase, new()
    {
        private readonly TModule _module;
        private readonly Type _serviceType;
        private Action<TImplementation> _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceConfiguration&lt;TModule, TServicesDescription, TRegistration, TImplementation&gt;"/> class.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <param name="serviceType">Type of the service.</param>
        public ServiceConfiguration(TModule module, Type serviceType)
        {
            _module = module;
            _serviceType = serviceType;
        }

        /// <summary>
        /// Allows extension of the configuration.
        /// </summary>
        /// <value>The extensibility hook.</value>
        public IModuleHook With
        {
            get { return CaliburnFramework.ModuleHook; }
        }

        /// <summary>
        /// Starts the framework.
        /// </summary>
        public void Start()
        {
            CaliburnFramework.Instance.Start();
        }

        /// <summary>
        /// Customizes the specified service.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <typeparam name="TRegistration">The type of the registration.</typeparam>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public ServiceConfiguration<TModule, TServicesDescription, TRegistration2, TInstance2> Using<TRegistration2, TInstance2>(Expression<Func<TServicesDescription, IConfiguredRegistration<TRegistration2, TInstance2>>> service)
            where TRegistration2 : ComponentRegistrationBase, new()
        {
            return _module.Using(service);
        }

        /// <summary>
        /// Configures the service implementation.
        /// </summary>
        /// <param name="configure">The configure.</param>
        /// <returns></returns>
        public ServiceConfiguration<TModule, TServicesDescription, TRegistration, TImplementation> Configured(Action<TImplementation> configure)
        {
            _configuration = configure;
            return this;
        }

        /// <summary>
        /// Creates the registration.
        /// </summary>
        /// <returns></returns>
        IComponentRegistration IServiceConfiguration.CreateRegistration()
        {
            var reg = new TRegistration {Service = _serviceType};
            reg.GetType().GetProperty("Implementation").SetValue(reg, typeof(TImplementation), null);
            return reg;
        }

        /// <summary>
        /// Configures the service.
        /// </summary>
        /// <param name="locator">The locator.</param>
        void IServiceConfiguration.ConfigureService(IServiceLocator locator)
        {
            if (_configuration == null) 
                return;

            var implementation = (TImplementation)locator.GetService(_serviceType);
            _configuration(implementation);
        }
    }
}