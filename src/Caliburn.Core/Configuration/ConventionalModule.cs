namespace Caliburn.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using IoC;

    public abstract class ConventionalModule<TModule, TServicesDescription> : CaliburnModule<TModule>
        where TModule : ConventionalModule<TModule, TServicesDescription>, new()
    {
        private readonly Dictionary<Type, IComponentRegistration> _services = new Dictionary<Type, IComponentRegistration>();

        protected ConventionalModule()
        {
            SetupDefaultServices();
        }

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

        protected virtual Type DetermineDefaultImplementation(Type service) 
        {
            var name = service.FullName.Replace(service.Name, "Default" + service.Name.Substring(1));
            return service.Assembly.GetType(name);
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

        protected override IEnumerable<IComponentRegistration> GetComponentsCore()
        {
            return _services.Values;
        }
    }
}