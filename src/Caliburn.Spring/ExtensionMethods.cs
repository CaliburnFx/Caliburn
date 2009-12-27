namespace Caliburn.Spring
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using global::Spring.Context;

    /// <summary>
    /// Hosts extension methods for a Spring <see cref="IApplicationContext"/>.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the objects of a type recursively.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public static IEnumerable<object> GetObjectsOfTypeRecursive(this IApplicationContext context, Type serviceType)
        {
            ICollection<object> result = new Collection<object>();
            GetAllInstancesRecursive(context, serviceType, ref result);
            return result;
        }

        private static void GetAllInstancesRecursive(IApplicationContext context, Type serviceType, ref ICollection<object> result)
        {
            if (context == null)
                return;

            foreach (var o in context.GetObjectsOfType(serviceType).Values)
            {
                result.Add(o);
            }

            GetAllInstancesRecursive(context.ParentContext, serviceType, ref result);
        }
    }
}