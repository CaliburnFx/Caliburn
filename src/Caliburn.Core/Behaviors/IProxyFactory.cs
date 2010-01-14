namespace Caliburn.Core.Behaviors
{
    using System;

    /// <summary>
    /// Implemented by services that can create proxies.
    /// </summary>
    public interface IProxyFactory
    {
        /// <summary>
        /// Creates a proxy.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="behaviors">The proxy behaviors.</param>
        /// <param name="constructorArgs">The constructor args.</param>
        /// <returns>The proxy.</returns>
        object CreateProxy(Type type, IBehavior[] behaviors, object[] constructorArgs);
    }
}