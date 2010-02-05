namespace Caliburn.Core.IoC
{
    using System;

    /// <summary>
    /// Represents a PerRequest registration.
    /// </summary>
    public class PerRequest : ComponentRegistrationBase
    {
        /// <summary>
        /// Gets or sets the implementation.
        /// </summary>
        /// <value>The implementation.</value>
        public Type Implementation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerRequest"/> class.
        /// </summary>
        public PerRequest() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="PerRequest"/> class.
        /// </summary>
        /// <param name="service">The type key.</param>
        public PerRequest(Type service)
        {
            Service = service;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerRequest"/> class.
        /// </summary>
        /// <param name="name">The key.</param>
        /// <param name="service">The type name.</param>
        public PerRequest(string name, Type service)
        {
            Name = name;
            Service = service;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerRequest"/> class.
        /// </summary>
        /// <param name="name">The key.</param>
        public PerRequest(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the component info.
        /// </summary>
        /// <param name="decoratedType">Type of the decorated.</param>
        /// <returns></returns>
        public override IComponentRegistration GetComponentInfo(Type decoratedType)
        {
            Implementation = decoratedType;
            if (Service == null) Service = decoratedType;
            return this;
        }
    }
}