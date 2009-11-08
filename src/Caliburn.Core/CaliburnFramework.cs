namespace Caliburn.Core
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// A gateway for configuring Caliburn.
    /// </summary>
    public static class CaliburnFramework
    {
        /// <summary>
        /// Configures caliburn with the <see cref="SimpleContainer"/>.
        /// </summary>
        /// <returns></returns>
        public static CoreConfiguration ConfigureCore()
        {
            return ConfigureCore(new SimpleContainer());
        }

        /// <summary>
        /// Configures Caliburn with the specified container implementation.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        public static CoreConfiguration ConfigureCore(IContainer container)
        {
            return ConfigureCore(container, container.ConfigureWith);
        }

        /// <summary>
        /// Configures Caliburn with the specified <see cref="IServiceLocator"/> and configurator <see cref="IConfigurator"/>.
        /// </summary>
        /// <param name="serviceLocator">The serviceLocator.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static CoreConfiguration ConfigureCore(IServiceLocator serviceLocator, IConfigurator configurator)
        {
            return ConfigureCore(serviceLocator, configurator.ConfigureWith);
        }

        /// <summary>
        /// Configures Caliburn with the specified <see cref="IServiceLocator"/> and configurator method.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static CoreConfiguration ConfigureCore(IServiceLocator serviceLocator, Action<IEnumerable<ComponentInfo>> configurator)
        {
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
            return new CoreConfiguration(serviceLocator, configurator);
        }
    }
}