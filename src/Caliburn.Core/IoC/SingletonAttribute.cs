namespace Caliburn.Core.IoC
{
    using System;

    /// <summary>
    /// An attribute that directs Caliburn to register the component with a singleton lifetime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonAttribute : RegisterAttribute
    {
        private readonly string _name;
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
        /// <param name="name">The key.</param>
        /// <param name="service">The service.</param>
        public SingletonAttribute(string name, Type service)
        {
            _name = name;
            _service = service;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonAttribute"/> class.
        /// </summary>
        /// <param name="name">The key.</param>
        public SingletonAttribute(string name)
        {
            _name = name;
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
        /// Registers the type with the specified container.
        /// </summary>
        /// <param name="decoratedType">The decorated type.</param>
        public override IComponentRegistration GetComponentInfo(Type decoratedType)
        {
            if(string.IsNullOrEmpty(Name))
                return new Singleton
                {
                    Service = Service,
                    Implementation = decoratedType,
                };

            if(Service == null)
                return new Singleton
                {
                    Name = Name,
                    Implementation = decoratedType,
                };

            return new Singleton
            {
                Name = Name,
                Service = Service,
                Implementation = decoratedType,
            };
        }
    }
}