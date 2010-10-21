namespace Caliburn.Core.InversionOfControl
{
    using System;

    /// <summary>
    /// Represents a Singleton registration.
    /// </summary>
    public class Singleton : ComponentRegistrationBase
    {
        /// <summary>
        /// Gets or sets the implementation.
        /// </summary>
        /// <value>The implementation.</value>
        public Type Implementation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Singleton"/> class.
        /// </summary>
        public Singleton() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Singleton"/> class.
        /// </summary>
        /// <param name="service">The type key.</param>
        public Singleton(Type service)
        {
            Service = service;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Singleton"/> class.
        /// </summary>
        /// <param name="name">The key.</param>
        /// <param name="service">The service.</param>
        public Singleton(string name, Type service)
        {
            Name = name;
            Service = service;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Singleton"/> class.
        /// </summary>
        /// <param name="name">The key.</param>
        public Singleton(string name)
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