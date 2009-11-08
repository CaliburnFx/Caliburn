namespace Caliburn.Prism
{
    using System;
    using System.Windows;
    using Core;

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
        public static CompositeApplicationLibraryModule WithCompositeApplicationLibrary(this IConfigurationHook hook, Func<DependencyObject> createShell)
        {
            return new CompositeApplicationLibraryModule(hook, createShell);
        }
    }
}