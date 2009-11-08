namespace Caliburn.Ninject
{
    using System;
    using System.Collections.Generic;
    using Core;
    using global::Ninject;
    using Microsoft.Practices.ServiceLocation;
    using ActivationException=global::Ninject.ActivationException;

    /// <summary>
    /// An adapter allowing an <see cref="IKernel"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IConfigurator"/>.
    /// </summary>
    public class NinjectAdapter : ServiceLocatorImplBase, IContainer
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
            Kernel.Bind<IConfigurator>().ToConstant(this);
            Kernel.Bind<IContainer>().ToConstant(this);
            Kernel.Bind<IKernel>().ToConstant(Kernel);
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
            var instance = Kernel.Get(serviceType ?? typeof(object), key);

            if(instance == null)
                throw new ActivationException();

            return instance;
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of
        /// resolving all the requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>Sequence of service instance objects.</returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return Kernel.GetAll(serviceType);
        }

        /// <summary>
        /// Configures the container with the provided components.
        /// </summary>
        /// <param name="components">The components.</param>
        public void ConfigureWith(IEnumerable<ComponentInfo> components)
        {
            foreach (var info in components)
            {
                switch (info.Lifetime)
                {
                    case ComponentLifetime.Singleton:
                        if(string.IsNullOrEmpty(info.Key))
                            Kernel.Bind(info.Service).To(info.Implementation).InSingletonScope();
                        else if(info.Service == null)
                            Kernel.Bind(typeof(object)).To(info.Implementation).InSingletonScope().Named(info.Key);
                        else Kernel.Bind(info.Service).To(info.Implementation).InSingletonScope().Named(info.Key);
                        break;
                    case ComponentLifetime.PerRequest:
                        if(string.IsNullOrEmpty(info.Key))
                            Kernel.Bind(info.Service).To(info.Implementation).InTransientScope();
                        else if(info.Service == null)
                            Kernel.Bind(typeof(object)).To(info.Implementation).InTransientScope().Named(info.Key);
                        else Kernel.Bind(info.Service).To(info.Implementation).InTransientScope().Named(info.Key);
                        break;
                    default:
                        throw new NotSupportedException(
                            string.Format("{0} is not supported in Ninject.", info.Lifetime)
                            );
                }
            }
        }
    }
}