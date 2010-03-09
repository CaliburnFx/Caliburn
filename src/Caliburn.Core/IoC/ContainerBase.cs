namespace Caliburn.Core.IoC
{
    using System;
    using System.Collections.Generic;
    using Behaviors;
    using Logging;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// A base class for IoC container implementations.
    /// </summary>
    public abstract class ContainerBase : ServiceLocatorImplBase, IContainer
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(ContainerBase));
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
                else 
                {
                    var exception = new CaliburnException(
                        string.Format(
                            "{0} cannot handle registrations of type {1}. Please add an appropriate registration handler.",
                            GetType().FullName,
                            key.FullName
                            )
                        );

                    Log.Error(exception);
                    throw exception;
                }
            }
        }

        /// <summary>
        /// Determines the constructor args.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <returns></returns>
        protected object[] DetermineConstructorArgs(Type implementation)
        {
            var args = new List<object>();
            var constructor = implementation.SelectEligibleConstructor();

            if (constructor != null)
            {
                foreach (var info in constructor.GetParameters())
                {
                    var arg = GetInstance(info.ParameterType);
                    args.Add(arg);
                }
            }

            return args.ToArray();
        }

        /// <summary>
        /// Installs a proxy factory.
        /// </summary>
        /// <typeparam name="T">The type of the proxy factory.</typeparam>
        /// <returns>
        /// A container with an installed proxy factory.
        /// </returns>
        public abstract IContainer WithProxyFactory<T>()
            where T : IProxyFactory;

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