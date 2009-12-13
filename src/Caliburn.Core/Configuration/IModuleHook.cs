namespace Caliburn.Core.Configuration
{
    using System.Reflection;

    /// <summary>
    /// Enables modules to attach their configuration to the core configuration.
    /// </summary>
    public interface IModuleHook
    {
        /// <summary>
        /// Adds assemblies to search for types registerable in the DI container.
        /// </summary>
        /// <param name="assembliesToInspect">The assemblies to register.</param>
        /// <returns></returns>
        IConfigurationBuilder Assemblies(params Assembly[] assembliesToInspect);

        /// <summary>
        /// Adds the module.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module">The module.</param>
        /// <returns></returns>
        T Module<T>(T module) where T : IModule;
    }
}