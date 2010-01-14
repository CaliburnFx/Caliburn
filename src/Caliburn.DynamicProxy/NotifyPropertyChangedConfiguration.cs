namespace Caliburn.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Configures the NotifyPropertyChanged behavior.
    /// </summary>
    public class NotifyPropertyChangedConfiguration : IBehaviorConfiguration
    {
        public IEnumerable<Type> GetInterceptors(Type implementation)
        {
            if(typeof(INotifyPropertyChanged).IsAssignableFrom(implementation))
                yield return typeof(NotifyPropertyChangedNoInterfaceInterceptor);
            else yield return typeof(NotifyPropertyChangedWithInterfaceInterceptor);

            yield return typeof(ProxyInterceptor);
        }
    }
}