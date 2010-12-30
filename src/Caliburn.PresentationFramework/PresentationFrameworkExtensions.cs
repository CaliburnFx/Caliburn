namespace Caliburn.PresentationFramework
{
    using Configuration;
    using Core.Configuration;

    /// <summary>
    /// Extension methods related to the PresentationFramework classes.
    /// </summary>
    public static class PresentationFrameworkExtensions
    {
        /// <summary>
        /// Adds the presentation framework module's configuration to the system.
        /// </summary>
        /// <param name="hook">The hook.</param>
        public static PresentationFrameworkConfiguration PresentationFramework(this IModuleHook hook)
        {
            return hook.Module(CaliburnModule<PresentationFrameworkConfiguration>.Instance);
        }

        /// <summary>
        /// Safely converts an object to a string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted string or null, if the value was null.</returns>
        internal static string SafeToString(this object value)
        {
            return value == null ? null : value.ToString();
        }
    }
}