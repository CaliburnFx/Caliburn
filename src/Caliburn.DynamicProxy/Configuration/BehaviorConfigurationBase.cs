namespace Caliburn.DynamicProxy.Configuration
{
    using System;
    using System.Collections.Generic;
    using Castle.Core.Interceptor;
    using Core.Behaviors;

    /// <summary>
    /// A base class for <see cref="IBehaviorConfiguration{T}"/>
    /// </summary>
    /// <typeparam name="T">The behavior being configured.</typeparam>
    public abstract class BehaviorConfigurationBase<T> : IBehaviorConfiguration<T>
        where T : IBehavior
    {
        /// <summary>
        /// Gets the interceptors.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="behavior">The behavior being configured.</param>
        /// <returns>The interceptors.</returns>
        public abstract IEnumerable<IInterceptor> GetInterceptors(Type implementation, T behavior);

        /// <summary>
        /// Gets the interceptors.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="behavior">The behavior being configured.</param>
        /// <returns>The interceptors.</returns>
        public IEnumerable<IInterceptor> GetInterceptors(Type implementation, IBehavior behavior)
        {
            return GetInterceptors(implementation, (T)behavior);
        }
    }
}