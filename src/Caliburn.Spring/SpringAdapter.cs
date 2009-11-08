namespace Caliburn.Spring
{
    using System;
    using System.Collections.Generic;
    using Core;
    using global::Spring.Context.Support;
    using global::Spring.Objects.Factory;
    using global::Spring.Objects.Factory.Config;
    using global::Spring.Objects.Factory.Support;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// An adapter allowing an <see cref="GenericApplicationContext"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IConfigurator"/>.
    /// </summary>
    public class SpringAdapter : ServiceLocatorImplBase, IContainer
    {
        private readonly GenericApplicationContext _context;
        private readonly AutoWiringMode _autoWiringMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpringAdapter"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public SpringAdapter(GenericApplicationContext context)
            : this(context, AutoWiringMode.Constructor) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SpringAdapter"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="autoWiringMode">The auto wiring mode for component instantiation.</param>
        public SpringAdapter(GenericApplicationContext context, AutoWiringMode autoWiringMode)
        {
            _context = context;
            _autoWiringMode = autoWiringMode;

            _context.ObjectFactory.RegisterSingleton(
                typeof(IContainer).FullName,
                this
                );

            _context.ObjectFactory.RegisterSingleton(
                typeof(GenericApplicationContext).FullName,
                _context
                );
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public GenericApplicationContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Resolves a requested service instance.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <param name="key">Name of registered service you want. May be null.</param>
        /// <returns>
        /// The requested service instance or null, if <paramref name="key"/> is not found.
        /// </returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            if(key == null)
            {
                var it = DoGetAllInstances(serviceType).GetEnumerator();
                if(it.MoveNext())
                {
                    return it.Current;
                }
                throw new ObjectCreationException(string.Format("no services of type '{0}' defined",
                                                                serviceType.FullName));
            }

            if(serviceType == null) return _context.GetObject(key);

            return _context.GetObject(key, serviceType);
        }

        /// <summary>
        /// Resolves service instances by type.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>
        /// Sequence of service instance objects matching the <paramref name="serviceType"/>.
        /// </returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            foreach(var o in _context.GetObjectsOfType(serviceType).Values)
            {
                yield return o;
            }
        }

        /// <summary>
        /// Configures the container with the provided components.
        /// </summary>
        /// <param name="components">The components.</param>
        public void ConfigureWith(IEnumerable<ComponentInfo> components)
        {
            foreach (var info in components)
            {
                var key = string.IsNullOrEmpty(info.Key) ? info.Service.FullName : info.Key;

                switch(info.Lifetime)
                {
                    case ComponentLifetime.PerRequest:
                    case ComponentLifetime.Singleton:
                        _context.RegisterObjectDefinition(
                            key,
                            new RootObjectDefinition(
                                info.Implementation,
                                info.Lifetime == ComponentLifetime.Singleton
                                ) {AutowireMode = _autoWiringMode}
                            );
                        break;
                    default:
                        throw new NotSupportedException(
                            string.Format("{0} is not supported in Spring.NET.", info.Lifetime)
                            );
                }
            }
        }
    }
}