namespace Caliburn.DynamicProxy.Configuration
{
    using System;
    using System.Collections.Generic;
    using Castle.Core.Interceptor;
    using Interceptors;
    using Microsoft.Practices.ServiceLocation;
    using PresentationFramework.Behaviors;
    using PresentationFramework.ViewModels;

    /// <summary>
    /// Configures the Validate behavior.
    /// </summary>
    public class ValidateConfiguration : BehaviorConfigurationBase<ValidateAttribute>
    {
        /// <summary>
        /// Gets the interceptors.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="behavior">The behavior being configured.</param>
        /// <returns>The interceptors.</returns>
        public override IEnumerable<IInterceptor> GetInterceptors(Type implementation, ValidateAttribute behavior)
        {
#if SILVERLIGHT_40 || NET
            yield return new DataErrorInfoInterceptor(ServiceLocator.Current.GetInstance<IValidator>());          
#elif SILVERLIGHT_30
            yield return new ExceptionValidatorInterceptor(ServiceLocator.Current.GetInstance<IValidator>());
#endif
            yield return ProxyInterceptor.Instance;
        }
    }
}