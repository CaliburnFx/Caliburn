namespace Caliburn.DynamicProxy
{
    using Castle.Core.Interceptor;
    using Core.Behaviors;

    /// <summary>
    /// Implements <see cref="IProxy"/>.
    /// </summary>
    public class ProxyInterceptor : IInterceptor
    {
        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            if(invocation.Method.DeclaringType.Equals(typeof(IProxy)))
            {
                if(invocation.Method.Name == "get_OriginalType")
                {
                    invocation.ReturnValue = invocation.Proxy.GetType().BaseType;
                }
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}