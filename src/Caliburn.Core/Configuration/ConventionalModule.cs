namespace Caliburn.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using IoC;
    using Logging;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// A module which uses conventions to register its requred components.
    /// </summary>
    /// <typeparam name="TModule">The type of the module.</typeparam>
    /// <typeparam name="TServicesDescription">The type of the services description.</typeparam>
    public abstract class ConventionalModule<TModule, TServicesDescription> : CaliburnModule<TModule>
        where TModule : ConventionalModule<TModule, TServicesDescription>, new()
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(ConventionalModule<TModule, TServicesDescription>));
        private readonly Dictionary<Type, IServiceConfiguration> _services = new Dictionary<Type, IServiceConfiguration>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionalModule&lt;TModule, TServicesDescription&gt;"/> class.
        /// </summary>
        protected ConventionalModule()
        {
            SetupDefaultServices();
        }

        /// <summary>
        /// Customizes the specified service.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <typeparam name="TRegistration">The type of the registration.</typeparam>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public ServiceConfiguration<TModule, TServicesDescription, TRegistration, TImplementation> Using<TImplementation, TRegistration>(Expression<Func<TServicesDescription, IConfiguredRegistration<TRegistration, TImplementation>>> service)
            where TRegistration : ComponentRegistrationBase, new()
        {
            var description = ((MethodCallExpression)service.Body).Method;
            var serviceType = DetermineService(description);

            return AddService<TRegistration, TImplementation>(serviceType);
        }

        /// <summary>
        /// Adds the service configuration.
        /// </summary>
        /// <typeparam name="TRegistration">The type of the registration.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        protected ServiceConfiguration<TModule, TServicesDescription, TRegistration, TImplementation> AddService<TRegistration, TImplementation>(Type serviceType)
            where TRegistration : ComponentRegistrationBase, new()
        {
            var configuration = new ServiceConfiguration<TModule, TServicesDescription, TRegistration, TImplementation>(
                (TModule)this,
                serviceType
                );

            _services[serviceType] = configuration;

            return configuration;
        }

        /// <summary>
        /// Gets the components.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IComponentRegistration> GetComponents()
        {
            Log.Info("Getting components for {0}.", this);
            return _services.Values.Select(x => x.CreateRegistration());
        }

        /// <summary>
        /// Initializes the specified locator.
        /// </summary>
        /// <param name="locator">The locator.</param>
        public override void Initialize(IServiceLocator locator)
        {
            Log.Info("Initializing {0}.", this);
            _services.Values.Apply(x => x.ConfigureService(locator));
        }

        private void SetupDefaultServices()
        {
            var methods = typeof(TServicesDescription)
                .GetMethods();

            foreach(var info in methods)
            {
                var service = DetermineService(info);
                var implementation = DetermineDefaultImplementation(service);
                var openConfigurationType = typeof(ServiceConfiguration<,,,>);
                var registration = DetermineRegistration(info);

                var closedConfigurationType = openConfigurationType.MakeGenericType(
                    typeof(TModule),
                    typeof(TServicesDescription),
                    registration,
                    implementation
                    );

                _services[service] = (IServiceConfiguration)Activator.CreateInstance(
                    closedConfigurationType,
                    new object[] { this, service }
                    );
            }
        }

        /// <summary>
        /// Determines the default implementation.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>The default implemenation.</returns>
        protected virtual Type DetermineDefaultImplementation(Type service) 
        {
            var name = service.FullName.Replace(service.Name, "Default" + service.Name.Substring(1));
            return service.Assembly.GetType(name);
        }

        private static Type DetermineRegistration(MethodInfo info)
        {
            return info.ReturnType
                .GetGenericArguments()
                .First();
        }

        private static Type DetermineService(MethodInfo info)
        {
            return info
                .GetGenericMethodDefinition()
                .GetGenericArguments()
                .First()
                .GetGenericParameterConstraints()
                .First();
        }
    }
}