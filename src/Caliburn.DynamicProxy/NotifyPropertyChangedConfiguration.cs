namespace Caliburn.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Castle.Core.Interceptor;
    using PresentationFramework.Behaviors;

    /// <summary>
    /// Configures the NotifyPropertyChanged behavior.
    /// </summary>
    public class NotifyPropertyChangedConfiguration : BehaviorConfigurationBase<NotifyPropertyChangedAttribute>
    {
        public override IEnumerable<IInterceptor> GetInterceptors(Type implementation, NotifyPropertyChangedAttribute behavior)
        {
            if(typeof(INotifyPropertyChanged).IsAssignableFrom(implementation))
                yield return new NotifyPropertyChangedNoInterfaceInterceptor(implementation, behavior);
            else yield return new NotifyPropertyChangedWithInterfaceInterceptor(implementation, behavior);

            yield return ProxyInterceptor.Instance;
        }
    }
}