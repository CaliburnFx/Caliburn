namespace Caliburn.Unity
{
    using System;
    using System.Collections.Generic;
    using Core.Behaviors;
    using Core.IoC;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// An adapter allowing an <see cref="IUnityContainer"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IRegistry"/>.
    /// </summary>
    public class UnityAdapter : ContainerBase
    {
        private readonly IUnityContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityAdapter"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public UnityAdapter(IUnityContainer container)
        {
            _container = container;

            _container.RegisterInstance<IServiceLocator>(this);
            _container.RegisterInstance<IRegistry>(this);
            _container.RegisterInstance<IContainer>(this);
            _container.RegisterInstance(_container);

            AddRegistrationHandler<Singleton>(HandleSingleton);
            AddRegistrationHandler<PerRequest>(HandlePerRequest);
            AddRegistrationHandler<CustomLifetime>(HandleCustomLifetime);
            AddRegistrationHandler<Instance>(HandleInstance);
        }

        /// <summary>
        /// Gets the <see cref="IUnityContainer"/>.
        /// </summary>
        /// <value>The container.</value>
        public IUnityContainer Container
        {
            get { return _container; }
        }

        /// <summary>
        ///             When implemented by inheriting classes, this method will do the actual work of resolving
        ///             the requested service instance.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <param name="key">Name of registered service you want. May be null.</param>
        /// <returns>
        /// The requested service instance.
        /// </returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            //HACK: Unity doesn't support component registration with string key only
            //		Named service are registered as object, so a null serviceType has to
            //		be adapted accordingly
            return _container.Resolve(serviceType ?? typeof(object), key);
        }

        /// <summary>
        ///             When implemented by inheriting classes, this method will do the actual work of
        ///             resolving all the requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>
        /// Sequence of service instance objects.
        /// </returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _container.ResolveAll(serviceType);
        }

        /// <summary>
        /// Installs a proxy factory.
        /// </summary>
        /// <typeparam name="T">The type of the proxy factory.</typeparam>
        /// <returns>
        /// A container with an installed proxy factory.
        /// </returns>
        public override IContainer WithProxyFactory<T>()
        {
            Container.RegisterType<IProxyFactory, T>(new ContainerControlledLifetimeManager());
            Container.AddExtension(new ProxyExtension());

            return this;
        }

        private void HandleSingleton(Singleton singleton)
        {
            if (!singleton.HasName())
                _container.RegisterType(singleton.Service, singleton.Implementation, new ContainerControlledLifetimeManager());
            else if (!singleton.HasService())
                _container.RegisterType(typeof(object), singleton.Implementation, singleton.Name, new ContainerControlledLifetimeManager());
            else _container.RegisterType(singleton.Service, singleton.Implementation, singleton.Name, new ContainerControlledLifetimeManager());
        }

        private void HandlePerRequest(PerRequest perRequest)
        {
            if (!perRequest.HasName())
                _container.RegisterType(perRequest.Service, perRequest.Implementation);
            else if (!perRequest.HasService())
                _container.RegisterType(typeof(object), perRequest.Implementation, perRequest.Name);
            else _container.RegisterType(perRequest.Service, perRequest.Implementation, perRequest.Name);
        }

        private void HandleCustomLifetime(CustomLifetime customLifetime)
        {
            if (!customLifetime.HasName())
                _container.RegisterType(customLifetime.Service, customLifetime.Implementation, (LifetimeManager)Activator.CreateInstance(customLifetime.Lifetime));
            else if (!customLifetime.HasService())
                _container.RegisterType(typeof(object), customLifetime.Implementation, customLifetime.Name, (LifetimeManager)Activator.CreateInstance(customLifetime.Lifetime));
            else _container.RegisterType(customLifetime.Service, customLifetime.Implementation, customLifetime.Name, (LifetimeManager)Activator.CreateInstance(customLifetime.Lifetime));
        }

        private void HandleInstance(Instance instance)
        {
            if(!instance.HasName())
                _container.RegisterInstance(instance.Service, instance.Implementation);
            else if(!instance.HasService())
                _container.RegisterInstance(typeof(object), instance.Name, instance.Implementation);
            else _container.RegisterInstance(instance.Service, instance.Name, instance.Implementation);
        }
    }
}