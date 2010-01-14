namespace Caliburn.DynamicProxy
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Used to provide interceptors for a type based on a behavior.
    /// </summary>
    public interface IBehaviorConfiguration
    {
        /// <summary>
        /// Gets the interceptors.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <returns>The interceptors.</returns>
        IEnumerable<Type> GetInterceptors(Type implementation);
    }
}