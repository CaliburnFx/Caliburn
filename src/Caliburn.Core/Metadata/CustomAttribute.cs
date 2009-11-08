namespace Caliburn.Core.Metadata
{
    using System;

    /// <summary>
    /// An attribute that directs Caliburn to register the component with a custom lifetime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomAttribute : RegisterAttribute
    {
        private readonly string _key;
        private readonly Type _service;
        private readonly Type _customLifetimeType;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAttribute"/> class.
        /// </summary>
        /// <param name="service">The type key.</param>
        /// <param name="customLifetimeType">The custom lifetime type.</param>
        public CustomAttribute(Type service, Type customLifetimeType)
        {
            _service = service;
            _customLifetimeType = customLifetimeType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAttribute"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="customLifetimeType">Type of the custom lifetime.</param>
        public CustomAttribute(string key, Type customLifetimeType)
        {
            _key = key;
            _customLifetimeType = customLifetimeType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAttribute"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="service">The service.</param>
        /// <param name="customLifetimeType">Type of the custom lifetime.</param>
        public CustomAttribute(string key, Type service, Type customLifetimeType)
        {
            _key = key;
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
        public string Key
        {
            get { return _key; }
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
        public override ComponentInfo GetComponentInfo(Type decoratedType)
        {
            if (string.IsNullOrEmpty(Key))
                return new ComponentInfo
                {
                    Service = Service,
                    Implementation = decoratedType,
                    Lifetime = ComponentLifetime.Custom,
                    CustomLifetimeType = CustomLifetimeType
                };

            if (Service == null)
            {
                return new ComponentInfo
                {
                    Key = Key,
                    Implementation = decoratedType,
                    Lifetime = ComponentLifetime.Custom,
                    CustomLifetimeType = CustomLifetimeType
                };
            }

            return new ComponentInfo
            {
                Key = Key,
                Service = Service,
                Implementation = decoratedType,
                Lifetime = ComponentLifetime.Custom,
                CustomLifetimeType = CustomLifetimeType
            };
        }
    }
}