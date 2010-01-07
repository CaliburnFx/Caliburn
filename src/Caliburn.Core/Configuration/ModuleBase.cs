namespace Caliburn.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using IoC;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// A base class for modules.
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        /// <summary>
        /// Gets the component information for this module.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IComponentRegistration> IModule.GetComponents()
        {
            return GetComponentsCore();
        }

        /// <summary>
        /// Gets the components.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<IComponentRegistration> GetComponentsCore()
        {
            yield break;
        }

        /// <summary>
        /// Initializes this module.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        void IModule.Initialize(IServiceLocator serviceLocator)
        {
            InitializeCore(serviceLocator);
        }

        /// <summary>
        /// Initializes this module.
        /// </summary>
        /// <param name="locator">The locator.</param>
        protected virtual void InitializeCore(IServiceLocator locator) { }

        /// <summary>
        /// Creates a singleton registration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="implementation">The implementation.</param>
        /// <returns>The registration.</returns>
        protected Singleton Singleton(Type service, Type implementation)
        {
            return new Singleton{ Service = service, Implementation = implementation};
        }

        /// <summary>
        /// Creates a per request registration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="implementation">The implementation.</param>
        /// <returns>The registration.</returns>
        protected PerRequest PerRequest(Type service, Type implementation)
        {
            return new PerRequest { Service = service, Implementation = implementation };
        }
    }
}