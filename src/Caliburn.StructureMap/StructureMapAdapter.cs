namespace Caliburn.StructureMap
{
    using System;
    using System.Collections.Generic;
    using Core;
    using global::StructureMap.Attributes;
    using Microsoft.Practices.ServiceLocation;
    using IContainer=Core.IContainer;

    /// <summary>
    /// An adapter allowing an <see cref="IContainer"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IConfigurator"/>.
    /// </summary>
    public class StructureMapAdapter : ServiceLocatorImplBase, IContainer
    {
        private readonly global::StructureMap.IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureMapAdapter"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public StructureMapAdapter(global::StructureMap.IContainer container)
        {
            _container = container;

            _container.Configure(reg => reg.InstanceOf<IServiceLocator>().IsThis(this));
            _container.Configure(reg => reg.InstanceOf<IConfigurator>().IsThis(this));
            _container.Configure(reg => reg.InstanceOf<IContainer>().IsThis(this));
            _container.Configure(reg => reg.InstanceOf<global::StructureMap.IContainer>().IsThis(_container));
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        [CLSCompliant(false)]
        public global::StructureMap.IContainer Container
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
            return string.IsNullOrEmpty(key)
                       ? _container.GetInstance(serviceType)
                       : _container.GetInstance(serviceType ?? typeof(object), key);
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
            foreach(var obj in _container.GetAllInstances(serviceType))
            {
                yield return obj;
            }
        }

        /// <summary>
        /// Configures the container with the provided components.
        /// </summary>
        /// <param name="components">The components.</param>
        public void ConfigureWith(IEnumerable<ComponentInfo> components)
        {
            _container.Configure(
                reg =>{
                    foreach(var info in components)
                    {
                        switch(info.Lifetime)
                        {
                            case ComponentLifetime.Singleton:
                                if (string.IsNullOrEmpty(info.Key))
                                    reg.ForRequestedType(info.Service)
                                        .CacheBy(InstanceScope.Singleton)
                                        .TheDefaultIsConcreteType(info.Implementation);
                                else if (info.Service == null)
                                    reg.ForRequestedType(typeof(object))
                                        .AddConcreteType(info.Implementation, info.Key)
                                        .CacheBy(InstanceScope.Singleton);
                                else
                                    reg.ForRequestedType(info.Service)
                                        .AddConcreteType(info.Implementation, info.Key)
                                        .CacheBy(InstanceScope.Singleton);
                                break;
                            case ComponentLifetime.PerRequest:
                                if (string.IsNullOrEmpty(info.Key))
                                    reg.ForRequestedType(info.Service)
                                        .CacheBy(InstanceScope.PerRequest)
                                        .TheDefaultIsConcreteType(info.Implementation);
                                else if (info.Service == null)
                                    reg.ForRequestedType(typeof(object))
                                        .AddConcreteType(info.Implementation, info.Key)
                                        .CacheBy(InstanceScope.PerRequest);
                                else
                                    reg.ForRequestedType(info.Service)
                                        .AddConcreteType(info.Implementation, info.Key)
                                        .CacheBy(InstanceScope.PerRequest);
                                break;
                            default:
                                throw new NotSupportedException(
                                    string.Format("{0} is not supported in StructureMap.", info.Lifetime)
                                    );
                        }
                    }
                });
        }
    }
}