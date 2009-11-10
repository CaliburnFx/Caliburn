namespace Caliburn.Core.IoC
{
    /// <summary>
    /// Extension methods related to IoC.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Determines whether the specified registration has a key.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns>
        /// 	<c>true</c> if the specified registration has key; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasName(this IComponentRegistration registration)
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
        public static bool HasService(this IComponentRegistration registration)
        {
            return registration.Service != null;
        }
    }
}