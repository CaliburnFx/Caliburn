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
        /// Creates a singleton registration.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <returns>The registration.</returns>
        protected Singleton Singleton<TService, TImplementation>()
            where TImplementation : TService
        {
            return new Singleton(typeof(TService)) {Implementation = typeof(TImplementation)};
        }

        /// <summary>
        /// Creates a singleton registration.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The registration.</returns>
        protected Singleton Singleton<TService, TImplementation>(string key)
                where TImplementation : TService
        {
            return new Singleton(key, typeof(TService)) { Implementation = typeof(TImplementation) };
        }

        /// <summary>
        /// Creates a singleton registration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="implementation">The implementation.</param>
        /// <param name="key">The key.</param>
        /// <returns>The registration.</returns>
        protected Singleton Singleton(Type service, Type implementation, string key)
        {
            return new Singleton(key, service) { Implementation = implementation };
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
        /// Creates a per request registration.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <returns>The registration.</returns>
        protected PerRequest PerRequest<TService, TImplementation>()
            where TImplementation : TService
        {
            return new PerRequest(typeof(TService)) {Implementation = typeof(TImplementation)};
        }

        /// <summary>
        /// Creates a per request registration.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The registration.</returns>
        protected PerRequest PerRequest<TService, TImplementation>(string key)
            where TImplementation : TService
        {
            return new PerRequest(key, typeof(TService)) { Implementation = typeof(TImplementation) };
        }

        /// <summary>
        /// Creates a per request registration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="implementation">The implementation.</param>
        /// <param name="key">The key.</param>
        /// <returns>The registration.</returns>
        protected PerRequest PerRequest(Type service, Type implementation, string key)
        {
            return new PerRequest(key, service) { Implementation = implementation };
        }

        /// <summary>
        /// Creates an instance registration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        protected Instance Instance(Type service, object instance)
        {
            return new Instance { Service = service, Implementation = instance };
        }

        /// <summary>
        /// Creates an instance registration.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        protected Instance Instance<TService>(TService instance)
        {
            return new Instance { Service = typeof(TService), Implementation = instance };
        }

        /// <summary>
        /// Creates an instance registration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected Instance Instance(Type service, object instance, string key)
        {
            return new Instance { Service = service, Implementation = instance };
        }

        /// <summary>
        /// Creates an instance registration.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected Instance Instance<TService>(TService instance, string key)
        {
            return new Instance { Service = typeof(TService), Implementation = instance, Name = key };
        }
    }
}