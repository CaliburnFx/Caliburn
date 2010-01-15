namespace Caliburn.Core.Configuration
{
    /// <summary>
    /// Various extension methods related to modules and configuration.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Configures the core.
        /// </summary>
        /// <param name="hook">The hook.</param>
        /// <returns></returns>
        public static CoreConfiguration Core(this IModuleHook hook)
        {
            return CaliburnModule<CoreConfiguration>.Instance;
        }
    }
}