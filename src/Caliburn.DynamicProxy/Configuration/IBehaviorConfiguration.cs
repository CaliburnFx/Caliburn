namespace Caliburn.DynamicProxy.Configuration
{
    using System;
    using System.Collections.Generic;
    using Castle.Core.Interceptor;
    using Core.Behaviors;

    /// <summary>
    /// Used to provide interceptors for a type based on a behavior.
    /// </summary>
    public interface IBehaviorConfiguration
    {
        /// <summary>
        /// Gets the interceptors.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="behavior">The behavior being configured.</param>
        /// <returns>The interceptors.</returns>
        IEnumerable<IInterceptor> GetInterceptors(Type implementation, IBehavior behavior);
    }

    /// <summary>
    /// A strongly typed version of <see cref="IBehaviorConfiguration"/>.
    /// </summary>
    /// <typeparam name="T">The type of the behavior to be configured.</typeparam>
    public interface IBehaviorConfiguration<T> : IBehaviorConfiguration where T : IBehavior
    {
        /// <summary>
        /// Gets the interceptors.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="behavior">The behavior being configured.</param>
        /// <returns>The interceptors.</returns>
        IEnumerable<IInterceptor> GetInterceptors(Type implementation, T behavior);
    }
}