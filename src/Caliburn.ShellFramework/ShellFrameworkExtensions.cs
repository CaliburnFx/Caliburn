namespace Caliburn.ShellFramework
{
    using Configuration;
    using Core.Configuration;

    /// <summary>
    /// General extension methods related to the shell framework.
    /// </summary>
    public static class ShellFrameworkExtensions
    {
        /// <summary>
        /// Adds the shell framework module's configuration to the system.
        /// </summary>
        /// <param name="hook">The hook.</param>
        public static ShellFrameworkConfiguration ShellFramework(this IModuleHook hook)
        {
            return hook.Module(CaliburnModule<ShellFrameworkConfiguration>.Instance);
        }
    }
}