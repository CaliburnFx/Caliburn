namespace Caliburn.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Various extension methods used by the framework.
    /// </summary>
    public static class ExtensionMethods
    {
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
        /// Safely converts an object to a string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted string or null, if the value was null.</returns>
        public static string SafeToString(this object value)
        {
            return value == null ? null : value.ToString();
        }
    }
}