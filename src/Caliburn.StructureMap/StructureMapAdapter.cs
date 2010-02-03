namespace Caliburn.StructureMap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Core.Behaviors;
    using Core.IoC;
    using global::StructureMap;
    using global::StructureMap.Attributes;
    using global::StructureMap.Pipeline;
    using Microsoft.Practices.ServiceLocation;
    using IContainer=Core.IoC.IContainer;
    using Instance=Core.IoC.Instance;
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
            AddRegistrationHandler<Instance>(HandleInstance);
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

        /// <summary>
        /// Installs a proxy factory.
        /// </summary>
        /// <typeparam name="T">The type of the proxy factory.</typeparam>
        /// <returns>
        /// A container with an installed proxy factory.
        /// </returns>
        public override IContainer WithProxyFactory<T>()
        {
            Container.Configure(
                x =>{
                    x.ForRequestedType<IProxyFactory>()
                        .TheDefaultIsConcreteType<T>()
                        .CacheBy(InstanceScope.Singleton);

                    x.IfTypeMatches(type => type.ShouldCreateProxy())
                        .InterceptWith((context, instance) =>{
                            var factory = context.GetInstance<IProxyFactory>();
                            var implementation = instance.GetType();

                            return factory.CreateProxy(
                                implementation,
                                implementation.GetAttributes<IBehavior>(true).ToArray(),
                                DetermineConstructorArgs(implementation)
                                );
                        });
                });

            return this;
        }

        private void HandleSingleton(Singleton singleton)
        {
            if(!singleton.HasName())
                _exp.For(singleton.Service)
                    .CacheBy(InstanceScope.Singleton)
                    .TheDefaultIsConcreteType(singleton.Implementation);
            else if(!singleton.HasService())
                _exp.For(typeof(object))
                    .CacheBy(InstanceScope.Singleton)
                    .TheDefaultIsConcreteType(singleton.Implementation)
                    .WithName(singleton.Name);
            else
                _exp.For(singleton.Service)
                    .CacheBy(InstanceScope.Singleton)
                    .TheDefaultIsConcreteType(singleton.Implementation)
                    .WithName(singleton.Name);
        }

        private void HandlePerRequest(PerRequest perRequest)
        {
            if (!perRequest.HasName())
                _exp.For(perRequest.Service)
                    .CacheBy(InstanceScope.PerRequest)
                    .TheDefaultIsConcreteType(perRequest.Implementation);
            else if (!perRequest.HasService())
                _exp.For(typeof(object))
                    .CacheBy(InstanceScope.PerRequest)
                    .TheDefaultIsConcreteType(perRequest.Implementation)
                    .WithName(perRequest.Name);
            else
                _exp.For(perRequest.Service)
                    .CacheBy(InstanceScope.PerRequest)
                    .TheDefaultIsConcreteType(perRequest.Implementation)
                    .WithName(perRequest.Name);
        }

        private void HandleInstance(Instance instance)
        {
            if (!instance.HasName())
                _exp.For(instance.Service)
                    .Add(new CustomInstance(instance.Implementation));
            else if (!instance.HasService())
                _exp.For(typeof(object))
                    .Add(new CustomInstance(instance.Implementation).WithName(instance.Name));
            else
                _exp.For(instance.Service)
                    .Add(new CustomInstance(instance.Implementation).WithName(instance.Name));
        }

        private class CustomInstance : LiteralInstance
        {
            public CustomInstance(object anObject) : 
                base(anObject) {}

            protected override bool canBePartOfPluginFamily(global::StructureMap.Graph.PluginFamily family)
            {
                return true;
            }
        }
    }
}