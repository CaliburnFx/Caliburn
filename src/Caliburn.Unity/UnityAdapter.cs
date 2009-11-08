namespace Caliburn.Unity
{
    using System;
    using System.Collections.Generic;
    using Core;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// An adapter allowing an <see cref="IUnityContainer"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IConfigurator"/>.
    /// </summary>
    public class UnityAdapter : ServiceLocatorImplBase, IContainer
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
            _container.RegisterInstance<IConfigurator>(this);
            _container.RegisterInstance<IContainer>(this);
            _container.RegisterInstance(_container);
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
        /// Configures the container with the provided components.
        /// </summary>
        /// <param name="components">The components.</param>
        public void ConfigureWith(IEnumerable<ComponentInfo> components)
        {
            //HACK: Unity doesn't support component registration with string key only
            //		Named service are registered as object

            foreach(var info in components)
            {
                switch(info.Lifetime)
                {
                    case ComponentLifetime.Singleton:
                        if(string.IsNullOrEmpty(info.Key))
                            _container.RegisterType(info.Service, info.Implementation, new ContainerControlledLifetimeManager());
                        else if(info.Service != null)
                            _container.RegisterType(info.Service, info.Implementation, info.Key, new ContainerControlledLifetimeManager());
                        else _container.RegisterType(typeof(object), info.Implementation, info.Key, new ContainerControlledLifetimeManager());
                        break;
                    case ComponentLifetime.Custom:
                        if (string.IsNullOrEmpty(info.Key))
                            _container.RegisterType(info.Service, info.Implementation, (LifetimeManager)Activator.CreateInstance(info.CustomLifetimeType));
                        else if (info.Service != null)
                            _container.RegisterType(info.Service, info.Implementation, info.Key, (LifetimeManager)Activator.CreateInstance(info.CustomLifetimeType));
                        else _container.RegisterType(typeof(object), info.Implementation, info.Key, (LifetimeManager)Activator.CreateInstance(info.CustomLifetimeType));
                        break;
                    case ComponentLifetime.PerRequest:
                        if (string.IsNullOrEmpty(info.Key))
                            _container.RegisterType(info.Service, info.Implementation);
                        else if (info.Service != null)
                            _container.RegisterType(info.Service, info.Implementation, info.Key);
                        else _container.RegisterType(typeof(object), info.Implementation, info.Key);
                        break;
                    default:
                        throw new NotSupportedException(
                            string.Format("{0} is not supported in Unity.", info.Lifetime)
                            );
                }
            }
        }
    }
}