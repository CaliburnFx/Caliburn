namespace Caliburn.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using IoC;

    /// <summary>
    /// A module which uses conventions to register its requred components.
    /// </summary>
    /// <typeparam name="TModule">The type of the module.</typeparam>
    /// <typeparam name="TServicesDescription">The type of the services description.</typeparam>
    public abstract class ConventionalModule<TModule, TServicesDescription> : CaliburnModule<TModule>
        where TModule : ConventionalModule<TModule, TServicesDescription>, new()
    {
        private readonly Dictionary<Type, IComponentRegistration> _services = new Dictionary<Type, IComponentRegistration>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionalModule&lt;TModule, TServicesDescription&gt;"/> class.
        /// </summary>
        protected ConventionalModule()
        {
            SetupDefaultServices();
        }

        /// <summary>
        /// Enables this modules services to be customized.
        /// </summary>
        /// <typeparam name="TRegistration">The type of the registration.</typeparam>
        /// <param name="service">The service.</param>
        /// <returns>The module.</returns>
        public TModule Using<TRegistration>(Expression<Func<TServicesDescription, TRegistration>> service)
            where TRegistration : IComponentRegistration, new()
        {
            var registration = new TRegistration();
            var description = ((MethodCallExpression)service.Body).Method;
            var serviceType = DetermineService(description);
            var implementationType = description
                .GetGenericArguments()
                .First();

            AddRegistration(registration, serviceType, implementationType);

            return (TModule)this;
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

        /// <summary>
        /// Gets the components.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IComponentRegistration> GetComponentsCore()
        {
            return _services.Values;
        }

        private void SetupDefaultServices()
        {
            var descriptions = typeof(TServicesDescription)
                .GetMethods();

            foreach(var description in descriptions)
            {
                var registration = (IComponentRegistration)Activator.CreateInstance(description.ReturnType);
                var service = DetermineService(description);
                var implementation = DetermineDefaultImplementation(service);

                AddRegistration(registration, service, implementation);
            }
        }

        private void AddRegistration(IComponentRegistration registration, Type service, Type implementation)
        {
            var implementationProperty = registration.GetType()
                .GetProperty("Implementation");
            implementationProperty.SetValue(registration, implementation, null);

            var serviceProperty = registration.GetType()
                .GetProperty("Service");
            serviceProperty.SetValue(registration, service, null);

            _services[service] = registration;
        }

        private static Type DetermineService(MethodInfo description)
        {
            return description
                .GetGenericMethodDefinition()
                .GetGenericArguments()
                .First()
                .GetGenericParameterConstraints()
                .First();
        }
    }
}