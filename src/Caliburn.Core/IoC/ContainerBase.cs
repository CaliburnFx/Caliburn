namespace Caliburn.Core.IoC
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// A base class for IoC container implementations.
    /// </summary>
    public abstract class ContainerBase : ServiceLocatorImplBase, IContainer
    {
        private readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();

        /// <summary>
        /// Configures the container using the provided component registrations.
        /// </summary>
        /// <param name="registrations">The component registrations.</param>
        public virtual void Register(IEnumerable<IComponentRegistration> registrations)
        {
            foreach(var registration in registrations)
            {
                var key = registration.GetType();
                Delegate handler;

                if (_handlers.TryGetValue(key, out handler))
                    handler.DynamicInvoke(registration);
                else throw new CaliburnException(
                        string.Format(
                            "{0} cannot handle registrations of type {1}. Please add an appropriate registration handler.",
                            GetType().FullName,
                            key.FullName
                            )
                        );

            }
        }

        /// <summary>
        /// Adds the registration handler.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler">The handler.</param>
        public void AddRegistrationHandler<T>(Action<T> handler)
            where T : IComponentRegistration
        {
            _handlers[typeof(T)] = handler;
        }
    }
}