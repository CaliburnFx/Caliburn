namespace Caliburn.Core.Behaviors
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Hosts extension methods related to behaviors.
    /// </summary>
    public static class BehaviorExtensions
    {
        internal static Func<Type, bool> DefaultShouldCreateProxyImplementation = DefaultShouldCreateProxy;

        /// <summary>
        /// Checks whether the specified type should be proxied.
        /// </summary>
        /// <param name="typeToCheck">The type to check.</param>
        /// <returns></returns>
        public static bool ShouldCreateProxy(this Type typeToCheck)
        {
            return DefaultShouldCreateProxyImplementation(typeToCheck);
        }

        private static bool DefaultShouldCreateProxy(this Type typeToCheck)
        {
            return typeToCheck.GetAttributes<IBehavior>(true).Any() ||
                   typeToCheck.GetMethod(
                       "GetProxyType",
                       BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy
                       ) != null;
        }
    }
}