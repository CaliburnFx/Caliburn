namespace Caliburn.Unity
{
    using System;
    using System.Collections.Generic;
    using Core.Behaviors;
    using Core.InversionOfControl;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// An adapter allowing an <see cref="IUnityContainer"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IRegistry"/>.
    /// </summary>
    public class UnityAdapter : ContainerBase
    {
        private readonly IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityAdapter"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public UnityAdapter(IUnityContainer container)
        {
            this.container = container;

            this.container.RegisterInstance<IServiceLocator>(this);
            this.container.RegisterInstance<IRegistry>(this);
            this.container.RegisterInstance<IContainer>(this);
            this.container.RegisterInstance(this.container);

            AddRegistrationHandler<Singleton>(HandleSingleton);
            AddRegistrationHandler<PerRequest>(HandlePerRequest);
            AddRegistrationHandler<Instance>(HandleInstance);
        }

        /// <summary>
        /// Gets the <see cref="IUnityContainer"/>.
        /// </summary>
        /// <value>The container.</value>
        public IUnityContainer Container
        {
            get { return container; }
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
        public override object GetInstance(Type serviceType, string key)
        {
            //HACK: Unity doesn't support component registration with string key only
            //		Named service are registered as object, so a null serviceType has to
            //		be adapted accordingly
            try
            {
                return container.Resolve(serviceType ?? typeof(object), key);                
            }
            catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///             When implemented by inheriting classes, this method will do the actual work of
        ///             resolving all the requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>
        /// Sequence of service instance objects.
        /// </returns>
        public override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return container.ResolveAll(serviceType);
        }

        /// <summary>
        /// Injects dependencies into the object.
        /// </summary>
        /// <param name="instance">The instance to build up.</param>
        public override void BuildUp(object instance)
        {
            container.BuildUp(instance);
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
                container.RegisterType(singleton.Service, singleton.Implementation, new ContainerControlledLifetimeManager());
            else if (!singleton.HasService())
                container.RegisterType(typeof(object), singleton.Implementation, singleton.Name, new ContainerControlledLifetimeManager());
            else container.RegisterType(singleton.Service, singleton.Implementation, singleton.Name, new ContainerControlledLifetimeManager());
        }

        private void HandlePerRequest(PerRequest perRequest)
        {
            if (!perRequest.HasName())
                container.RegisterType(perRequest.Service, perRequest.Implementation);
            else if (!perRequest.HasService())
                container.RegisterType(typeof(object), perRequest.Implementation, perRequest.Name);
            else container.RegisterType(perRequest.Service, perRequest.Implementation, perRequest.Name);
        }

        private void HandleInstance(Instance instance)
        {
            if(!instance.HasName())
                container.RegisterInstance(instance.Service, instance.Implementation);
            else if(!instance.HasService())
                container.RegisterInstance(typeof(object), instance.Name, instance.Implementation);
            else container.RegisterInstance(instance.Service, instance.Name, instance.Implementation);
        }
    }
}