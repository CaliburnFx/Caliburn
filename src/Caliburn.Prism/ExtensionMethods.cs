namespace Caliburn.Prism
{
    using System;
    using System.Windows;
    using Core.Configuration;

    /// <summary>
    /// Extension methods related to Prism integration.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Adds the composite application library module to the system.
        /// </summary>
        /// <param name="hook">The hook.</param>
        /// <param name="createShell">The create shell.</param>
        /// <returns></returns>
        public static CompositeApplicationLibraryConfiguration CompositeApplicationLibrary(this IModuleHook hook, Func<DependencyObject> createShell)
        {
            var module = CaliburnModule<CompositeApplicationLibraryConfiguration>.Instance;
            module.CreateShellUsing(createShell);
            return hook.Module(module);
        }
    }
}