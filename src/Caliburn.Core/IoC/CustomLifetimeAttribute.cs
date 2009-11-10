namespace Caliburn.Core.IoC
{
    using System;

    /// <summary>
    /// An attribute that directs Caliburn to register the component with a custom lifetime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomLifetimeAttribute : RegisterAttribute
    {
        private readonly string _name;
        private readonly Type _service;
        private readonly Type _customLifetimeType;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomLifetimeAttribute"/> class.
        /// </summary>
        /// <param name="service">The type key.</param>
        /// <param name="customLifetimeType">The custom lifetime type.</param>
        public CustomLifetimeAttribute(Type service, Type customLifetimeType)
        {
            _service = service;
            _customLifetimeType = customLifetimeType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomLifetimeAttribute"/> class.
        /// </summary>
        /// <param name="name">The key.</param>
        /// <param name="customLifetimeType">Type of the custom lifetime.</param>
        public CustomLifetimeAttribute(string name, Type customLifetimeType)
        {
            _name = name;
            _customLifetimeType = customLifetimeType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomLifetimeAttribute"/> class.
        /// </summary>
        /// <param name="name">The key.</param>
        /// <param name="service">The service.</param>
        /// <param name="customLifetimeType">Type of the custom lifetime.</param>
        public CustomLifetimeAttribute(string name, Type service, Type customLifetimeType)
        {
            _name = name;
            _service = service;
            _customLifetimeType = customLifetimeType;
        }

        /// <summary>
        /// Gets the service type.
        /// </summary>
        /// <value>The service type.</value>
        public Type Service
        {
            get { return _service; }
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the lifetime.
        /// </summary>
        /// <value>The lifetime.</value>
        public Type CustomLifetimeType
        {
            get { return _customLifetimeType; }
        }

        /// <summary>
        /// Registers the type with the specified container.
        /// </summary>
        /// <param name="decoratedType">The decorated type.</param>
        public override IComponentRegistration GetComponentInfo(Type decoratedType)
        {
            if(string.IsNullOrEmpty(Name))
                return new CustomLifetime
                {
                    Service = Service,
                    Implementation = decoratedType,
                    Lifetime = CustomLifetimeType
                };

            if(Service == null)
                return new CustomLifetime
                {
                    Name = Name,
                    Implementation = decoratedType,
                    Lifetime = CustomLifetimeType
                };

            return new CustomLifetime
            {
                Name = Name,
                Service = Service,
                Implementation = decoratedType,
                Lifetime = CustomLifetimeType
            };
        }
    }
}