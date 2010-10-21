namespace Caliburn.DynamicProxy.Configuration
{
    using System;
    using System.Collections.Generic;
	using Castle.DynamicProxy;
    using Core.InversionOfControl;
    using Core.Validation;
    using Interceptors;
    using PresentationFramework.Behaviors;

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
            yield return new DataErrorInfoInterceptor(IoC.Get<IValidator>());          
#else
            yield return new ExceptionValidatorInterceptor(IoC.Get<IValidator>());
#endif
            yield return ProxyInterceptor.Instance;
        }
    }
}