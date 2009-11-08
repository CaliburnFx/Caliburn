namespace Caliburn.Core.Metadata
{
    using System;

    /// <summary>
    /// An attribute that directs Caliburn to register the component with a singleton lifetime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonAttribute : RegisterAttribute
    {
        private readonly string _key;
        private readonly Type _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonAttribute"/> class.
        /// </summary>
        /// <param name="service">The type key.</param>
        public SingletonAttribute(Type service)
        {
            _service = service;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonAttribute"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="service">The service.</param>
        public SingletonAttribute(string key, Type service)
        {
            _key = key;
            _service = service;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonAttribute"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public SingletonAttribute(string key)
        {
            _key = key;
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
                    Lifetime = ComponentLifetime.Singleton
                };

            if(Service == null)
            {
                return new ComponentInfo
                {
                    Key = Key,
                    Implementation = decoratedType,
                    Lifetime = ComponentLifetime.Singleton
                };
            }

            return new ComponentInfo
            {
                Key = Key,
                Service = Service,
                Implementation = decoratedType,
                Lifetime = ComponentLifetime.Singleton
            };
        }
    }
}