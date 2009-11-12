﻿namespace Caliburn.Castle
{
    using System;
    using System.Collections.Generic;
    using Core.IoC;
    using global::Castle.Core;
    using global::Castle.MicroKernel;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.MicroKernel.Registration.Lifestyle;
    using global::Castle.Windsor;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// An adapter allowing an <see cref="IWindsorContainer"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IRegistry"/>.
    /// </summary>
    public class WindsorAdapter : ContainerBase, IContainerAccessor
    {
        private readonly IWindsorContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorAdapter"/> class.
        /// </summary>
        /// <param name="container">The _container.</param>
        public WindsorAdapter(IWindsorContainer container)
        {
            _container = container;

            _container.Kernel.AddComponentInstance<IServiceLocator>(this);
            _container.Kernel.AddComponentInstance<IRegistry>(this);
            _container.Kernel.AddComponentInstance<IContainer>(this);
            _container.Kernel.AddComponentInstance<IContainerAccessor>(this);
            _container.Kernel.AddComponentInstance<IWindsorContainer>(_container);

            AddRegistrationHandler<Singleton>(HandleSingleton);
            AddRegistrationHandler<PerRequest>(HandlePerRequest);
            AddRegistrationHandler<CustomLifetime>(HandleCustom);
            AddRegistrationHandler<Instance>(HandleInstance);
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public IWindsorContainer Container
        {
            get { return _container; }
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of resolving
        /// the requested service instance.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <param name="key">Name of registered service you want. May be null.</param>
        /// <returns>
        /// The requested service instance.
        /// </returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            if(key != null)
            {
                if(serviceType != null) return _container.Resolve(key, serviceType);
                return _container.Resolve(key);
            }

            return _container.Resolve(serviceType);
        }

        /// <summary>
        ///When implemented by inheriting classes, this method will do the actual work of
        ///resolving all the requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>
        /// Sequence of service instance objects.
        /// </returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return (object[])_container.ResolveAll(serviceType);
        }

        private void HandleSingleton(Singleton singleton)
        {
            if (!singleton.HasName())
                _container.Register(Component.For(singleton.Service).ImplementedBy(singleton.Implementation).LifeStyle.Singleton);
            else if (!singleton.HasService())
                _container.AddComponentLifeStyle(singleton.Name, singleton.Implementation, LifestyleType.Singleton);
            else _container.AddComponentLifeStyle(singleton.Name, singleton.Service, singleton.Implementation, LifestyleType.Singleton);
        }

        private void HandlePerRequest(PerRequest perRequest)
        {
            if (!perRequest.HasName())
                _container.Register(Component.For(perRequest.Service).ImplementedBy(perRequest.Implementation).LifeStyle.Transient);
            else if (!perRequest.HasService())
                _container.AddComponentLifeStyle(perRequest.Name, perRequest.Implementation, LifestyleType.Transient);
            else _container.AddComponentLifeStyle(perRequest.Name, perRequest.Service, perRequest.Implementation, LifestyleType.Transient);
        }

        private void HandleCustom(CustomLifetime customLifetime)
        {
            if (!customLifetime.HasName())
                throw new NotSupportedException();

            var method = typeof(LifestyleGroup<object>).GetMethod(
                "Custom",
                new[] {typeof(ILifestyleManager)}
                );

            var genericMethod = method.MakeGenericMethod(customLifetime.Lifetime);

            var lifestyle = Component.For(customLifetime.Service).ImplementedBy(customLifetime.Implementation).LifeStyle;
            _container.Register((ComponentRegistration<object>)genericMethod.Invoke(lifestyle, new object[0]));
        }

        private void HandleInstance(Instance instance)
        {
            if (!instance.HasName())
                _container.Register(Component.For(instance.Service).Instance(instance.Implementation));
            else if (!instance.HasService())
                _container.Kernel.AddComponentInstance(instance.Name, instance.Implementation);
            else _container.Register(Component.For(instance.Service).Named(instance.Name).Instance(instance.Implementation));
        }
    }
}