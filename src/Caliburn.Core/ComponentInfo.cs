namespace Caliburn.Core
{
    using System;

    /// <summary>
    /// Used to provide component info to a container configurator.
    /// </summary>
    public class ComponentInfo
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the service.
        /// </summary>
        /// <value>The service.</value>
        public Type Service { get; set; }

        /// <summary>
        /// Gets or sets the implementation.
        /// </summary>
        /// <value>The implementation.</value>
        public Type Implementation { get; set; }

        /// <summary>
        /// Gets or sets the lifetime.
        /// </summary>
        /// <value>The lifetime.</value>
        public ComponentLifetime Lifetime { get; set; }

        /// <summary>
        /// Gets or sets the custom lifetime type.
        /// </summary>
        /// <value>The custom lifetime type.</value>
        public Type CustomLifetimeType { get; set; }
    }
}