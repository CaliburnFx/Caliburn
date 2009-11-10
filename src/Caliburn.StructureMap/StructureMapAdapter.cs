namespace Caliburn.StructureMap
{
    using System;
    using System.Collections.Generic;
    using Core.IoC;
    using global::StructureMap;
    using global::StructureMap.Attributes;
    using Microsoft.Practices.ServiceLocation;
    using IContainer=Core.IoC.IContainer;
    using IRegistry=Core.IoC.IRegistry;

    /// <summary>
    /// An adapter allowing an <see cref="IContainer"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="Core.IoC.IRegistry"/>.
    /// </summary>
    public class StructureMapAdapter : ContainerBase
    {
        private readonly global::StructureMap.IContainer _container;
        private ConfigurationExpression _exp;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureMapAdapter"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public StructureMapAdapter(global::StructureMap.IContainer container)
        {
            _container = container;

            _container.Configure(reg => reg.InstanceOf<IServiceLocator>().IsThis(this));
            _container.Configure(reg => reg.InstanceOf<IRegistry>().IsThis(this));
            _container.Configure(reg => reg.InstanceOf<IContainer>().IsThis(this));
            _container.Configure(reg => reg.InstanceOf<global::StructureMap.IContainer>().IsThis(_container));

            AddRegistrationHandler<Singleton>(HandleSingleton);
            AddRegistrationHandler<PerRequest>(HandlePerRequest);
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
        /// Configures the container using the provided component registrations.
        /// </summary>
        /// <param name="registrations">The component registrations.</param>
        public override void Register(IEnumerable<IComponentRegistration> registrations)
        {
            _container.Configure(
                exp =>{
                    _exp = exp;
                    base.Register(registrations);
                    _exp = null;
                });
        }

        private void HandleSingleton(Singleton singleton)
        {
            if(!singleton.HasName())
                _exp.ForRequestedType(singleton.Service)
                    .CacheBy(InstanceScope.Singleton)
                    .TheDefaultIsConcreteType(singleton.Implementation);
            else if(!singleton.HasService())
                _exp.ForRequestedType(typeof(object))
                    .AddConcreteType(singleton.Implementation, singleton.Name)
                    .CacheBy(InstanceScope.Singleton);
            else
                _exp.ForRequestedType(singleton.Service)
                    .AddConcreteType(singleton.Implementation, singleton.Name)
                    .CacheBy(InstanceScope.Singleton);
        }

        private void HandlePerRequest(PerRequest perRequest)
        {
            if(!perRequest.HasName())
                _exp.ForRequestedType(perRequest.Service)
                    .CacheBy(InstanceScope.PerRequest)
                    .TheDefaultIsConcreteType(perRequest.Implementation);
            else if (!perRequest.HasService())
                _exp.ForRequestedType(typeof(object))
                    .AddConcreteType(perRequest.Implementation, perRequest.Name)
                    .CacheBy(InstanceScope.PerRequest);
            else
                _exp.ForRequestedType(perRequest.Service)
                    .AddConcreteType(perRequest.Implementation, perRequest.Name)
                    .CacheBy(InstanceScope.PerRequest);
        }
    }
}