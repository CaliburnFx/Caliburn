namespace Caliburn.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Various extension methods used by the framework.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <typeparam name="T">The attribute type.</typeparam>
        /// <param name="member">The member.</param>
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <returns>The attributes.</returns>
        public static IEnumerable<T> GetAttributes<T>(this MemberInfo member, bool inherit)
        {
            return Attribute.GetCustomAttributes(member, inherit).OfType<T>();
        }

        /// <summary>
        /// Applies the specified action to each item in the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The action.</param>
        public static void Apply<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach(var item in enumerable)
            {
                action(item);
            }
        }

        /// <summary>
        /// Gets the member info represented by an expression.
        /// </summary>
        /// <param name="expression">The member expression.</param>
        /// <returns>The member info represeted by the expression.</returns>
        public static MemberInfo GetMemberInfo(this Expression expression)
        {
            var lambda = (LambdaExpression)expression;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else memberExpression = (MemberExpression)lambda.Body;

            return memberExpression.Member;
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