namespace Caliburn.Autofac
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Core.IoC;
    using global::Autofac;
    using global::Autofac.Builder;
    using Microsoft.Practices.ServiceLocation;
    using IContainer=global::Autofac.IContainer;

    /// <summary>
    /// An adapter allowing an <see cref="IContext"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IRegistry"/>.
    /// </summary>
    public class AutofacAdapter : ContainerBase
    {
        private readonly IContainer _container;
        private ContainerBuilder _builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacAdapter"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public AutofacAdapter(IContainer container)
        {
            if(container == null)
                throw new ArgumentNullException("container");
            _container = container;

            var builder = new ContainerBuilder();

            builder.Register<IServiceLocator>(this);
            builder.Register<IRegistry>(this);
            builder.Register<Core.IoC.IContainer>(this);
            builder.Register<IContainer>(_container);

            builder.Build(_container);

            AddRegistrationHandler<Singleton>(HandleSingleton);
            AddRegistrationHandler<PerRequest>(HandlePerRequest);
            AddRegistrationHandler<Instance>(HandleInstance);
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public IContainer Container
        {
            get { return _container; }
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of resolving
        /// the requested service instance.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <param name="key">Name of registered service you want. May be null.</param>
        /// <returns>The requested service instance.</returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            return key != null ? _container.Resolve(key) : _container.Resolve(serviceType);
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of
        /// resolving all the requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>Sequence of service instance objects.</returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            var type = typeof(IEnumerable<>).MakeGenericType(serviceType);

            object instance;
            return _container.TryResolve(type, out instance) ? ((IEnumerable)instance).Cast<object>() : Enumerable.Empty<object>();
        }

        /// <summary>
        /// Configures the container using the provided component registrations.
        /// </summary>
        /// <param name="registrations">The component registrations.</param>
        public override void Register(IEnumerable<Core.IoC.IComponentRegistration> registrations)
        {
            _builder = new ContainerBuilder();

            base.Register(registrations);

            _builder.Build(_container);
            _builder = null;
        }

        private void HandleSingleton(Singleton singleton)
        {
            if (!singleton.HasName())
                _builder.Register(singleton.Implementation).As(singleton.Service).SingletonScoped();
            else if (!singleton.HasService())
                _builder.Register(singleton.Implementation).As(typeof(object)).Named(singleton.Name).SingletonScoped();
            else _builder.Register(singleton.Implementation).As(singleton.Service).Named(singleton.Name).SingletonScoped();
        }

        private void HandlePerRequest(PerRequest perRequest)
        {
            if (!perRequest.HasName())
                _builder.Register(perRequest.Implementation).As(perRequest.Service).FactoryScoped();
            else if (!perRequest.HasService())
                _builder.Register(perRequest.Implementation).As(typeof(object)).Named(perRequest.Name).FactoryScoped();
            else _builder.Register(perRequest.Implementation).As(perRequest.Service).Named(perRequest.Name).FactoryScoped();
        }

        private void HandleInstance(Instance instance)
        {
            if (!instance.HasName())
                _builder.Register(instance.Implementation).As(instance.Service);
            else if (!instance.HasService())
                _builder.Register(instance.Implementation).As(typeof(object)).Named(instance.Name);
            else _builder.Register(instance.Implementation).As(instance.Service).Named(instance.Name);
        }
    }
}