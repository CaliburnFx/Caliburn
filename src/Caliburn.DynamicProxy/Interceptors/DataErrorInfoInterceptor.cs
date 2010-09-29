#if SILVERLIGHT_40 || NET

namespace Caliburn.DynamicProxy.Interceptors
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Castle.DynamicProxy;
    using Core.Validation;

    /// <summary>
    /// An interceptor for <see cref="IDataErrorInfo"/>.
    /// </summary>
#if NET
	[System.Serializable]
#endif
    public class DataErrorInfoInterceptor : InterceptorBase
    {
        private readonly IValidator _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataErrorInfoInterceptor"/> class.
        /// </summary>
        /// <param name="validator">The validator.</param>
        public DataErrorInfoInterceptor(IValidator validator)
        {
            _validator = validator;
        }

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public override void Intercept(IInvocation invocation)
        {
            if (invocation.Method.DeclaringType.Equals(typeof(IDataErrorInfo)))
            {
                if ("get_Item".Equals(invocation.Method.Name))
                {
                    var messages = _validator
                        .Validate(invocation.Proxy, (string)invocation.Arguments[0])
                        .Select(x => x.Message)
                        .ToArray();

                    invocation.ReturnValue = string.Join(Environment.NewLine, messages);
                }
                else if ("get_Error".Equals(invocation.Method.Name))
                {
                    var messages = _validator.Validate(invocation.Proxy)
                        .Select(x => x.Message)
                        .ToArray();

                    invocation.ReturnValue = string.Join(Environment.NewLine, messages);
                }
            }
            else invocation.Proceed();
        }
    }
}

#endif