namespace Caliburn.Core.IoC
{
    using System;

    /// <summary>
    /// A base class for implementations of <see cref="IComponentRegistration"/>.
    /// </summary>
    public abstract class ComponentRegistrationBase : IComponentRegistration
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <value>The service.</value>
        public Type Service { get; set; }
    }
}