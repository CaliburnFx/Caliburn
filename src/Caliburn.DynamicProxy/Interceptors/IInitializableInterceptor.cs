namespace Caliburn.DynamicProxy.Interceptors
{
    using Castle.Core.Interceptor;

    /// <summary>
    /// An custom interceptor which is made aware of its proxy once immediately following proxy creation.
    /// </summary>
    public interface IInitializableInterceptor : IInterceptor
    {
        /// <summary>
        /// Initializes the interceptor with the specified proxy.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        void Initialize(object proxy);
    }
}