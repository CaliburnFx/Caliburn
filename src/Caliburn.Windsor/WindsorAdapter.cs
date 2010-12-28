namespace Caliburn.Windsor
{
    using System;
	using System.Collections.Generic;
    using System.Linq;
    using Core.InversionOfControl;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    /// <summary>
    /// An adapter allowing an <see cref="IWindsorContainer"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IRegistry"/>.
    /// </summary>
    public class WindsorAdapter : ContainerBase, IContainerAccessor
    {
        private readonly IWindsorContainer container;
    	private readonly ComponentInstaller installer = new ComponentInstaller();

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorAdapter"/> class.
        /// </summary>
        /// <param name="container">The _container.</param>
        public WindsorAdapter(IWindsorContainer container)
        {
            this.container = container;
        	this.container.Register(
				Component.For(typeof(IServiceLocator), typeof(IRegistry), typeof(IContainer), typeof(IContainerAccessor)).Instance(this),
				Component.For<IWindsorContainer>().Instance(this.container)
				);

            AddRegistrationHandler<Singleton>(HandleSingleton);
            AddRegistrationHandler<PerRequest>(HandlePerRequest);
            AddRegistrationHandler<Instance>(HandleInstance);
        }

    	/// <summary>
    	/// Configures the container using the provided component registrations.
    	/// </summary>
    	/// <param name="registrations">The component registrations.</param>
    	public override void Register(IEnumerable<IComponentRegistration> registrations)
		{
			base.Register(registrations);
    		container.Install(installer);
		}

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public IWindsorContainer Container
        {
            get { return container; }
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
        public override object GetInstance(Type serviceType, string key)
        {
            if (key != null)
            {
                if (container.Kernel.HasComponent(key))
                {
                    if (serviceType != null)
                        return container.Resolve(key, serviceType);
                    return container.Resolve(key, new Dictionary<string, object>());
                }

                return null;
            }

            return container.Kernel.HasComponent(serviceType)
                ? container.Resolve(serviceType)
                : null;
        }

        /// <summary>
        ///When implemented by inheriting classes, this method will do the actual work of
        ///resolving all the requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>
        /// Sequence of service instance objects.
        /// </returns>
        public override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return (object[])container.ResolveAll(serviceType);
        }

        /// <summary>
        /// Injects dependencies into the object.
        /// </summary>
        /// <param name="instance">The instance to build up.</param>
        public override void BuildUp(object instance)
        {
            var properties = instance.GetType()
                .GetProperties()
                .Where(p => p.CanWrite && p.PropertyType.IsPublic);

            foreach(var propertyInfo in properties)
            {
                if(container.Kernel.HasComponent(propertyInfo.PropertyType))
                    propertyInfo.SetValue(instance, container.Resolve(propertyInfo.PropertyType), null);
            }
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
            Container.Register(Component.For<Core.Behaviors.IProxyFactory>().ImplementedBy<T>());
            Container.AddFacility<ProxyBehaviorFacility>();
            return this;
        }

		/// <summary>
		/// Adds an <see cref="IRegistration" /> to the adapter's internal installer.
		/// </summary>
		/// <param name="registration">The registration to be added to the installer.</param>
		protected virtual void AddRegistration(IRegistration registration)
		{
			installer.AddRegistration(registration);
		}

        private void HandleSingleton(Singleton singleton)
        {
            if (!singleton.HasName())
                AddRegistration(Component.For(singleton.Service).ImplementedBy(singleton.Implementation).LifeStyle.Singleton);
            else if (!singleton.HasService())
                AddRegistration(Component.For(singleton.Implementation).Named(singleton.Name).LifeStyle.Singleton);
            else AddRegistration(Component.For(singleton.Service).ImplementedBy(singleton.Implementation).Named(singleton.Name).LifeStyle.Singleton);
        }

        private void HandlePerRequest(PerRequest perRequest)
        {
            if (!perRequest.HasName())
                AddRegistration(Component.For(perRequest.Service).ImplementedBy(perRequest.Implementation).LifeStyle.Transient);
            else if (!perRequest.HasService())
				AddRegistration(Component.For(perRequest.Implementation).Named(perRequest.Name).LifeStyle.Transient);
			else AddRegistration(Component.For(perRequest.Service).ImplementedBy(perRequest.Implementation).Named(perRequest.Name).LifeStyle.Transient);
		}

        private void HandleInstance(Instance instance)
        {
            if (!instance.HasName())
                AddRegistration(Component.For(instance.Service).Instance(instance.Implementation));
            else if (!instance.HasService())
				AddRegistration(Component.For(instance.Implementation.GetType()).Instance(instance.Implementation).Named(instance.Name));
			else AddRegistration(Component.For(instance.Service).Instance(instance.Implementation).Named(instance.Name));
		}
    }
}