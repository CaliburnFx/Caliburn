namespace Caliburn.StructureMap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Core.Behaviors;
    using Core.InversionOfControl;
    using global::StructureMap;
    using IContainer=Core.InversionOfControl.IContainer;
    using Instance=Core.InversionOfControl.Instance;
    using IRegistry=Core.InversionOfControl.IRegistry;

    /// <summary>
    /// An adapter allowing an <see cref="IContainer"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="Caliburn.Core.InversionOfControl.IRegistry"/>.
    /// </summary>
    public class StructureMapAdapter : ContainerBase
    {
        private readonly global::StructureMap.IContainer container;
        private ConfigurationExpression exp;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureMapAdapter"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public StructureMapAdapter(global::StructureMap.IContainer container)
        {
            this.container = container;

            this.container.Configure(reg => reg.For<IServiceLocator>().Add(this));
            this.container.Configure(reg => reg.For<IRegistry>().Add(this));
            this.container.Configure(reg => reg.For<IContainer>().Add(this));
            this.container.Configure(reg => reg.For<global::StructureMap.IContainer>().Add(this.container));

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
            return string.IsNullOrEmpty(key)
                    ? container.TryGetInstance(serviceType)
                    : container.TryGetInstance(serviceType ?? typeof(object), key);
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
            foreach(var obj in container.GetAllInstances(serviceType))
            {
                yield return obj;
            }
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
        /// Configures the container using the provided component registrations.
        /// </summary>
        /// <param name="registrations">The component registrations.</param>
        public override void Register(IEnumerable<IComponentRegistration> registrations)
        {
            container.Configure(
                exp =>{
                    this.exp = exp;
                    base.Register(registrations);
                    this.exp = null;
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
                    x.For<IProxyFactory>()
                        .Singleton()
                        .Use<T>();

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
                exp.For(singleton.Service)
                    .Singleton()
                    .Use(singleton.Implementation);
            else if(!singleton.HasService())
                exp.For(typeof(object))
                    .Singleton()
                    .Use(singleton.Implementation)
                    .Named(singleton.Name);
            else
                exp.For(singleton.Service)
                    .Singleton()
                    .Use(singleton.Implementation)
                    .Named(singleton.Name);
        }

        private void HandlePerRequest(PerRequest perRequest)
        {
            if (!perRequest.HasName())
                exp.For(perRequest.Service)
                    .LifecycleIs(InstanceScope.PerRequest)
                    .Use(perRequest.Implementation);
            else if (!perRequest.HasService())
                exp.For(typeof(object))
                    .LifecycleIs(InstanceScope.PerRequest)
                    .Use(perRequest.Implementation)
                    .Named(perRequest.Name);
            else
                exp.For(perRequest.Service)
                    .LifecycleIs(InstanceScope.PerRequest)
                    .Use(perRequest.Implementation)
                    .Named(perRequest.Name);
        }

        private void HandleInstance(Instance instance)
        {
            if(!instance.HasName())
                exp.For(instance.Service)
                    .Use(instance.Implementation);
            else if(!instance.HasService())
                exp.For(typeof(object))
                    .Use(instance.Implementation)
                    .Named(instance.Name);
            else
                exp.For(instance.Service)
                    .Use(instance.Implementation)
                    .Named(instance.Name);
        }
    }
}