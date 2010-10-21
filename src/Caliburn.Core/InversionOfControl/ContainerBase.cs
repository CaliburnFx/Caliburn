namespace Caliburn.Core.InversionOfControl
{
    using System;
    using System.Collections.Generic;
    using Behaviors;
    using Core;
    using Logging;

    /// <summary>
    /// A base class for IoC container implementations.
    /// </summary>
    public abstract class ContainerBase : IContainer
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(ContainerBase));
        private readonly Dictionary<Type, Delegate> handlers = new Dictionary<Type, Delegate>();

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

                if (handlers.TryGetValue(key, out handler))
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
                    var arg = GetInstance(info.ParameterType, null);
                    args.Add(arg);
                }
            }

            return args.ToArray();
        }

        /// <summary>
        /// Gets a service of the specified type.
        /// </summary>
        /// <param name="serviceType">The service to locate.</param>
        /// <returns>The located service.</returns>
        public object GetService(Type serviceType)
        {
            return GetInstance(serviceType, null);
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
        /// Gets an instance by type and/or key.
        /// </summary>
        /// <param name="serviceType">The type of service to locate.</param>
        /// <param name="key">The key for the service to locate.</param>
        /// <returns>The located service.</returns>
        public abstract object GetInstance(Type serviceType, string key);

        /// <summary>
        /// Locates all the instances of the type.
        /// </summary>
        /// <param name="serviceType">The type to locate all services of.</param>
        /// <returns>The located services.</returns>
        public abstract IEnumerable<object> GetAllInstances(Type serviceType);

        /// <summary>
        /// Injects dependencies into the object.
        /// </summary>
        /// <param name="instance">The instance to build up.</param>
        public abstract void BuildUp(object instance);

        /// <summary>
        /// Adds the registration handler.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler">The handler.</param>
        public void AddRegistrationHandler<T>(Action<T> handler)
            where T : IComponentRegistration
        {
            handlers[typeof(T)] = handler;
        }
    }
}