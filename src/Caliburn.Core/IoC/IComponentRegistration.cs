namespace Caliburn.Core.IoC
{
    using System;

    /// <summary>
    /// Represents a component registration.
    /// </summary>
    public interface IComponentRegistration
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        string Name { get; }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <value>The service.</value>
        Type Service { get; }
    }
}