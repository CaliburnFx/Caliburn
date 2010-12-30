namespace Caliburn.Core.InversionOfControl
{
    using System;
    using System.Collections.Generic;
    using Logging;

    /// <summary>
    /// Implemented by services which can locate other services.
    /// </summary>
    public interface IServiceLocator : IServiceProvider
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        ILog Log { get; set; }

        /// <summary>
        /// Gets an instance by type and/or key.
        /// </summary>
        /// <param name="serviceType">The type of service to locate.</param>
        /// <param name="key">The key for the service to locate.</param>
        /// <returns>The located service.</returns>
        object GetInstance(Type serviceType, string key);

        /// <summary>
        /// Locates all the instances of the type.
        /// </summary>
        /// <param name="serviceType">The type to locate all services of.</param>
        /// <returns>The located services.</returns>
        IEnumerable<object> GetAllInstances(Type serviceType);
    }
}