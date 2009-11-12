namespace Caliburn.Spring
{
    using System;
    using System.Collections.Generic;
    using Core.IoC;
    using global::Spring.Context.Support;
    using global::Spring.Objects.Factory;
    using global::Spring.Objects.Factory.Config;
    using global::Spring.Objects.Factory.Support;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// An adapter allowing an <see cref="GenericApplicationContext"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IRegistry"/>.
    /// </summary>
    public class SpringAdapter : ContainerBase
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

            AddRegistrationHandler<Singleton>(HandleSingleton);
            AddRegistrationHandler<PerRequest>(HandlePerRequest);
            AddRegistrationHandler<Instance>(HandleInstance);
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

        private void HandleSingleton(Singleton singleton)
        {
            _context.RegisterObjectDefinition(
                GetName(singleton),
                new RootObjectDefinition(
                    singleton.Implementation,
                    true
                    ) {AutowireMode = _autoWiringMode}
                );
        }

        private void HandlePerRequest(PerRequest perRequest)
        {
            _context.RegisterObjectDefinition(
                GetName(perRequest),
                new RootObjectDefinition(
                    perRequest.Implementation,
                    false
                    ) {AutowireMode = _autoWiringMode}
                );
        }

        private void HandleInstance(Instance instance)
        {
            _context.ObjectFactory.RegisterSingleton(GetName(instance), this);
        }

        private static string GetName(ComponentRegistrationBase registration)
        {
            return !registration.HasName() ? registration.Service.FullName : registration.Name;
        }
    }
}