namespace Caliburn.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using IoC;
    using Microsoft.Practices.ServiceLocation;

    public abstract class ModuleBase : IModule
    {
        IEnumerable<IComponentRegistration> IModule.GetComponents()
        {
            return GetComponentsCore();
        }

        protected virtual IEnumerable<IComponentRegistration> GetComponentsCore()
        {
            yield break;
        }

        void IModule.Initialize(IServiceLocator serviceLocator)
        {
            InitializeCore(serviceLocator);
        }

        protected virtual void InitializeCore(IServiceLocator locator) { }

        protected Singleton Singleton(Type service, Type implementation)
        {
            return new Singleton{ Service = service, Implementation = implementation};
        }

        protected PerRequest PerRequest(Type service, Type implementation)
        {
            return new PerRequest { Service = service, Implementation = implementation };
        }
    }
}