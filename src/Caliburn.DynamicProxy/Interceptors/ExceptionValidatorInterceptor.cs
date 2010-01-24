#if SILVERLIGHT_30

namespace Caliburn.DynamicProxy.Interceptors
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Castle.Core.Interceptor;
    using PresentationFramework.ViewModels;

    public class ExceptionValidatorInterceptor : InterceptorBase
    {
        private readonly IValidator _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionValidatorInterceptor"/> class.
        /// </summary>
        /// <param name="validator">The validator.</param>
        public ExceptionValidatorInterceptor(IValidator validator)
        {
            _validator = validator;
        }

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public override void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            if(!invocation.Method.Name.StartsWith("set_"))
                return;

            var messages = _validator
                .Validate(invocation.Proxy, invocation.Method.Name.Substring(4))
                .Select(x => x.Message)
                .ToArray();

            if(!messages.Any())
                return;

            throw new ValidationException(string.Join(Environment.NewLine, messages));
        }
    }
}

#endif