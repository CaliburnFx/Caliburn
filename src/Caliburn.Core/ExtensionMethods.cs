namespace Caliburn.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            return member.GetCustomAttributes(typeof(T), inherit)
                .OfType<T>();
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
    }
}