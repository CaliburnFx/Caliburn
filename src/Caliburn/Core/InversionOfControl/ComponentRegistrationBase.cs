namespace Caliburn.Core.InversionOfControl
{
    using System;

    /// <summary>
    /// A base class for implementations of <see cref="IComponentRegistration"/>.
    /// </summary>
    public abstract class ComponentRegistrationBase : Attribute, IComponentRegistration, IComponentMetadata
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

        /// <summary>
        /// Gets the component info.
        /// </summary>
        /// <param name="decoratedType">Type of the decorated.</param>
        /// <returns></returns>
        public abstract IComponentRegistration GetComponentInfo(Type decoratedType);
    }
}