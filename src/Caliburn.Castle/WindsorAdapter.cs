namespace Caliburn.Castle
{
    using System;
    using System.Collections.Generic;
    using Core;
    using global::Castle.Core;
    using global::Castle.MicroKernel;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.MicroKernel.Registration.Lifestyle;
    using global::Castle.Windsor;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// An adapter allowing an <see cref="IWindsorContainer"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IConfigurator"/>.
    /// </summary>
    public class WindsorAdapter : ServiceLocatorImplBase, IContainerAccessor, IContainer
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
            _container.Kernel.AddComponentInstance<IConfigurator>(this);
            _container.Kernel.AddComponentInstance<IContainer>(this);
            _container.Kernel.AddComponentInstance<IContainerAccessor>(this);
            _container.Kernel.AddComponentInstance<IWindsorContainer>(_container);
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

        /// <summary>
        /// Configures the container with the provided components.
        /// </summary>
        /// <param name="components">The components.</param>
        public void ConfigureWith(IEnumerable<ComponentInfo> components)
        {
            foreach(var info in components)
            {
                switch (info.Lifetime)
                {
                    case ComponentLifetime.Singleton:
                        if(string.IsNullOrEmpty(info.Key))
                            _container.Register(
                                Component.For(info.Service).ImplementedBy(info.Implementation).LifeStyle.Singleton
                                );
                        else if(info.Service == null)
                            _container.AddComponentLifeStyle(info.Key, info.Implementation, LifestyleType.Singleton);
                        else
                            _container.AddComponentLifeStyle(info.Key, info.Service, info.Implementation,
                                                             LifestyleType.Singleton);
                        break;
                    case ComponentLifetime.Custom:
                        {
                            if(!string.IsNullOrEmpty(info.Key))
                                throw new NotSupportedException();

                            var method = typeof(LifestyleGroup<object>).GetMethod(
                                "Custom",
                                new[] {typeof(ILifestyleManager)}
                                );

                            var genericMethod = method.MakeGenericMethod(info.CustomLifetimeType);

                            var lifestyle = Component.For(info.Service).ImplementedBy(info.Implementation).LifeStyle;
                            _container.Register(
                                (ComponentRegistration<object>)genericMethod.Invoke(lifestyle, new object[0]));
                        }
                        break;
                    case ComponentLifetime.PerRequest:
                        if(string.IsNullOrEmpty(info.Key))
                            _container.Register(
                                Component.For(info.Service).ImplementedBy(info.Implementation).LifeStyle.Transient
                                );
                        else if(info.Service == null)
                            _container.AddComponentLifeStyle(info.Key, info.Implementation, LifestyleType.Transient);
                        else
                            _container.AddComponentLifeStyle(info.Key, info.Service, info.Implementation, LifestyleType.Transient);
                        break;
                    default:
                        throw new NotSupportedException(
                            string.Format("{0} is not supported in Windsor.", info.Lifetime)
                            );
                }
            }
        }
    }
}