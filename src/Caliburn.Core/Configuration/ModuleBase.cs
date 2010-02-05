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
        public virtual IEnumerable<IComponentRegistration> GetComponents()
        {
            yield break;
        }

        /// <summary>
        /// Initializes the specified locator.
        /// </summary>
        /// <param name="locator">The locator.</param>
        public virtual void Initialize(IServiceLocator locator) { }

        /// <summary>
        /// Creates a singleton registration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="implementation">The implementation.</param>
        /// <returns>The registration.</returns>
        protected Singleton Singleton(Type service, Type implementation)
        {
            return new Singleton(service) {Implementation = implementation};
        }

        /// <summary>
        /// Creates a per request registration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="implementation">The implementation.</param>
        /// <returns>The registration.</returns>
        protected PerRequest PerRequest(Type service, Type implementation)
        {
            return new PerRequest(service) { Implementation = implementation };
        }

        /// <summary>
        /// Creates a singleton registration.
        /// </summary>
        /// <returns>The registration.</returns>
        protected Singleton Singleton<TService, TImplementation>()
        {
            return new Singleton(typeof(TService)) {Implementation = typeof(TImplementation)};
        }

        /// <summary>
        /// Creates a per request registration.
        /// </summary>
        /// <returns>The registration.</returns>
        protected PerRequest PerRequest<TService, TImplementation>()
        {
            return new PerRequest(typeof(TService)) {Implementation = typeof(TImplementation)};
        }
    }
}