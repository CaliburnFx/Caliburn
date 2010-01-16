namespace Caliburn.DynamicProxy.Configuration
{
    using System;
    using System.Collections.Generic;
    using Castle.Core.Interceptor;
    using Interceptors;
    using PresentationFramework.Behaviors;

    /// <summary>
    /// Configures the Screen behavior.
    /// </summary>
    public class ScreenConfiguration : BehaviorConfigurationBase<ScreenAttribute>
    {
        /// <summary>
        /// Gets the interceptors.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="behavior">The behavior being configured.</param>
        /// <returns>The interceptors.</returns>
        public override IEnumerable<IInterceptor> GetInterceptors(Type implementation, ScreenAttribute behavior)
        {
            yield return new ScreenInterceptor(behavior);
            yield return ProxyInterceptor.Instance;
        }
    }
}