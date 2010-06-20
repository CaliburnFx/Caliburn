namespace Caliburn.DynamicProxy.Interceptors
{
    using Castle.Core.Interceptor;
    using Core.Behaviors;

    /// <summary>
    /// Implements <see cref="IProxy"/>.
	/// </summary>
#if NET
	[System.Serializable]
#endif
	public class ProxyInterceptor : InterceptorBase
    {
        private static readonly ProxyInterceptor _instance = new ProxyInterceptor();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ProxyInterceptor Instance
        {
            get { return _instance; }
        }

        private ProxyInterceptor() {}

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public override void Intercept(IInvocation invocation)
        {
            if(invocation.Method.DeclaringType.Equals(typeof(IProxy)))
            {
                if(invocation.Method.Name == "get_OriginalType")
                    invocation.ReturnValue = invocation.Proxy.GetType().BaseType;
            }
            else invocation.Proceed();
        }
    }
}