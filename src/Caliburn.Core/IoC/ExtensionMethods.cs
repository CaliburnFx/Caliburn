namespace Caliburn.Core.IoC
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extension methods related to IoC.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// The overridable implemenation of GetModelType.
        /// </summary>
        public static Func<Type, ConstructorInfo> SelectEligibleConstructorImplementation = DefaultSelectEligibleConstructor;

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