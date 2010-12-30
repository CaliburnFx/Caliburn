namespace Caliburn.Core.InversionOfControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extension methods related to IoC.
    /// </summary>
    public static class IoCExtensions
    {
        /// <summary>
        /// Gets a service of the specified type.
        /// </summary>
        /// <param name="locator">The service locator.</param>
        /// <param name="serviceType">The service to locate.</param>
        /// <returns>The located service.</returns>
        public static object GetInstance(this IServiceLocator locator, Type serviceType)
        {
            return locator.GetInstance(serviceType, null);
        }

        /// <summary>
        /// Gets a service of the specified type.
        /// </summary>
        /// <param name="locator">The service locator.</param>
        /// <typeparam name="TService">The service to locate.</typeparam>
        /// <returns>The located service.</returns>
        public static TService GetInstance<TService>(this IServiceLocator locator)
        {
            return (TService)locator.GetInstance(typeof(TService));
        }

        /// <summary>
        /// Gets a service based on type and key.
        /// </summary>
        /// <typeparam name="TService">The service type to locate.</typeparam>
        /// <param name="locator">The service locator.</param>
        /// <param name="key">The key of the service to locate.</param>
        /// <returns>The located service.</returns>
        public static TService GetInstance<TService>(this IServiceLocator locator, string key)
        {
            return (TService)locator.GetInstance(typeof(TService), key);
        }

        /// <summary>
        /// Gets all instances of the specified service type.
        /// </summary>
        /// <typeparam name="TService">The type of service to locate.</typeparam>
        /// <param name="locator">The service locator.</param>
        /// <returns>The located service.</returns>
        public static IEnumerable<TService> GetAllInstances<TService>(this IServiceLocator locator)
        {
            return locator.GetAllInstances(typeof(TService)).Cast<TService>();
        }

        internal static Func<Type, ConstructorInfo> SelectEligibleConstructorImplementation = DefaultSelectEligibleConstructor;

        /// <summary>
        /// Gets the preferred constructor for instantiation.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The constructor.</returns>
        public static ConstructorInfo SelectEligibleConstructor(this Type type)
        {
            return SelectEligibleConstructorImplementation(type);
        }

        private static ConstructorInfo DefaultSelectEligibleConstructor(Type type)
        {
            return (from c in type.GetConstructors()
                    orderby c.GetParameters().Length descending
                    select c).FirstOrDefault();
        }

        /// <summary>
        /// Determines whether the specified registration has a key.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns>
        /// 	<c>true</c> if the specified registration has key; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasName(this ComponentRegistrationBase registration)
        {
            return !string.IsNullOrEmpty(registration.Name);
        }

        /// <summary>
        /// Determines whether the specified registration has a service.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns>
        /// 	<c>true</c> if the specified registration has service; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasService(this ComponentRegistrationBase registration)
        {
            return registration.Service != null;
        }

        /// <summary>
        /// Determines whether the specified type is concrete.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type is concrete; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsConcrete(this Type type)
        {
            return !type.IsAbstract && !type.IsInterface;
        }

        /// <summary>
        /// Finds the interface that closes the open generic on the type if it exists.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="openGeneric">The open generic.</param>
        /// <returns>The interface type or null if not found.</returns>
        public static Type FindInterfaceThatCloses(this Type type, Type openGeneric)
        {
            if (!type.IsConcrete())
                return null;

            if (openGeneric.IsInterface)
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == openGeneric)
                    {
                        return interfaceType;
                    }
                }
            }
            else if (type.BaseType.IsGenericType &&
                    type.BaseType.GetGenericTypeDefinition() == openGeneric)
            {
                return type.BaseType;
            }

            return type.BaseType == typeof(object)
                       ? null
                       : FindInterfaceThatCloses(type.BaseType, openGeneric);
        }  
    }
}