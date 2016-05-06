namespace Caliburn.Core.Behaviors
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implemented by services that can create proxies.
    /// </summary>
    public interface IProxyFactory
    {
        /// <summary>
        /// Creates the proxy using the specified target.
        /// </summary>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="target">The target.</param>
        /// <param name="behaviors">The behaviors.</param>
        /// <returns>The proxy.</returns>
        object CreateProxyWithTarget(Type interfaceType, object target, IEnumerable<IBehavior> behaviors);

        /// <summary>
        /// Creates a proxy.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="behaviors">The proxy behaviors.</param>
        /// <param name="constructorArgs">The constructor args.</param>
        /// <returns>The proxy.</returns>
        object CreateProxy(Type type, IEnumerable<IBehavior> behaviors, IEnumerable<object> constructorArgs);
    }
}