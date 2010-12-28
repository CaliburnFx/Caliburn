namespace Caliburn.Ninject
{
    using System;
    using System.Collections.Generic;
    using Core.Behaviors;
    using Core.InversionOfControl;
    using global::Ninject;
    using global::Ninject.Injection;

    /// <summary>
    /// An adapter allowing an <see cref="IKernel"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IRegistry"/>.
    /// </summary>
    public class NinjectAdapter : ContainerBase
    {
        /// <summary>
        /// Gets or sets the kernel.
        /// </summary>
        /// <value>The kernel.</value>
        public IKernel Kernel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectAdapter"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public NinjectAdapter(IKernel kernel)
        {
            Kernel = kernel;

            Kernel.Bind<IServiceLocator>().ToConstant(this);
            Kernel.Bind<IRegistry>().ToConstant(this);
            Kernel.Bind<IContainer>().ToConstant(this);
            Kernel.Bind<IKernel>().ToConstant(Kernel);

            AddRegistrationHandler<Singleton>(HandleSingleton);
            AddRegistrationHandler<PerRequest>(HandlePerRequest);
            AddRegistrationHandler<Instance>(HandleInstance);
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of resolving
        /// the requested service instance.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <param name="key">Name of registered service you want. May be null.</param>
        /// <returns>The requested service instance.</returns>
        public override object GetInstance(Type serviceType, string key)
        {
            return Kernel.TryGet(serviceType ?? typeof(object), key); 
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of
        /// resolving all the requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>Sequence of service instance objects.</returns>
        public override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Kernel.GetAll(serviceType);
        }

        /// <summary>
        /// Injects dependencies into the object.
        /// </summary>
        /// <param name="instance">The instance to build up.</param>
        public override void BuildUp(object instance)
        {
            Kernel.Inject(instance);
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
            Kernel.Components.RemoveAll<IInjectorFactory>();
            Kernel.Components.Add<IInjectorFactory, ProxyInjectorFactory>();

            Kernel.Bind<IProxyFactory>().To<T>().InSingletonScope();

            return this;
        }

        private void HandleSingleton(Singleton singleton)
        {
            if (!singleton.HasName())
                Kernel.Bind(singleton.Service).To(singleton.Implementation).InSingletonScope();
            else if (!singleton.HasService())
                Kernel.Bind(typeof(object)).To(singleton.Implementation).InSingletonScope().Named(singleton.Name);
            else Kernel.Bind(singleton.Service).To(singleton.Implementation).InSingletonScope().Named(singleton.Name);
        }

        private void HandlePerRequest(PerRequest perRequest)
        {
            if (!perRequest.HasName())
                Kernel.Bind(perRequest.Service).To(perRequest.Implementation).InTransientScope();
            else if (!perRequest.HasService())
                Kernel.Bind(typeof(object)).To(perRequest.Implementation).InTransientScope().Named(perRequest.Name);
            else Kernel.Bind(perRequest.Service).To(perRequest.Implementation).InTransientScope().Named(perRequest.Name);
        }

        private void HandleInstance(Instance instance)
        {
            if (!instance.HasName())
                Kernel.Bind(instance.Service).ToConstant(instance.Implementation);
            else if (!instance.HasService())
                Kernel.Bind(typeof(object)).ToConstant(instance.Implementation).Named(instance.Name);
            else Kernel.Bind(instance.Service).ToConstant(instance.Implementation).Named(instance.Name);
        }
    }
}