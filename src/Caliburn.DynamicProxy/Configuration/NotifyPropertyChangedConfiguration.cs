namespace Caliburn.DynamicProxy.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Castle.Core.Interceptor;
    using Interceptors;
    using PresentationFramework.Behaviors;

    /// <summary>
    /// Configures the NotifyPropertyChanged behavior.
    /// </summary>
    public class NotifyPropertyChangedConfiguration : BehaviorConfigurationBase<NotifyPropertyChangedAttribute>
    {
        /// <summary>
        /// Gets the interceptors.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="behavior">The behavior being configured.</param>
        /// <returns>The interceptors.</returns>
        public override IEnumerable<IInterceptor> GetInterceptors(Type implementation, NotifyPropertyChangedAttribute behavior)
        {
            if(typeof(INotifyPropertyChanged).IsAssignableFrom(implementation))
                yield return new NotifyPropertyChangedNoInterfaceInterceptor(implementation, behavior);
            else yield return new NotifyPropertyChangedWithInterfaceInterceptor(implementation, behavior);

            yield return ProxyInterceptor.Instance;
        }
    }
}