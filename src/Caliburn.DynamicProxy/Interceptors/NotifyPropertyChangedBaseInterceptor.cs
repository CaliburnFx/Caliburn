namespace Caliburn.DynamicProxy.Interceptors
{
    using System;
    using System.ComponentModel;
    using Castle.Core.Interceptor;
    using Core;
    using PresentationFramework.Behaviors;

    /// <summary>
    /// A base class for interceptors which handle <see cref="INotifyPropertyChanged"/>.
	/// </summary>
#if NET
	[System.Serializable]
#endif
	public abstract class NotifyPropertyChangedBaseInterceptor : InterceptorBase
    {
        private readonly NotificationProfile _profile;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyPropertyChangedBaseInterceptor"/> class.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="behavior">The behavior.</param>
        protected NotifyPropertyChangedBaseInterceptor(Type implementation, NotifyPropertyChangedAttribute behavior)
        {
            _profile = NotificationProfile.Get(implementation, behavior);
        }

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public override void Intercept(IInvocation invocation)
        {
            var methodName = invocation.Method.Name;

            if(!ShouldProceed(invocation))
                return;

            if(methodName.StartsWith("get_"))
                _profile.HandleGetter(GetPropertyName(methodName), invocation);
            else
            {
                invocation.Proceed();

                if(methodName.StartsWith("set_"))
                    TryRaiseChangeNotification(methodName, invocation.Proxy);
            }
        }

        /// <summary>
        /// Indicates whether the invocation should proceed.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <returns></returns>
        protected virtual bool ShouldProceed(IInvocation invocation)
        {
            return true;
        }

        /// <summary>
        /// Called to raise a property change notification.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected abstract void OnPropertyChanged(object sender, string propertyName);

        private void TryRaiseChangeNotification(string methodName, object proxy)
        {
            var propertyName = GetPropertyName(methodName);

            if(_profile.ShouldNotify(propertyName))
                OnPropertyChanged(proxy, propertyName);

            _profile.GetDependencies(propertyName).Apply(x =>{
                if(_profile.ShouldNotify(x))
                    OnPropertyChanged(proxy, x);
            });
        }

        private static string GetPropertyName(string methodName)
        {
            return methodName.Substring(4);
        }
    }
}