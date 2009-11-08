namespace Caliburn.Autofac
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using global::Autofac;
    using global::Autofac.Builder;
    using Microsoft.Practices.ServiceLocation;
    using IContainer=global::Autofac.IContainer;

    /// <summary>
    /// An adapter allowing an <see cref="IContext"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IConfigurator"/>.
    /// </summary>
    public class AutofacAdapter : ServiceLocatorImplBase, Core.IContainer
    {
        private readonly IContainer _container;

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
            builder.Register<IConfigurator>(this);
            builder.Register<Core.IContainer>(this);
            builder.Register<IContainer>(_container);

            builder.Build(_container);
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
        /// Configures the container with the provided components.
        /// </summary>
        /// <param name="components">The components.</param>
        public void ConfigureWith(IEnumerable<ComponentInfo> components)
        {
            var builder = new ContainerBuilder();

            foreach (var info in components)
            {
                switch (info.Lifetime)
                {
                    case ComponentLifetime.Singleton:
                        if (string.IsNullOrEmpty(info.Key))
                            builder.Register(info.Implementation).As(info.Service).SingletonScoped();
                        else if (info.Service == null)
                            builder.Register(info.Implementation).As(typeof(object)).Named(info.Key).SingletonScoped();
                        else builder.Register(info.Implementation).As(info.Service).Named(info.Key).SingletonScoped();
                        break;
                    case ComponentLifetime.PerRequest:
                        if (string.IsNullOrEmpty(info.Key))
                            builder.Register(info.Implementation).As(info.Service).FactoryScoped();
                        else if (info.Service == null)
                            builder.Register(info.Implementation).As(typeof(object)).Named(info.Key).FactoryScoped();
                        else builder.Register(info.Implementation).As(info.Service).Named(info.Key).FactoryScoped();
                        break;
                    default:
                        throw new NotSupportedException(
                            string.Format("{0} is not supported in Autofac.", info.Lifetime)
                            );
                }
            }

            builder.Build(_container);
        }
    }
}